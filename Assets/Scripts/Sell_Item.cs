using System;
using Unity;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sell_Item : Singleton<Sell_Item>
{
    [Header("UI")]
    private GameObject[] blue, green, yellow, red;
    [SerializeField] private Sprite[] BackgroundByTier;
    public Image coreimg;
    public GameObject ItemPrice;
    public Image ITEMPRICE;
    public TMP_Text priceText;
    private List<Itm> allItems;
    private Dictionary<int, List<Itm>> itemsByRate;
    private Image selfBg;
    void Awake()
    {
        blue = QueueRoutin.Instance.Lv1;
        green = QueueRoutin.Instance.Lv2;
        yellow = QueueRoutin.Instance.Lv3;
        red = QueueRoutin.Instance.Lv4;

        //GameItems = DataManager.Instance.Items;
        selfBg = GetComponent<Image>();

        allItems = DataManager.Instance.Items ?? new List<Itm>();
        itemsByRate = allItems
            .GroupBy(i => i.rate)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
    private static readonly (int rate, int cum)[] TableHow1 =
    {
        (0, 90), // 0~89
        (1, 98), // 90~97
        (2,100), // 98~99
    };

    private static readonly (int rate, int cum)[] TableHow6 =
    {
        (0, 50), // 0~49
        (1, 82), // 50~81
        (2, 92), // 82~91
        (3, 97), // 92~96
        (4, 99), // 97~98
        (5,100), // 99
    };
    private int RollRate(int how)
    {
        var r = UnityEngine.Random.Range(0, 100); // 0~99
        var table = (how == 6) ? TableHow6 : TableHow1;
        foreach (var (rate, cum) in table)
            if (r < cum) return rate;
        return table[^1].rate;
    }

    public GameObject UseRandom(int how)
    {
        int rate = RollRate(how);
        // 2) 후보 확보(없으면 인접 희귀도로 폴백)
        List<Itm> candidates = null;
        if (!itemsByRate.TryGetValue(rate, out candidates) || candidates.Count == 0)
        {
            // 상향→하향 폴백
            for (int up = rate + 1; up <= 5 && (candidates == null || candidates.Count == 0); up++)
                itemsByRate.TryGetValue(up, out candidates);
            for (int down = rate - 1; (candidates == null || candidates.Count == 0) && down >= 0; down--)
                itemsByRate.TryGetValue(down, out candidates);

            if (candidates == null || candidates.Count == 0)
            {
                // 완전 폴백(전체에서 아무거나)
                if (allItems.Count == 0) return gameObject;
                candidates = allItems;
            }
        }
        // 3) 최종 아이템 선택
        var get_ = candidates[UnityEngine.Random.Range(0, candidates.Count)];

        // 4) 코어 아이콘 매핑(널 대비)
        var core = Array.Find(DataManager.Instance.muscleItem, x => x.name == get_.name);
        if (coreimg != null) coreimg.sprite = core != null ? core : coreimg.sprite;
        Debug.Log(coreimg.name);
        // 5) 희귀도별 UI 세팅(배경, 가격 아이콘, 수량)
        //    rate→배경
        if (selfBg != null && rate >= 0 && rate < BackgroundByTier.Length)
            selfBg.sprite = BackgroundByTier[Math.Min(rate, BackgroundByTier.Length - 1)];

        //    rate→가격 재료 풀 + 수량 문자열
        var (pool, qtyText) = rate switch
        {
            0 => (blue, "x1"),
            1 => (green, "x1"),
            2 => (yellow, "x2"),
            3 => (red, "x3"),
            4 => (red, "x4"),
            5 => (red, "x5"),
            _ => (blue, "x1"),
        };

        if (pool != null && pool.Length > 0)
        {
            int idx = UnityEngine.Random.Range(0, pool.Length);
            var icon = pool[idx]?.GetComponent<Image>()?.sprite;
            if (ITEMPRICE != null && icon != null)
                ITEMPRICE.sprite = icon;
        }

        if (priceText != null) priceText.text = qtyText;

        return gameObject;
    }
}
