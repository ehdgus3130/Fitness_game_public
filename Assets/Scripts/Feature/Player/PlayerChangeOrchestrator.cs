using UnityEngine;

public interface IPlayerChangeRuntimePort
{
    void RespawnPlayers();
    void SyncEquipment(DataManager data, PlayerData currentPlayer, string curPlayerId);
    void RefreshLevelUi(DataManager data);
}

public sealed class UnityPlayerChangeRuntimePort : IPlayerChangeRuntimePort
{
    public void RespawnPlayers()
    {
        var players = Object.FindObjectsOfType<PlayerController>();
        foreach (var player in players)
        {
            Object.Destroy(player.gameObject);
        }

        var queue = RoutineQueueManager.Instance;
        if (queue == null || queue.Screen == null) return;

        Object.Instantiate(queue.P2, queue.Screen);
        Object.Instantiate(queue.P3, queue.Screen);
        Object.Instantiate(queue.P1, queue.Screen);
    }

    public void SyncEquipment(DataManager data, PlayerData currentPlayer, string curPlayerId)
    {
        var equipment = EquipmentScreen.Instance;
        if (equipment == null) return;

        if (!data.TryFindItemByName(currentPlayer.Item1, out var item1))
            equipment.OnDeleteClick(equipment.Item1_);
        else
            equipment.Equip_Item(item1, false);

        if (!data.TryFindItemByName(currentPlayer.Item2, out var item2))
            equipment.OnDeleteClick(equipment.Item2_);
        else
            equipment.Equip_Item(item2, false);

        if (!data.TryFindItemByName(currentPlayer.Item3, out var item3))
            equipment.OnDeleteClick(equipment.Item3_);
        else
            equipment.Equip_Item(item3, false);

        equipment.GetItem(curPlayerId, false);
    }

    public void RefreshLevelUi(DataManager data)
    {
        var levelUp = LevelUpScreen.Instance;
        if (levelUp == null) return;

        levelUp.reset_();

        var queue = RoutineQueueManager.Instance;
        if (queue?.Lv1 == null) return;

        for (int i = 0; i < 6 && i < queue.Lv1.Length; i++)
        {
            data.LevelUp(queue.Lv1[i], 0);
        }
    }
}

public class PlayerChangeOrchestrator : Singleton<PlayerChangeOrchestrator>
{
    private static IPlayerChangeRuntimePort _runtimePort;
    public static IPlayerChangeRuntimePort RuntimePort
    {
        get => _runtimePort ??= new UnityPlayerChangeRuntimePort();
        set => _runtimePort = value ?? new UnityPlayerChangeRuntimePort();
    }

    public static void ResetRuntimePort() => _runtimePort = null;

    public void ApplyAfterPlayerSwitch()
    {
        var data = DataManager.Instance;
        if (data == null) return;
        if (!data.TryGetCurrentPlayer(out var currentPlayer)) return;
        var curPlayerId = data.GetCurrentPlayerId();

        RuntimePort.RespawnPlayers();
        RuntimePort.SyncEquipment(data, currentPlayer, curPlayerId);
        RuntimePort.RefreshLevelUi(data);
    }
}
