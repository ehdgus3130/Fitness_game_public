using System.Collections.Generic;

public enum ItemListLoadStatus
{
    MissingFile,
    Empty,
    Loaded,
    ParseError
}

public interface IItemStore
{
    ItemListLoadStatus LoadList(string fileName, List<ItemData> target, bool clearTarget = false);
    void SaveList(string fileName, List<ItemData> src);
    string EmptyListJson();
    List<ItemData> ParseFromResourceJson(string json);
}

public interface IPlayerStore
{
    void Save(string fileName, List<PlayerData> players);
    List<PlayerData> Load(string fileName, out bool usedLegacy);
}
