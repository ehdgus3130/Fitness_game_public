using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroppableUI : MonoBehaviour, IDropHandler, IPointerExitHandler, IPointerEnterHandler
{
    private const string NestedScrollTag = "NestedScrollManager";
    private Image _image;
    //private RectTransform _rect;
    static Dictionary<string, int> _lv1Idx, _lv2Idx, _lv3Idx; //합성용
    private void Awake()
    {
        _image = GetComponent<Image>(); //_rect = GetComponent<RectTransform>();

        BuildCachesIfNeeded();
    }
    private static void EnsureCaches()
    {
        if (_lv1Idx == null || _lv2Idx == null || _lv3Idx == null ||
            (_lv1Idx.Count + _lv2Idx.Count + _lv3Idx.Count) == 0)
        {
            BuildCachesIfNeeded();
        }
    }
    static void BuildCachesIfNeeded()
    {
        _lv1Idx ??= new(StringComparer.Ordinal);
        _lv2Idx ??= new(StringComparer.Ordinal);
        _lv3Idx ??= new(StringComparer.Ordinal);
        _lv1Idx.Clear(); _lv2Idx.Clear(); _lv3Idx.Clear();

        var q = RoutineQueueManager.Instance;
        if (q?.Lv1 != null) for (int i = 0; i < q.Lv1.Length; i++) if (q.Lv1[i]) _lv1Idx[q.Lv1[i].tag] = i;
        if (q?.Lv2 != null) for (int i = 0; i < q.Lv2.Length; i++) if (q.Lv2[i]) _lv2Idx[q.Lv2[i].tag] = i;
        if (q?.Lv3 != null) for (int i = 0; i < q.Lv3.Length; i++) if (q.Lv3[i]) _lv3Idx[q.Lv3[i].tag] = i;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_image != null) _image.color = Color.yellow;
    }
    public void OnDrop(PointerEventData eventData)
    {
        EnsureCaches();
        var dragGO = eventData.pointerDrag;
        if (!dragGO || dragGO.CompareTag(NestedScrollTag)) return; // 화면 드랍은 무시

        var drag = dragGO.GetComponent<DraggableUI>();
        var targetSlot = GetComponent<SlotIndex>();
        if (!drag || !targetSlot) return; //드래그 대상과 슬롯 확인

        if (!drag.TryGetPrevSlotIndex(out var from)) return; //만약 이미 존재하는 위치에 드랍
        int to = targetSlot.Index;

        var parent = targetSlot.GetContentRoot(); //자식 위치 확인
        GameObject existing = (parent.childCount > 0) ? parent.GetChild(0).gameObject : null;
        Debug.Log($"{existing} {dragGO} is hitting");
        if (existing && CanMerge(existing, dragGO, out GameObject nextPrefab))
        {
            // 프리팹 교체
            Destroy(existing);
            Destroy(dragGO);
            var newGo = Instantiate(nextPrefab, parent);

            // Slot[] 갱신
            LevelUpScreen.Instance.Slot[from] = null;
            LevelUpScreen.Instance.Slot[to] = newGo;

            // 드롭 성공 신호
            drag.WasDropped = true;
            return;
        }

        // 합성 불가 && 슬롯이 비어있지 않다면 → 드롭 거절(원복)
        if (existing != null)
        {
            drag.WasDropped = false; // OnEndDrag에서 원복
            return;
        }

        dragGO.transform.SetParent(parent, true);
        var rt = dragGO.GetComponent<RectTransform>();
        if (rt) rt.anchoredPosition = Vector2.zero;

        LevelUpScreen.Instance.Slot[from] = null;
        LevelUpScreen.Instance.Slot[to] = dragGO;
        drag.WasDropped = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_image != null) _image.color = Color.white;
    }

    private static bool CanMerge(GameObject a, GameObject b, out GameObject next)
    {
        next = null;
        if (!a || !b) { Debug.Log("[Merge] fail: null obj"); return false; }
        if (!a.CompareTag(b.tag))
        {
            Debug.Log($"[Merge] fail: tag mismatch a={a.tag}, b={b.tag}"); return false;
        }

        int tier = NameToTier(a.name);
        if (tier == 0) tier = TagToTier(a.tag);
        if (tier == 0) { Debug.Log($"[Merge] fail: tier not detected name={a.name}, tag={a.tag}"); return false; }
        if (tier >= 4) { Debug.Log("[Merge] fail: already top tier"); return false; }

        var q = RoutineQueueManager.Instance;
        bool ok = false;

        if (tier == 1 && _lv1Idx.TryGetValue(a.tag, out int i1))
            ok = (next = (q?.Lv2 != null && i1 < q.Lv2.Length) ? q.Lv2[i1] : null) != null;
        else if (tier == 2 && _lv2Idx.TryGetValue(a.tag, out int i2))
            ok = (next = (q?.Lv3 != null && i2 < q.Lv3.Length) ? q.Lv3[i2] : null) != null;
        else if (tier == 3 && _lv3Idx.TryGetValue(a.tag, out int i3))
            ok = (next = (q?.Lv4 != null && i3 < q.Lv4.Length) ? q.Lv4[i3] : null) != null;
        else
            Debug.Log($"[Merge] fail: no index for tag={a.tag} at tier={tier}");

        if (!ok)
            Debug.Log($"[Merge] fail: next prefab not found (tier={tier}, tag={a.tag})");

        return ok;
    }

    private static int NameToTier(string n)
    {
        if (string.IsNullOrEmpty(n)) return 0;
        n = n.ToLowerInvariant();
        if (n.Contains("blue")) return 1;
        if (n.Contains("green")) return 2;
        if (n.Contains("yellow")) return 3;
        if (n.Contains("red")) return 4;
        return 0;
    }
    private static int TagToTier(string tag)
    {
        if (string.IsNullOrEmpty(tag)) return 0;
        if (tag.StartsWith("BLUE_", StringComparison.Ordinal)) return 1;
        if (tag.StartsWith("GREEN_", StringComparison.Ordinal)) return 2;
        if (tag.StartsWith("YELLOW_", StringComparison.Ordinal)) return 3;
        if (tag.StartsWith("RED_", StringComparison.Ordinal)) return 4;
        return 0;
    }
}
