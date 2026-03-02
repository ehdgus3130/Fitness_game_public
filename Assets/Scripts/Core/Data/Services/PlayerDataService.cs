using System.Collections.Generic;

public sealed class PlayerDataService
{
    public List<PlayerData> CreateDefaultPlayers()
    {
        return new List<PlayerData>
        {
            new PlayerData("1", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0),
            new PlayerData("2", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0),
            new PlayerData("3", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0)
        };
    }

    public PlayerData EnsureCurrentPlayer(List<PlayerData> allPlayers, string curPlayer, out string resolvedCurPlayer)
    {
        resolvedCurPlayer = curPlayer;
        if (allPlayers == null || allPlayers.Count == 0) return null;

        if (string.IsNullOrEmpty(resolvedCurPlayer)) resolvedCurPlayer = "1";
        string playerIdToFind = resolvedCurPlayer;
        var target = allPlayers.Find(p => p.name == playerIdToFind);
        if (target != null) return target;

        resolvedCurPlayer = allPlayers[0].name;
        return allPlayers[0];
    }

    public bool TrySwitchPlayer(
        List<PlayerData> allPlayers,
        List<PlayerData> currentPlayers,
        string currentPlayerId,
        int newIdx,
        out string nextPlayerId,
        out PlayerData nextPlayer)
    {
        nextPlayerId = currentPlayerId;
        nextPlayer = null;

        if (allPlayers == null || allPlayers.Count == 0) return false;
        if (newIdx <= 0 || newIdx > allPlayers.Count) return false;

        if (int.TryParse(currentPlayerId, out var oldIdx) && oldIdx > 0 && oldIdx <= allPlayers.Count)
        {
            if (currentPlayers != null && currentPlayers.Count > 0)
            {
                allPlayers[oldIdx - 1] = currentPlayers[0];
            }
        }

        nextPlayer = allPlayers[newIdx - 1];
        if (nextPlayer == null) return false;

        nextPlayerId = newIdx.ToString();
        return true;
    }
}
