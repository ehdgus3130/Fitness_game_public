#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Item_Info.json 파일을 읽어 ItemDefinitionSO 및 ItemDatabaseSO로 변환하는 에디터 유틸리티
/// JSON 파일은 Resources/Json/Item_Info.json 경로에 위치해야 함
/// 아이템 정의 에셋은 Resources/DB/Items 폴더에 생성되며,
/// 아이템 데이터베이스 에셋은 Resources/DB/ItemDatabase.asset 경로에 생성됨
/// </summary>
public static class ItemDatabaseImporter
{
    [Serializable]
    private class ItemJson
    {
        public string name;
        public string var;
        public int effect;
        public int rate;
        public string explain;
    }

    [Serializable] // 아이템 JSON 리스트 래퍼
    private class ItemJsonList
    {
        public List<ItemJson> Items;
    }

    [MenuItem("Tools/Game Data/Import Item_Info.json/Resources.Json")]
    public static void Import()
    {
        var ta = Resources.Load<TextAsset>("Json/Item_Info");
        if (ta == null)
        {
            EditorUtility.DisplayDialog("Import Failed", "Resources/Json/Item_Info.json 을 찾지 못했습니다.", "OK");
            return;
        }

        ItemJsonList parsed;
        try
        {
            parsed = JsonUtility.FromJson<ItemJsonList>(ta.text);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Import Failed", $"JSON 파싱 실패\n{e}", "OK");
            return;
        }

        if (parsed == null || parsed.Items == null || parsed.Items.Count == 0)
        {
            EditorUtility.DisplayDialog("Import Failed", "Items 배열이 비어있습니다.", "OK");
            return;
        }

        // 폴더 생성
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/DB");
        EnsureFolder("Assets/Resources/DB/Items");

        // 기존 DB 로드 또는 생성
        const string dbPath = "Assets/Resources/DB/ItemDatabase.asset";
        var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(dbPath);
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<ItemDatabaseSO>();
            AssetDatabase.CreateAsset(db, dbPath);
        }

        db.items = new List<ItemDefinitionSO>();

        for (int i = 0; i < parsed.Items.Count; i++)
        {
            var src = parsed.Items[i];
            if (src == null) continue;

            var assetName = SanitizeFileName(src.name);
            var itemPath = $"Assets/Resources/DB/Items/{assetName}.asset";

            var item = AssetDatabase.LoadAssetAtPath<ItemDefinitionSO>(itemPath);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemDefinitionSO>();
                AssetDatabase.CreateAsset(item, itemPath);
            }

            item.displayName = src.name;
            item.id = string.IsNullOrEmpty(item.id) ? src.name : item.id; // 기본은 name
            item.varKey = src.var;
            item.effect = src.effect;
            item.rate = src.rate;
            item.explain = src.explain;

            EditorUtility.SetDirty(item); // 변경 사항 저장
            db.items.Add(item);
        }

        EditorUtility.SetDirty(db); // DB 변경 사항 저장
        AssetDatabase.SaveAssets(); // 에셋 저장
        AssetDatabase.Refresh(); // 에셋 데이터베이스 갱신

        EditorUtility.DisplayDialog("Import Complete", $"{db.items.Count}개 아이템을 ItemDatabaseSO로 변환했습니다.\n(DB: Resources/DB/ItemDatabase)", "OK");
        // 인덱스 재구축
    }

    /// <summary>
    /// 폴더가 없으면 생성
    /// </summary>
    /// <param name="path"> 생성할 폴더 경로 </param>
    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        var parent = Path.GetDirectoryName(path).Replace("\\", "/");
        var name = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    /// <summary>
    /// 파일 이름으로 사용할 수 없는 문자 제거
    /// </summary>
    /// <returns></returns>
    private static string SanitizeFileName(string s)
    {
        if (string.IsNullOrEmpty(s)) return "Item";

        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c.ToString(), "_");

        // 에셋 이름이 너무 길면 Unity/OS에서 문제가 날 수 있어 적당히 컷
        if (s.Length > 64) s = s.Substring(0, 64);
        return s;
    }
}
#endif
