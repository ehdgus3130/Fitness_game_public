using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public sealed class ItemRepository : IItemStore
{
    [Serializable]
    private class ItemListWrap
    {
        public List<ItemData> Items;
    }

    [Serializable]
    private class ItemList
    {
        public List<ItemData> Items;
    }

    private readonly JsonFileStore _store;

    public ItemRepository(JsonFileStore store)
    {
        _store = store;
    }

    public ItemListLoadStatus LoadList(string fileName, List<ItemData> target, bool clearTarget = false)
    {
        if (clearTarget) target.Clear();
        if (!_store.Exists(fileName)) return ItemListLoadStatus.MissingFile;

        var json = _store.Read(fileName);
        if (string.IsNullOrWhiteSpace(json)) return ItemListLoadStatus.Empty;

        try
        {
            var s = json.TrimStart();
            if (s.StartsWith("["))
            {
                var arr = JsonConvert.DeserializeObject<List<ItemData>>(json) ?? new List<ItemData>();
                if (arr.Count == 0) return ItemListLoadStatus.Empty;
                target.AddRange(arr);
                return ItemListLoadStatus.Loaded;
            }

            var w = JsonUtility.FromJson<ItemListWrap>(json);
            var list = w?.Items ?? new List<ItemData>();
            if (list.Count == 0) return ItemListLoadStatus.Empty;
            target.AddRange(list);
            return ItemListLoadStatus.Loaded;
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadItemList parse error: {fileName}\n{e}\njson: {json}");
            return ItemListLoadStatus.ParseError;
        }
    }

    public void SaveList(string fileName, List<ItemData> src)
    {
        var json = JsonUtility.ToJson(new ItemListWrap { Items = src }, true);
        _store.Write(fileName, json);
    }

    public string EmptyListJson() => JsonUtility.ToJson(new ItemListWrap { Items = new List<ItemData>() });

    public List<ItemData> ParseFromResourceJson(string json)
    {
        var parsed = JsonUtility.FromJson<ItemList>(json)?.Items;
        return parsed ?? new List<ItemData>();
    }
}
