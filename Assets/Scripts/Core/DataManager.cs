using System.Collections.Generic;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
#endif

public class DataManager : Singleton<DataManager>    //Information
{

#if UNITY_EDITOR
    [ContextMenu("Debug/Open Persistent Folder")]
    private void Ctx_OpenPersist() => UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
#endif


    public TextAsset FormatUser_DataBase; //All User data
    public List<PlayerData> AllPlayer_Info, Player_Info;   //make into List 
    public List<ItemData> Items = new List<ItemData>();
    public ItemDatabaseSO itemDatabase;
    private readonly Dictionary<string, ItemData> _itemByName = new Dictionary<string, ItemData>();
    private JsonFileStore _fileStore;
    private ItemRepository _itemRepository;
    private PlayerRepository _playerRepository;

    private const string PlayerInfoFileName = "Player_Info.json";
    private const string AllPlayerInfoFileName = "AllPlayer_Info.json";
    private const string CurItemInfo1FileName = "CurItem_Info1.json";
    private const string CurItemInfo2FileName = "CurItem_Info2.json";
    private const string CurItemInfo3FileName = "CurItem_Info3.json";

    public Sprite[] muscleItem;
    public string CurPlayer;        //CUrrent PlayerData
    private string _PersistentDataPath;

    /// <summary>
    /// Dictionary 생성
    /// </summary>
    private void BuildItemIndex()
    {
        _itemByName.Clear();
        for (int i = 0; i < Items.Count; i++)
        {
            var it = Items[i];
            if (it == null) continue;
            if (string.IsNullOrEmpty(it.name)) continue;
            _itemByName[it.name] = it;
        }
    }

    private bool TryGetItemByName(string itemName, out ItemData item)
    {
        item = null;
        if (string.IsNullOrEmpty(itemName)) return false;

        if (_itemByName.TryGetValue(itemName, out item) && item != null) return true;

        // fallback (index 누락 방지)
        item = Items.Find(x => x != null && x.name == itemName);
        if (item != null)
        {
            _itemByName[itemName] = item;
            return true;
        }
        return false;
    }

    private void EnsureRepositories()
    {
        if (string.IsNullOrEmpty(_PersistentDataPath))
        {
            _PersistentDataPath = Application.persistentDataPath;
        }

        if (_fileStore == null)
        {
            _fileStore = new JsonFileStore(_PersistentDataPath);
        }

        if (_itemRepository == null)
        {
            _itemRepository = new ItemRepository(_fileStore);
        }

        if (_playerRepository == null)
        {
            _playerRepository = new PlayerRepository(_fileStore);
        }
    }

    public bool IsInitialized { get; private set; }
    public bool ItemsLoaded { get; private set; }
    void Start() //private IEnumerator
    {   //전체 플레이어 리스트 불러오기
        _PersistentDataPath = Application.persistentDataPath; //edit path
        EnsureRepositories();

#if UNITY_EDITOR
        Debug.Log("디버깅용 전체 포멧 진행");
        FormatAllPlayerData();
#endif

        Player_Info = new List<PlayerData>();
        AllPlayer_Info = new List<PlayerData>();


        Debug.Log("아이템 만들기 전");

        Items.Clear();
        // check ItemDatabaseSO first
        if (itemDatabase == null) itemDatabase = Resources.Load<ItemDatabaseSO>("DB/ItemDatabase");

        // Load items from ItemDatabaseSO if available
        if (itemDatabase != null && itemDatabase.items != null && itemDatabase.items.Count > 0)
        {
            for (int i = 0; i < itemDatabase.items.Count; i++)
            {
                var def = itemDatabase.items[i];
                if (def == null) continue;
                Items.Add(def.ToItemData());
            }
        }
        else // Fallback to JSON file
        {
            var itemTa = Resources.Load<TextAsset>("Json/Item_Info");
            if (itemTa != null)
            {
                var parsedItems = _itemRepository.ParseFromResourceJson(itemTa.text);
                if (parsedItems != null)
                    foreach (var it in parsedItems)
                        Items.Add(new ItemData(it.name, it.var, it.effect, it.rate, it.explain));
            }
            else
            {
                Debug.LogWarning("Item_Info.json not found in Resources/Json and ItemDatabaseSO not assigned.");
            }
        }

        BuildItemIndex(); // create dictionary for item lookup

        // Load muscle item sprites
        muscleItem = Resources.LoadAll<Sprite>("Images/Items");
        ItemsLoaded = true;
        Debug.Log("아이템 만들기 후");



        //Call All_player Info
        var allTa = Resources.Load<TextAsset>("Json/AllPlayer_Info");
        if (_fileStore.Exists(AllPlayerInfoFileName)) //check file exist
        {
            LoadAllPlayerDataFromJson();
        }
        else if (allTa != null) //check Resources
        {
            _fileStore.Write(AllPlayerInfoFileName, allTa.text);
            LoadAllPlayerDataFromJson();
        }
        else //finally, create default
        {
            //default data
            AllPlayer_Info = new List<PlayerData> {
        new PlayerData("1","100","0","1","","","",0,0,0,0,0,0),
        new PlayerData("2","100","0","1","","","",0,0,0,0,0,0),
        new PlayerData("3","100","0","1","","","",0,0,0,0,0,0),
    };
            SavePlayerDataToAllPlayerJson();
        }


        //Call Main_Player Info
        if (_fileStore.Exists(PlayerInfoFileName)) //check file exist
        {
            LoadPlayerDataFromJson();
        }
        else //If not, create new file from AllPlayer
        {
            //Set Current PlayerData to Player_Info
            if (string.IsNullOrEmpty(CurPlayer)) CurPlayer = "1";
            var target = AllPlayer_Info.Find(p => p.name == CurPlayer) ?? AllPlayer_Info[0];
            Player_Info = new List<PlayerData> { target };
            SavePlayerDataToJson();
        }


        //Check itemEquipment
        var s1 = _itemRepository.LoadList(CurItemInfo1FileName, EquipmentScreen.Instance.ItmSlot1, clearTarget: true);
        if (s1 == ItemRepository.LoadStatus.Empty || EquipmentScreen.Instance.ItmSlot1.Count == 0)
        {
            EquipmentScreen.Instance.GetItem("스트랩", true);
        }

        _itemRepository.LoadList(CurItemInfo2FileName, EquipmentScreen.Instance.ItmSlot2, clearTarget: true);
        _itemRepository.LoadList(CurItemInfo3FileName, EquipmentScreen.Instance.ItmSlot3, clearTarget: true);

        //Initialize DataManager
        IsInitialized = true;
        Debug.Log("After Datamanager");
    }
    public void ChangePlayer(int newIdx)
    {
        int oldIdx = int.Parse(CurPlayer) - 1;
        if (Player_Info != null && Player_Info.Count > 0) AllPlayer_Info[oldIdx] = Player_Info[0];

        var next = AllPlayer_Info[newIdx - 1];
        if (Player_Info == null) Player_Info = new List<PlayerData>(1);
        if (Player_Info.Count == 0) Player_Info.Add(next);
        else Player_Info[0] = next;
        // Player_Info.Clear();
        // Player_Info.Add(AllPlayer_Info[newIdx - 1]);
        CurPlayer = newIdx.ToString();

        SavePlayerDataToAllPlayerJson();
        SavePlayerDataToJson();

        PlayerController[] pla = FindObjectsOfType<PlayerController>();
        foreach (PlayerController p1 in pla)
        {
            Destroy(p1.gameObject);
        }



        Instantiate(RoutineQueueManager.Instance.P2, RoutineQueueManager.Instance.Screen);
        Instantiate(RoutineQueueManager.Instance.P3, RoutineQueueManager.Instance.Screen);
        Instantiate(RoutineQueueManager.Instance.P1, RoutineQueueManager.Instance.Screen);

        //Check itemEquipment
        if (!TryGetItemByName(Player_Info[0].Item1, out var it1))
            EquipmentScreen.Instance.OnDeleteClick(EquipmentScreen.Instance.Item1_);   //unequip
        else
            EquipmentScreen.Instance.Equip_Item(it1, false);//equip

        if (!TryGetItemByName(Player_Info[0].Item2, out var it2))
            EquipmentScreen.Instance.OnDeleteClick(EquipmentScreen.Instance.Item2_);
        else
            EquipmentScreen.Instance.Equip_Item(it2, false);

        if (!TryGetItemByName(Player_Info[0].Item3, out var it3))
            EquipmentScreen.Instance.OnDeleteClick(EquipmentScreen.Instance.Item3_);
        else
            EquipmentScreen.Instance.Equip_Item(it3, false);

        EquipmentScreen.Instance.GetItem(CurPlayer, false);

        //레벨업 UI 초기화 & 재실행
        LevelUpScreen.Instance.reset_();

        for (int i = 0; i < 6; i++) LevelUp(RoutineQueueManager.Instance.Lv1[i], 0);

    }

    public void LevelUp(GameObject name_, float exp_) //Abs,Arm,Back,Chest,Leg,SHoulder
    {
        string n = name_.name;
        float bonus;

        if (n.Contains("Shoulder"))
        {
            if (EquipmentScreen.Instance.EXPS_.TryGetValue("ShoulderEXP", out bonus)) exp_ += bonus;
            Player_Info[0].ShoulderEXP = exp_;
            LevelUpScreen.Instance.Set_FillAmount(Player_Info[0].ShoulderEXP, Player_Info[0].ShoulderMax, LevelUpScreen.Instance.fills[5]);
        }
        else if (n.Contains("Chest"))
        {
            if (EquipmentScreen.Instance.EXPS_.TryGetValue("ChestEXP", out bonus)) exp_ += bonus;
            Player_Info[0].ChestEXP = exp_;
            LevelUpScreen.Instance.Set_FillAmount(Player_Info[0].ChestEXP, Player_Info[0].ChestMax, LevelUpScreen.Instance.fills[3]);
        }
        else if (n.Contains("Arm"))
        {
            if (EquipmentScreen.Instance.EXPS_.TryGetValue("ArmEXP", out bonus)) exp_ += bonus;
            Player_Info[0].ArmEXP = exp_;
            LevelUpScreen.Instance.Set_FillAmount(Player_Info[0].ArmEXP, Player_Info[0].ArmMax, LevelUpScreen.Instance.fills[1]);
        }
        else if (n.Contains("Abs"))
        {
            if (EquipmentScreen.Instance.EXPS_.TryGetValue("AbsEXP", out bonus)) exp_ += bonus;
            Player_Info[0].AbsEXP = exp_;
            LevelUpScreen.Instance.Set_FillAmount(Player_Info[0].AbsEXP, Player_Info[0].AbsMax, LevelUpScreen.Instance.fills[0]);
        }
        else if (n.Contains("Back"))
        {
            if (EquipmentScreen.Instance.EXPS_.TryGetValue("BackEXP", out bonus)) exp_ += bonus;
            Player_Info[0].BackEXP = exp_;
            LevelUpScreen.Instance.Set_FillAmount(Player_Info[0].BackEXP, Player_Info[0].BackMax, LevelUpScreen.Instance.fills[2]);
        }
        else if (n.Contains("Leg"))
        {
            if (EquipmentScreen.Instance.EXPS_.TryGetValue("LegEXP", out bonus)) exp_ += bonus;
            Player_Info[0].LegEXP = exp_;
            LevelUpScreen.Instance.Set_FillAmount(Player_Info[0].LegEXP, Player_Info[0].LegMax, LevelUpScreen.Instance.fills[4]);
        }
        //need checkpoint
        //SavePlayerDataToJson();
    }

    /// <summary>
    /// 매 10초마다 자동 저장
    /// Enabled 시 자동 저장 코루틴 시작
    /// Disabled 시 자동 저장 코루틴 중지 및 즉시 저장
    /// </summary>
    [SerializeField] float autosaveInterval = 10f;
    Coroutine _autosave;
    void OnEnable() { if (_autosave == null) _autosave = StartCoroutine(AutoSaveLoop()); }
    void OnDisable() { if (_autosave != null) { StopCoroutine(_autosave); _autosave = null; } SavePlayerDataToJson(); }
    //void OnApplicationPause(bool p) { if (p) SavePlayerDataToJson(); }

    IEnumerator AutoSaveLoop()
    {
        var wait = new WaitForSeconds(autosaveInterval);
        while (true) { yield return wait; SavePlayerDataToJson(); }
    }

    private bool _legacyPlayerFileMigrated;
    private bool _legacyAllPlayerFileMigrated;

    public void SavePlayerDataToJson()    //Save Load state(player_Info -> json player_info)
    {
        EnsureRepositories();
        _playerRepository.Save(PlayerInfoFileName, Player_Info);
    }

    public void SavePlayerDataToAllPlayerJson()    //save Load state(Allplayer_Info -> json Allplayer_info)
    {
        EnsureRepositories();
        _playerRepository.Save(AllPlayerInfoFileName, AllPlayer_Info);
    }

    void LoadPlayerDataFromJson()    //진행상황 불러오기(json player_info -> 게임player_info)
    {
        EnsureRepositories();
        bool legacy;
        Player_Info = _playerRepository.Load(PlayerInfoFileName, out legacy);
        if (legacy && !_legacyPlayerFileMigrated)
        {
            _legacyPlayerFileMigrated = true;
            SavePlayerDataToJson();
        }
    }

    void LoadAllPlayerDataFromJson()    //진행상황 불러오기(json Allplayer -> 게임 Allplayer)
    {
        EnsureRepositories();
        bool legacy;
        AllPlayer_Info = _playerRepository.Load(AllPlayerInfoFileName, out legacy);
        if (legacy && !_legacyAllPlayerFileMigrated)
        {
            _legacyAllPlayerFileMigrated = true;
            SavePlayerDataToAllPlayerJson();
        }
    }

    public void SavePlayer1_Item()
    {
        EnsureRepositories();
        _itemRepository.SaveList(CurItemInfo1FileName, EquipmentScreen.Instance.ItmSlot1);
    }

    public void SavePlayer2_Item()
    {
        EnsureRepositories();
        _itemRepository.SaveList(CurItemInfo2FileName, EquipmentScreen.Instance.ItmSlot2);
    }

    public void SavePlayer3_Item()
    {
        EnsureRepositories();
        _itemRepository.SaveList(CurItemInfo3FileName, EquipmentScreen.Instance.ItmSlot3);
    }

    public void FormatAllPlayerData()
    {
        EnsureRepositories();
        AllPlayer_Info.Clear();
        Player_Info.Clear();
        EquipmentScreen.Instance.ItmSlot1.Clear();
        EquipmentScreen.Instance.ItmSlot2.Clear();
        EquipmentScreen.Instance.ItmSlot3.Clear();

        var empty = _itemRepository.EmptyListJson();
        _fileStore.Write(CurItemInfo1FileName, empty);
        _fileStore.Write(CurItemInfo2FileName, empty);
        _fileStore.Write(CurItemInfo3FileName, empty);

        AllPlayer_Info.Add(new PlayerData("1", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0));
        AllPlayer_Info.Add(new PlayerData("2", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0));
        AllPlayer_Info.Add(new PlayerData("3", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0));

        SavePlayerDataToAllPlayerJson();
        if (string.IsNullOrEmpty(CurPlayer)) CurPlayer = "1";
        var target = AllPlayer_Info.Find(p => p.name == CurPlayer) ?? AllPlayer_Info[0];
        Player_Info = new List<PlayerData> { target };
        SavePlayerDataToJson();
        //need end game button
    }
}
