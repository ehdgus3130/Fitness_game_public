using System.Collections.Generic;
using UnityEngine.UI;

public interface IPlayerRuntimeUiPort
{
    bool TryGetEquipmentSlots(out List<ItemData> slot1, out List<ItemData> slot2, out List<ItemData> slot3);
    void EnsureDefaultItemInSlot1();
    bool TryGetEquipmentBonus(string key, out float bonus);
    void SetLevelUpFill(float exp, float max, int fillIndex);
}

public sealed class UnityPlayerRuntimeUiPort : IPlayerRuntimeUiPort
{
    public bool TryGetEquipmentSlots(out List<ItemData> slot1, out List<ItemData> slot2, out List<ItemData> slot3)
    {
        slot1 = null;
        slot2 = null;
        slot3 = null;

        var equipment = EquipmentScreen.Instance;
        if (equipment == null) return false;

        slot1 = equipment.ItmSlot1;
        slot2 = equipment.ItmSlot2;
        slot3 = equipment.ItmSlot3;
        return true;
    }

    public void EnsureDefaultItemInSlot1()
    {
        var equipment = EquipmentScreen.Instance;
        if (equipment == null) return;
        equipment.GetItem("스트랩", true);
    }

    public bool TryGetEquipmentBonus(string key, out float bonus)
    {
        bonus = 0f;
        var equipment = EquipmentScreen.Instance;
        if (equipment == null || equipment.EXPS_ == null) return false;
        return equipment.EXPS_.TryGetValue(key, out bonus);
    }

    public void SetLevelUpFill(float exp, float max, int fillIndex)
    {
        var levelUp = LevelUpScreen.Instance;
        if (levelUp == null || levelUp.fills == null) return;
        if (fillIndex < 0 || fillIndex >= levelUp.fills.Length) return;

        Image fill = levelUp.fills[fillIndex];
        if (fill == null) return;
        levelUp.Set_FillAmount(exp, max, fill);
    }
}

public static class PlayerRuntimeUiBridge
{
    private static IPlayerRuntimeUiPort _port;
    public static IPlayerRuntimeUiPort Port
    {
        get => _port ??= new UnityPlayerRuntimeUiPort();
        set => _port = value ?? new UnityPlayerRuntimeUiPort();
    }

    public static void ResetPort() => _port = null;

    public static bool TryGetEquipmentSlots(out List<ItemData> slot1, out List<ItemData> slot2, out List<ItemData> slot3)
        => Port.TryGetEquipmentSlots(out slot1, out slot2, out slot3);

    public static bool TryGetEquipmentSlot(int slotIndex, out List<ItemData> slot)
    {
        slot = null;
        if (!TryGetEquipmentSlots(out var slot1, out var slot2, out var slot3)) return false;

        switch (slotIndex)
        {
            case 1: slot = slot1; return true;
            case 2: slot = slot2; return true;
            case 3: slot = slot3; return true;
            default: return false;
        }
    }

    public static void EnsureDefaultItemInSlot1() => Port.EnsureDefaultItemInSlot1();

    public static bool TryGetEquipmentBonus(string key, out float bonus)
        => Port.TryGetEquipmentBonus(key, out bonus);

    public static void SetLevelUpFill(float exp, float max, int fillIndex)
        => Port.SetLevelUpFill(exp, max, fillIndex);
}
