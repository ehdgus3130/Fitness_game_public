using UnityEngine;
/// <summary>
/// 아이템 정의 ScriptableObject
/// 아이템의 속성들을 보유하며, ItemData 구조체로 변환하는 기능을 제공
/// </summary>

[CreateAssetMenu(menuName = "Game Data/Item Definition", fileName = "ItemDefinition")]
public class ItemDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("저장/참조용 불변 ID. 기존 세이브 호환을 위해 displayName과 동일하게 두어도 됩니다.")]
    public string id;

    [Header("Presentation")]
    [Tooltip("게임 내 표기 이름(현재 Item_Info.json의 name)")]
    public string displayName;

    [TextArea]
    public string explain;

    public Sprite icon;

    [Header("Effect")]
    [Tooltip("적용 대상 변수 키 (예: AbsEXP, ArmEXP, ene, fat, time, ALL_EXP)")]
    public string varKey;

    [Tooltip("효과 수치")]
    public int effect;

    [Tooltip("레어도/등급")]
    public int rate;

    public ItemData ToItemData() // ItemData 변환
    {
        var n = string.IsNullOrEmpty(displayName) ? name : displayName;
        var v = varKey ?? "";
        var ex = explain ?? "";
        return new ItemData(n, v, effect, rate, ex);
    }

#if UNITY_EDITOR // Editor 전용 코드
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(displayName)) displayName = name;
        if (string.IsNullOrEmpty(id)) id = displayName;
    }
#endif
}
