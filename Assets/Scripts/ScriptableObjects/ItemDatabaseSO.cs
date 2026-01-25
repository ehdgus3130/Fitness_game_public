using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 아이템 정의들의 데이터베이스 ScriptableObject
/// 아이템 정의들을 리스트로 보유하며, ID 및 이름 기반 조회 기능을 제공
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Item Database", fileName = "ItemDatabase")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemDefinitionSO> items = new List<ItemDefinitionSO>();

    // 런타임 캐시(조회 O(1))
    private Dictionary<string, ItemDefinitionSO> _byId;
    private Dictionary<string, ItemDefinitionSO> _byName;

    public void RebuildIndex() // 인덱스 재구축
    {
        _byId = new Dictionary<string, ItemDefinitionSO>();
        _byName = new Dictionary<string, ItemDefinitionSO>();

        if (items == null) return;
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            if (it == null) continue;

            if (!string.IsNullOrEmpty(it.id)) _byId[it.id] = it;
            if (!string.IsNullOrEmpty(it.displayName)) _byName[it.displayName] = it;
        }
    }

    public bool TryGetById(string id, out ItemDefinitionSO item) // ID로 조회
    {
        item = null;
        if (string.IsNullOrEmpty(id)) return false;

        if (_byId == null) { RebuildIndex(); return false; }

        return _byId.TryGetValue(id, out item) && item != null;
    }

    // 기존 세이브가 "아이템 이름"을 저장하는 경우 호환용
    public bool TryGetByName(string displayName, out ItemDefinitionSO item) // 이름으로 조회
    {
        item = null;
        if (string.IsNullOrEmpty(displayName)) return false;

        if (_byName == null) { RebuildIndex(); return false; }

        return _byName.TryGetValue(displayName, out item) && item != null;
    }
}
