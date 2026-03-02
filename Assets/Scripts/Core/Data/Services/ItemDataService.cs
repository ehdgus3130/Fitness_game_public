using System.Collections.Generic;

public sealed class ItemDataService
{
    private readonly Dictionary<string, ItemData> _itemByName = new Dictionary<string, ItemData>();

    public void RebuildIndex(List<ItemData> items)
    {
        _itemByName.Clear();
        if (items == null) return;

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.name)) continue;
            _itemByName[item.name] = item;
        }
    }

    public bool TryGetByName(List<ItemData> items, string itemName, out ItemData item)
    {
        item = null;
        if (string.IsNullOrEmpty(itemName)) return false;

        if (_itemByName.TryGetValue(itemName, out item) && item != null)
        {
            return true;
        }

        item = items?.Find(x => x != null && x.name == itemName);
        if (item == null) return false;

        _itemByName[itemName] = item;
        return true;
    }
}
