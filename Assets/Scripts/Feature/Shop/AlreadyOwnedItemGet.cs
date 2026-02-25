using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlreadyOwnedItemGet : MonoBehaviour
{
    public Image itmCore;
    [SerializeField] private TMP_Text text_Item;
    [SerializeField] private Transform[] EXPPos;
    private Image[][] EXPS = new Image[8][];

    [SerializeField] private Sprite[] Bar = new Sprite[8];

    private Dictionary<string, Sprite> _spriteByName;
    private Dictionary<string, ItemData> _itemByName;

    private enum StatVar
    {
        Ene, Fat, Abs, Arm, Back, Chest, Leg, Shoulder, Time, Unknown
    }
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
                if (EXPPos[i].GetChild(j).TryGetComponent<Image>(out var img))
                    EXPS[i][j] = img;
                else
                    Debug.LogError($"Row {i} Col {j} has no Image component");
            }
        }

        _spriteByName = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        foreach (var s in EquipmentScreen.Instance.muscleItem)
            if (s != null && !_spriteByName.ContainsKey(s.name)) _spriteByName.Add(s.name, s);

        _itemByName = new Dictionary<string, ItemData>(StringComparer.Ordinal);
        foreach (var itm in DataManager.Instance.Items)
            if (itm != null && !_itemByName.ContainsKey(itm.name)) _itemByName.Add(itm.name, itm);

    }
    public void SetName(string name)
    {
        var newName = name.EndsWith("(Clone)", StringComparison.Ordinal)
            ? name.Substring(0, name.Length - "(Clone)".Length)
            : name;

        if (_spriteByName != null && _spriteByName.TryGetValue(newName, out var spr))
            itmCore.sprite = spr;
        else
            Debug.LogWarning($"Sprite not found for {newName}");

        text_Item.text = newName;

        if (_itemByName == null || !_itemByName.TryGetValue(newName, out var equip_))
        {
            // 모르는 경우 EXP 전부 채움
            for (int i = 2; i < 8; i++) ApplyBars(i, Level.All);
            return;
        }

        var stat = _varMap.TryGetValue(equip_.var, out var v) ? v : StatVar.Unknown;
        switch (stat)
        {
            case StatVar.Ene: ApplyByValue(equip_.effect, 0, isExpType: false); break;
            case StatVar.Fat: ApplyByValue(equip_.effect, 1, isExpType: false); break;
            case StatVar.Abs: ApplyByValue(equip_.effect, 2, isExpType: true); break;
            case StatVar.Arm: ApplyByValue(equip_.effect, 3, isExpType: true); break;
            case StatVar.Back: ApplyByValue(equip_.effect, 4, isExpType: true); break;
            case StatVar.Chest: ApplyByValue(equip_.effect, 5, isExpType: true); break;
            case StatVar.Leg: ApplyByValue(equip_.effect, 6, isExpType: true); break;
            case StatVar.Shoulder: ApplyByValue(equip_.effect, 7, isExpType: true); break;
            case StatVar.Time:
                Debug.Log("time is coming");
                break;
            default:
                for (int i = 2; i < 8; i++) ApplyBars(i, Level.All);
                break;
        }
    }
    private void ApplyByValue(int num, int row, bool isExpType)
    {
        int normalized = isExpType ? num / 10 : num; // 10->1, 30->3, ...
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

    private enum Level { L1, L3, L5, L7, All }

    private void ApplyBars(int row, Level lv)
    {
        if (EXPS == null || EXPS[row] == null) return;

        // 초기화(필요 시 null 가능)
        for (int c = 0; c < 4; c++) EXPS[row][c].sprite = null;

        switch (lv)
        {
            case Level.L1: // EXPS[row][3] = Bar[0]
                EXPS[row][3].sprite = Bar[0];
                break;

            case Level.L3: // [3]=Bar0
                EXPS[row][3].sprite = Bar[0];
                break;

            case Level.L5: // [3]=Bar1, [2]=Bar2
                EXPS[row][3].sprite = Bar[1];
                EXPS[row][2].sprite = Bar[2];
                break;

            case Level.L7: // [3]=Bar3, [2]=Bar4, [1]=Bar4
                EXPS[row][3].sprite = Bar[3];
                EXPS[row][2].sprite = Bar[4];
                EXPS[row][1].sprite = Bar[4];
                break;

            case Level.All: // [3]=Bar5,[2]=Bar6,[1]=Bar6,[0]=Bar7
                EXPS[row][3].sprite = Bar[5];
                EXPS[row][2].sprite = Bar[6];
                EXPS[row][1].sprite = Bar[6];
                EXPS[row][0].sprite = Bar[7];
                break;
        }
    }
}
