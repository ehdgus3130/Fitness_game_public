using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class LevelUpScreen : Singleton<LevelUpScreen>
{
    //private RectTransform CanvasRect;
    //public GameObject SlotPre;

    [Header("Level Panel")]
    [SerializeField] private Transform LevelPanel;
    public GameObject[] LvList;  //Show level gap UI
    public Image[] fills; //Abs,Arm,Back,Chest,Leg,SHoulder

    [Header("Item Panel")]
    [SerializeField] private Transform ItemsPanel;
    public GameObject[] Slot;

    [Header("Score Panel")]
    [SerializeField] private Transform ScorePanel;
    public GameObject[] ScoreList;

    public AudioSource PlusSound;
    public AudioSource LevelUpSound;
    private Dictionary<string, GameObject> _prefabByName;
    private Dictionary<string, (int tier, float gain)> _tierByTag;
    void Awake()
    {
        LvList = new GameObject[6];
        fills = new Image[6];
        ScoreList = new GameObject[6];

        for (int i = 0; i < 6; i++)
        {
            LvList[i] = LevelPanel.GetChild(i).gameObject;
            fills[i] = LvList[i].transform.GetChild(0).GetComponent<Image>();
            ScoreList[i] = ScorePanel.GetChild(i).gameObject;
        }
        _prefabByName = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        _tierByTag = new Dictionary<string, (int, float)>(StringComparer.OrdinalIgnoreCase);
        Slot = new GameObject[24];

    }
    void OnEnable()
    {

        var q = RoutineQueueManager.Instance;
        void AddTier(GameObject[] arr, int tier, float gain)
        {
            if (arr == null) return;
            foreach (var go in arr) if (go != null)
                {
                    _tierByTag[go.tag] = (tier, gain);
                }

        }
        AddTier(q.Lv1, 1, 30f);
        AddTier(q.Lv2, 2, 100f);
        AddTier(q.Lv3, 3, 250f);
        AddTier(q.Lv4, 4, 600f);
        BuildPrefabCache();

    }

    private void EnsureTierMap()
    {
        if (_tierByTag != null && _tierByTag.Count > 0) return;

        var q = RoutineQueueManager.Instance;
        if (q == null) return; // 아직 RoutineQueueManager가 준비 안 됨 → 다음에 다시 시도

        void AddTier(GameObject[] arr, int tier, float gain)
        {
            if (arr == null) return;
            foreach (var go in arr)
            {
                if (!go) continue;
                _tierByTag[go.tag] = (tier, gain);
            }
        }

        AddTier(q.Lv1, 1, 30f);
        AddTier(q.Lv2, 2, 100f);
        AddTier(q.Lv3, 3, 250f);
        AddTier(q.Lv4, 4, 600f);
    }
    private void BuildPrefabCache()
    {
        if (_prefabByName.Count > 0) return;
        var q = RoutineQueueManager.Instance;
        if (q == null) return;
        void AddAll(GameObject[] arr) { if (arr == null) return; foreach (var go in arr) if (go) _prefabByName[go.name] = go; }
        AddAll(q.Lv1); AddAll(q.Lv2); AddAll(q.Lv3); AddAll(q.Lv4);
    }
    /// <summary>
    /// Check New Character Unlock
    /// sorting score the level position
    /// </summary>
    public void Check_Lv_Portion()
    {
        var pairs = new List<(GameObject go, float score)>(ScoreList.Length);
        for (int i = 0; i < ScoreList.Length; i++)
        {
            var t = ScoreList[i].transform.GetChild(0).GetComponent<TMP_Text>();
            float s = 0;
            if (t != null)
            {
                var txt = t.text;
                // 끝에 %가 붙는 포맷만 빠르게 처리
                if (txt.EndsWith("%", StringComparison.Ordinal))
                    float.TryParse(txt.AsSpan(0, txt.Length - 1), out s);
                else
                    float.TryParse(txt, out s);
            }
            pairs.Add((ScoreList[i], s));
        }
        pairs.Sort((a, b) => b.score.CompareTo(a.score));

        // 100% 체크 + 시블링 재배치
        int full = 0;
        for (int i = 0; i < pairs.Count; i++)
        {
            if (pairs[i].score >= 100f) full++;
            pairs[i].go.transform.SetSiblingIndex(i);
        }
        if (full == 6) GameManager.Instance.UnlockCharacter(DataManager.Instance.GetCurrentPlayerId());
    }


    /// <summary>
    /// Fill the Image of Fill Bar
    /// </summary>
    /// <param name="value"> current exp </param>
    /// <param name="MAX"> max exp</param>
    /// <param name="part"> which exp is used? </param>
    public void Set_FillAmount(float value, float MAX, Image part) => part.fillAmount = value / MAX;

    /// <summary>
    /// Show Level up Image for seconds
    /// </summary>
    /// <param name="name">which muscle level up image active</param>
    /// <returns></returns>
    public void ShowLevelUP(GameObject name)
    {
        GameObject Up = name.transform.GetChild(0).gameObject; //Level Up Image
        if (Up.activeSelf == false)
        {
            StartCoroutine(UP(Up));
        }
    }
    private IEnumerator UP(GameObject name)
    {
        if (!Settings.Instance.IsSoundEnabled) LevelUpSound.Play();

        name.SetActive(true);
        var alph = name.GetComponent<CanvasGroup>();
        alph.alpha = 1;

        yield return new WaitForSeconds(2f);

        float timer = 0.0f;

        while (timer < 2.0f)
        {
            alph.alpha = Mathf.Lerp(1.0f, 0.0f, timer / 2);
            timer += Time.deltaTime;
            yield return null;
        }

        name.SetActive(false);
        alph.alpha = 0;
    }

    public void LevelUP_Routin(GameObject muscle)      //레벨 증가요소
    {
        if (!muscle) return;
        EnsureTierMap();

        if (_tierByTag.TryGetValue(muscle.tag, out var info))
        {
            DataManager.Instance.LevelUp(muscle, info.gain);
        }


        Check_Lv_Portion();

    }
    public void OnAll_Click()    //전체 진행
    {
        for (int i = 0; i < Slot.Length; i++)
        {
            var item = Slot[i];
            if (item != null)
            {
                Debug.Log($"{item.name} found at All CLick");
                LevelUP_Routin(item);
                Slot[i] = null;

                var root = GetSlotRoot(i);
                if (root.childCount > 0) Destroy(root.GetChild(0).gameObject);
            }
        }
        if (!Settings.Instance.IsSoundEnabled) PlusSound.Play();
    }

    public void OnDragMuscle(int num) => Slot[num] = null;
    public void EndRoutin(GameObject Routin)        //가져오는 루틴이 색에 따라 1렙 2렙 이런식으로 랜덤으로 가져옴
    {
        BuildPrefabCache();

        var key = Routin.name.Replace("(Clone)", "");

        // 2) 프리팹 찾기: 선형탐색 대신 캐시
        if (!_prefabByName.TryGetValue(key, out var prefab)) return;

        // 3) 빈칸 찾기: UI childCount 대신 Slot[] 기준
        int idx = FindBlank();

        if (idx < 0)
        {
            // 빈칸이 없으면 바로 레벨업 처리
            LevelUP_Routin(prefab); // 원본의 "빈칸 없으면 레벨업"
            return;
        }

        // 4) 상태 먼저 기록 → 화면 반영 (일관된 순서)
        Slot[idx] = prefab;

        // 5) 화면에 붙이기(슬롯의 ContentRoot 기준)
        var root = GetSlotRoot(idx);
        var go = Instantiate(prefab, root);
        var img = go.GetComponent<Image>(); if (img) img.enabled = true; 
        var drag = go.GetComponent<DraggableUI>(); if (drag) drag.enabled = true; 
        Slot[idx] = go;
    }
    private int FindBlank()
    {
        for (int i = 0; i < Slot.Length; i++) if (Slot[i] == null) return i;
        return -1;
    }

    private Transform GetSlotRoot(int i)
    {
        var si = ItemsPanel.GetChild(i).GetComponent<SlotIndex>();
        return si ? (Transform)si.GetContentRoot() : ItemsPanel.GetChild(i);
    }
    public void reset_()
    {
        foreach (GameObject list in ScoreList)
        {
            list.transform.GetChild(0).GetComponent<TMP_Text>().text = "0";
        } //LevelList의 글자와 채우기 위치
        foreach (Image list in fills)
        {
            list.fillAmount = 0;
        }
    }
}
