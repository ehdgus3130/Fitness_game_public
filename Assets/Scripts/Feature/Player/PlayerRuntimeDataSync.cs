using UnityEngine;

public static class PlayerRuntimeDataSync
{
    public static void LoadEquippedItems(IItemStore itemRepository, string slot1File, string slot2File, string slot3File)
    {
        if (itemRepository == null) return;
        if (!PlayerRuntimeUiBridge.TryGetEquipmentSlots(out var slot1, out var slot2, out var slot3)) return;

        var s1 = itemRepository.LoadList(slot1File, slot1, clearTarget: true);
        if (s1 == ItemListLoadStatus.Empty || slot1.Count == 0)
        {
            PlayerRuntimeUiBridge.EnsureDefaultItemInSlot1();
        }

        itemRepository.LoadList(slot2File, slot2, clearTarget: true);
        itemRepository.LoadList(slot3File, slot3, clearTarget: true);
    }

    public static void ClearEquipmentAndPersistEmpty(
        JsonFileStore fileStore,
        IItemStore itemRepository,
        string slot1File,
        string slot2File,
        string slot3File)
    {
        if (PlayerRuntimeUiBridge.TryGetEquipmentSlots(out var slot1, out var slot2, out var slot3))
        {
            slot1.Clear();
            slot2.Clear();
            slot3.Clear();
        }

        if (fileStore == null || itemRepository == null) return;

        var empty = itemRepository.EmptyListJson();
        fileStore.Write(slot1File, empty);
        fileStore.Write(slot2File, empty);
        fileStore.Write(slot3File, empty);
    }

    public static void SaveEquippedItemSlot(IItemStore itemRepository, string fileName, int slotIndex)
    {
        if (itemRepository == null) return;
        if (!PlayerRuntimeUiBridge.TryGetEquipmentSlot(slotIndex, out var slot)) return;
        itemRepository.SaveList(fileName, slot);
    }

    public static void ApplyLevelUp(PlayerData player, GameObject target, float exp)
    {
        if (player == null || target == null) return;

        var n = target.name;
        float bonus;

        if (n.Contains("Shoulder"))
        {
            if (PlayerRuntimeUiBridge.TryGetEquipmentBonus("ShoulderEXP", out bonus)) exp += bonus;
            player.ShoulderEXP = exp;
            PlayerRuntimeUiBridge.SetLevelUpFill(player.ShoulderEXP, player.ShoulderMax, 5);
        }
        else if (n.Contains("Chest"))
        {
            if (PlayerRuntimeUiBridge.TryGetEquipmentBonus("ChestEXP", out bonus)) exp += bonus;
            player.ChestEXP = exp;
            PlayerRuntimeUiBridge.SetLevelUpFill(player.ChestEXP, player.ChestMax, 3);
        }
        else if (n.Contains("Arm"))
        {
            if (PlayerRuntimeUiBridge.TryGetEquipmentBonus("ArmEXP", out bonus)) exp += bonus;
            player.ArmEXP = exp;
            PlayerRuntimeUiBridge.SetLevelUpFill(player.ArmEXP, player.ArmMax, 1);
        }
        else if (n.Contains("Abs"))
        {
            if (PlayerRuntimeUiBridge.TryGetEquipmentBonus("AbsEXP", out bonus)) exp += bonus;
            player.AbsEXP = exp;
            PlayerRuntimeUiBridge.SetLevelUpFill(player.AbsEXP, player.AbsMax, 0);
        }
        else if (n.Contains("Back"))
        {
            if (PlayerRuntimeUiBridge.TryGetEquipmentBonus("BackEXP", out bonus)) exp += bonus;
            player.BackEXP = exp;
            PlayerRuntimeUiBridge.SetLevelUpFill(player.BackEXP, player.BackMax, 2);
        }
        else if (n.Contains("Leg"))
        {
            if (PlayerRuntimeUiBridge.TryGetEquipmentBonus("LegEXP", out bonus)) exp += bonus;
            player.LegEXP = exp;
            PlayerRuntimeUiBridge.SetLevelUpFill(player.LegEXP, player.LegMax, 4);
        }
    }
}
