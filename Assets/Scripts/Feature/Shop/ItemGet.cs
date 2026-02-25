using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGet : Singleton<ItemGet>
{
    [Header("UI")]
    public Image itmCore;
    public TMP_Text text_Item;
    public Transform[] EXPPos;
    private Image[][] EXPS = new Image[8][];

    public Sprite[] Bar = new Sprite[8];
    private Dictionary<string, Sprite> _spriteByName;
    private Dictionary<string, ItemData> _itemByName;
    private enum StatVar { Ene, Fat, Abs, Arm, Back, Chest, Leg, Shoulder, Time, Unknown }
    private static readonly Dictionary<string, StatVar> _varMap = new(StringComparer.Ordinal)
    {
        ["ene"] = StatVar.Ene,
        ["fat"] = StatVar.Fat,
        ["AbsEXP"] = StatVar.Abs,
        ["ArmEXP"] = StatVar.Arm,
        ["BackEXP"] = StatVar.Back,
        ["ChestEXP"] = StatVar.Chest,
        ["LegEXP"] = StatVar.Leg,
        ["ShoulderEXP"] = StatVar.Shoulder,
        ["time"] = StatVar.Time
    };
    void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            EXPS[i] = new Image[4];
            for (int j = 0; j < 4; j++)
            {
                if (!EXPPos[i].GetChild(j).TryGetComponent<Image>(out var img))
                    Debug.LogError($"Row {i} Col {j} has no Image component.");
                EXPS[i][j] = img;
            }
        }
        _spriteByName = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        foreach (var s in EquipmentScreen.Instance.muscleItem)
            if (s != null && !_spriteByName.ContainsKey(s.name)) _spriteByName.Add(s.name, s);

        _itemByName = new Dictionary<string, ItemData>(StringComparer.Ordinal);
        foreach (var it in DataManager.Instance.Items)
            if (it != null && !_itemByName.ContainsKey(it.name)) _itemByName.Add(it.name, it);

    }
    public void SetName(string rawName)
    {
        string name = rawName.EndsWith("(Clone)", StringComparison.Ordinal)
                    ? rawName.AsSpan(0, rawName.Length - "(Clone)".Length).ToString()
                    : rawName;

        // 스프라이트/텍스트 세팅 (선형탐색 → 딕셔너리)
        if (_spriteByName != null && _spriteByName.TryGetValue(name, out var spr))
            itmCore.sprite = spr;
        else
            Debug.LogWarning($"Sprite not found: {name}");
        if (text_Item != null) text_Item.text = name;

        // 아이템 조회
        if (_itemByName == null || !_itemByName.TryGetValue(name, out var equip_) || equip_ == null)
        {
            // 모르는 경우 기존 default 동작 유지: EXP 전부 채움
            for (int i = 2; i < 8; i++) ApplyBars(i, Level.All);
            return;
        }

        // 효과 반영: ene/fat(1/3/5/7) vs EXP류(10/30/50/70) → 공통 레벨로 정규화
        var stat = _varMap.TryGetValue(equip_.var, out var v) ? v : StatVar.Unknown;
        switch (stat)
        {
            case StatVar.Ene: ApplyByValue(equip_.effect, 0, false); break;
            case StatVar.Fat: ApplyByValue(equip_.effect, 1, false); break;
            case StatVar.Abs: ApplyByValue(equip_.effect, 2, true); break;
            case StatVar.Arm: ApplyByValue(equip_.effect, 3, true); break;
            case StatVar.Back: ApplyByValue(equip_.effect, 4, true); break;
            case StatVar.Chest: ApplyByValue(equip_.effect, 5, true); break;
            case StatVar.Leg: ApplyByValue(equip_.effect, 6, true); break;
            case StatVar.Shoulder: ApplyByValue(equip_.effect, 7, true); break;

            case StatVar.Time:
                Debug.Log("time is coming");
                break;

            default:
                for (int i = 2; i < 8; i++) ApplyBars(i, Level.All);
                break;
        }
    }
    private enum Level { L1, L3, L5, L7, All }

    private void ApplyByValue(int num, int row, bool isExpType)
    {
        int normalized = isExpType ? num / 10 : num; // 10→1, 30→3, ...
        Level lv = normalized switch
        {
            1 => Level.L1,
            3 => Level.L3,
            5 => Level.L5,
            7 => Level.L7,
            _ => Level.All
        };
        ApplyBars(row, lv);
    }

    private void ApplyBars(int row, Level lv)
    {
        if (EXPS == null || row < 0 || row >= EXPS.Length || EXPS[row] == null || EXPS[row].Length < 4)
            return;

        // 먼저 행 초기화(잔상 제거)
        for (int c = 0; c < 4; c++) if (EXPS[row][c] != null) EXPS[row][c].sprite = null;

        switch (lv)
        {
            case Level.L1:
                EXPS[row][3].sprite = Bar[0];
                break;
            case Level.L3:
                EXPS[row][3].sprite = Bar[0];
                break;
            case Level.L5:
                EXPS[row][3].sprite = Bar[1];
                EXPS[row][2].sprite = Bar[2];
                break;
            case Level.L7:
                EXPS[row][3].sprite = Bar[3];
                EXPS[row][2].sprite = Bar[4];
                EXPS[row][1].sprite = Bar[4];
                break;
            case Level.All:
                EXPS[row][3].sprite = Bar[5];
                EXPS[row][2].sprite = Bar[6];
                EXPS[row][1].sprite = Bar[6];
                EXPS[row][0].sprite = Bar[7];
                break;
        }
    }

}

