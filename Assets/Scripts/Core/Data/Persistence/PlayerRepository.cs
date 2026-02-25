using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public sealed class PlayerRepository
{
    private readonly JsonFileStore _store;

    public PlayerRepository(JsonFileStore store)
    {
        _store = store;
    }

    public void Save(string fileName, List<PlayerData> players)
    {
        var save = new List<PlayerSaveData>(players?.Count ?? 0);
        if (players != null)
        {
            foreach (var p in players)
            {
                save.Add(p.ToSaveData());
            }
        }

        var json = JsonConvert.SerializeObject(save, Formatting.Indented);
        _store.Write(fileName, json);
    }

    public List<PlayerData> Load(string fileName, out bool usedLegacy)
    {
        usedLegacy = false;
        if (!_store.Exists(fileName)) return new List<PlayerData>();

        var json = _store.Read(fileName);
        if (string.IsNullOrWhiteSpace(json)) return new List<PlayerData>();

        try
        {
            var saveList = JsonConvert.DeserializeObject<List<PlayerSaveData>>(json);
            if (saveList != null && saveList.Count > 0)
            {
                var players = new List<PlayerData>(saveList.Count);
                foreach (var d in saveList)
                {
                    if (d == null) continue;
                    players.Add(PlayerFromSaveData(d));
                }
                return players;
            }
        }
        catch
        {
            // legacy fallback below
        }

        try
        {
            usedLegacy = true;
            return JsonConvert.DeserializeObject<List<PlayerData>>(json) ?? new List<PlayerData>();
        }
        catch (Exception e)
        {
            Debug.LogError($"DeserializePlayers failed.\n{e}\njson: {json}");
            return new List<PlayerData>();
        }
    }

    private static PlayerData PlayerFromSaveData(PlayerSaveData d)
    {
        var lvs = (d.lvs != null && d.lvs.Length >= 6) ? d.lvs : new float[6];

        var p = new PlayerData(
            d.name ?? "",
            d.energy.ToString(),
            d.fatigue.ToString(),
            d.dayLv.ToString(),
            d.item1 ?? "",
            d.item2 ?? "",
            d.item3 ?? "",
            lvs[5], lvs[3], lvs[1], lvs[0], lvs[2], lvs[4]
        );

        p.LoadFromSaveData(d, refreshUI: false);
        return p;
    }
}
