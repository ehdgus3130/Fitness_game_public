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


    [SerializeField] private TextAsset FormatUser_DataBase; //All User data
    private readonly List<PlayerData> _allPlayerInfo = new List<PlayerData>();
    private readonly List<PlayerData> _playerInfo = new List<PlayerData>();
    private readonly List<ItemData> _items = new List<ItemData>();
    [SerializeField] private ItemDatabaseSO itemDatabase;
    private ItemDataService _itemDataService;
    private PlayerDataService _playerDataService;
    private JsonFileStore _fileStore;
    private IItemStore _itemRepository;
    private IPlayerStore _playerRepository;
    private IDataResourceProvider _resourceProvider;
    private bool _playerDataHooksBound;

    private const string PlayerInfoFileName = "Player_Info.json";
    private const string AllPlayerInfoFileName = "AllPlayer_Info.json";
    private const string CurItemInfo1FileName = "CurItem_Info1.json";
    private const string CurItemInfo2FileName = "CurItem_Info2.json";
    private const string CurItemInfo3FileName = "CurItem_Info3.json";

    [SerializeField] private Sprite[] muscleItem;
    public string CurPlayer { get; private set; }        //CUrrent PlayerData
    private string _PersistentDataPath;

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

        if (_itemDataService == null)
        {
            _itemDataService = new ItemDataService();
        }

        if (_playerDataService == null)
        {
            _playerDataService = new PlayerDataService();
        }

        if (_resourceProvider == null)
        {
            _resourceProvider = new UnityDataResourceProvider();
        }

        if (!_playerDataHooksBound)
        {
            PlayerData.MuscleLevelUpHandler = PlayerDataRuntimeBridge.NotifyMuscleLevelUp;
            PlayerData.GiftBoxCountUpHandler = PlayerDataRuntimeBridge.NotifyGiftBoxCountUp;
            PlayerData.ScoreChangedHandler = PlayerDataRuntimeBridge.NotifyScoreChanged;
            PlayerData.MuscleRecalculateHandler = PlayerDataRuntimeBridge.RecalculateMuscleLevel;
            _playerDataHooksBound = true;
        }
    }

    public bool IsInitialized { get; private set; }
    public bool ItemsLoaded { get; private set; }
    public TextAsset FormatUserDatabase => FormatUser_DataBase;
    public ItemDatabaseSO ItemDatabase => itemDatabase;
    public Sprite[] MuscleItems => muscleItem;
    public IReadOnlyList<PlayerData> AllPlayerInfo => _allPlayerInfo;
    public IReadOnlyList<PlayerData> CurrentPlayerInfo => _playerInfo;
    public IReadOnlyList<ItemData> ItemCatalog => _items;

    public PlayerData GetCurrentPlayer()
    {
        if (_playerInfo.Count == 0) return null;
        return _playerInfo[0];
    }

    public bool TryGetCurrentPlayer(out PlayerData player)
    {
        player = GetCurrentPlayer();
        return player != null;
    }

    public string GetCurrentPlayerId() => CurPlayer;

    public IReadOnlyList<ItemData> GetItems() => _items;

    public bool TryGetCurrentPlayerDayLevel(out int dayLevel)
    {
        dayLevel = 1;
        var current = GetCurrentPlayer();
        if (current == null) return false;
        return int.TryParse(current.DAYLV, out dayLevel);
    }

    public ItemData FindItemByName(string itemName)
    {
        EnsureRepositories();
        return _itemDataService.TryGetByName(_items, itemName, out var item) ? item : null;
    }

    public bool TryFindItemByName(string itemName, out ItemData item)
    {
        EnsureRepositories();
        return _itemDataService.TryGetByName(_items, itemName, out item);
    }

    public bool IsCurrentPlayerItemEquipped(string itemName)
    {
        var p = GetCurrentPlayer();
        if (p == null || string.IsNullOrEmpty(itemName)) return false;
        return p.Item1 == itemName || p.Item2 == itemName || p.Item3 == itemName;
    }

    public void SetCurrentPlayerItem(int slot, string itemName)
    {
        var p = GetCurrentPlayer();
        if (p == null) return;

        switch (slot)
        {
            case 1: p.Item1 = itemName ?? ""; break;
            case 2: p.Item2 = itemName ?? ""; break;
            case 3: p.Item3 = itemName ?? ""; break;
        }
    }

    public void ClearCurrentPlayerItem(int slot) => SetCurrentPlayerItem(slot, "");

    public void SetCurrentPlayerDayLevel(int dayLevel)
    {
        var current = GetCurrentPlayer();
        if (current == null) return;

        var dayLevelText = dayLevel.ToString();
        current.DAYLV = dayLevelText;

        if (!int.TryParse(CurPlayer, out var parsedId)) return;
        var index = parsedId - 1;
        if (index < 0 || index >= _allPlayerInfo.Count) return;
        if (_allPlayerInfo[index] == null) return;

        _allPlayerInfo[index].DAYLV = dayLevelText;
    }
    void Start() //private IEnumerator
    {   //전체 플레이어 리스트 불러오기
        _PersistentDataPath = Application.persistentDataPath; //edit path
        EnsureRepositories();

#if UNITY_EDITOR
        Debug.Log("디버깅용 전체 포멧 진행");
        FormatAllPlayerData();
#endif

        InitializeStateContainers();
        LogInitializationResult(InitializeItems());
        LogInitializationResult(InitializeAllPlayers());
        LogInitializationResult(InitializeCurrentPlayer());
        InitializeEquipment();
        FinalizeInitialization();
    }

    private static void LogInitializationResult(InitStepResult result)
    {
        if (string.IsNullOrEmpty(result.WarningMessage)) return;
        Debug.LogWarning(result.WarningMessage);
    }

    private void InitializeStateContainers()
    {
        _playerInfo.Clear();
        _allPlayerInfo.Clear();
    }

    private InitStepResult InitializeItems()
    {
        var warnings = new List<string>();
        _items.Clear();

        if (itemDatabase == null) itemDatabase = _resourceProvider.Load<ItemDatabaseSO>("DB/ItemDatabase");

        if (itemDatabase != null && itemDatabase.items != null && itemDatabase.items.Count > 0)
        {
            for (int i = 0; i < itemDatabase.items.Count; i++)
            {
                var def = itemDatabase.items[i];
                if (def == null) continue;
                _items.Add(def.ToItemData());
            }
        }
        else
        {
            var itemTa = _resourceProvider.Load<TextAsset>("Json/Item_Info");
            if (itemTa != null)
            {
                var parsedItems = _itemRepository.ParseFromResourceJson(itemTa.text);
                if (parsedItems != null)
                {
                    foreach (var it in parsedItems)
                    {
                        _items.Add(new ItemData(it.name, it.var, it.effect, it.rate, it.explain));
                    }
                }
            }
            else
            {
                warnings.Add("Item initialization fallback failed: Item_Info.json not found in Resources/Json and ItemDatabaseSO not assigned.");
            }
        }

        _itemDataService.RebuildIndex(_items);
        muscleItem = _resourceProvider.LoadAll<Sprite>("Images/Items");
        ItemsLoaded = true;

        if (_items.Count == 0)
        {
            warnings.Add("Item initialization completed with empty item list.");
        }

        return InitStepResult.FromWarnings(warnings);
    }

    private InitStepResult InitializeAllPlayers()
    {
        var warnings = new List<string>();
        var allTa = _resourceProvider.Load<TextAsset>("Json/AllPlayer_Info");
        if (_fileStore.Exists(AllPlayerInfoFileName))
        {
            LoadAllPlayerDataFromJson();
        }
        else if (allTa != null)
        {
            _fileStore.Write(AllPlayerInfoFileName, allTa.text);
            LoadAllPlayerDataFromJson();
        }
        else
        {
            warnings.Add("All player data source missing. Creating default players.");
            ReplaceAllPlayerInfo(_playerDataService.CreateDefaultPlayers());
            SaveAllPlayers();
        }

        if (_allPlayerInfo.Count == 0)
        {
            warnings.Add("All player data was empty after load. Rebuilding with defaults.");
            ReplaceAllPlayerInfo(_playerDataService.CreateDefaultPlayers());
            SaveAllPlayers();
        }

        return InitStepResult.FromWarnings(warnings);
    }

    private InitStepResult InitializeCurrentPlayer()
    {
        var warnings = new List<string>();
        if (_fileStore.Exists(PlayerInfoFileName))
        {
            LoadPlayerDataFromJson();
        }
        else
        {
            warnings.Add("Player_Info.json missing. Creating current player from AllPlayer data.");
            CreateAndPersistCurrentPlayer();
        }

        if (_playerInfo.Count == 0 || _playerInfo[0] == null)
        {
            warnings.Add("Loaded current player data invalid. Rebuilding from AllPlayer data.");
            CreateAndPersistCurrentPlayer();
        }

        return InitStepResult.FromWarnings(warnings);
    }

    private void CreateAndPersistCurrentPlayer()
    {
        var target = _playerDataService.EnsureCurrentPlayer(_allPlayerInfo, CurPlayer, out var resolvedCurPlayer);
        CurPlayer = resolvedCurPlayer;
        SetCurrentPlayer(target);
        SaveCurrentPlayer();
    }

    private void InitializeEquipment()
    {
        PlayerRuntimeDataSync.LoadEquippedItems(
            _itemRepository,
            CurItemInfo1FileName,
            CurItemInfo2FileName,
            CurItemInfo3FileName);
    }

    private void FinalizeInitialization()
    {
        IsInitialized = true;
        Debug.Log("After Datamanager");
    }
    public void ChangePlayer(int newIdx)
    {
        if (!_playerDataService.TrySwitchPlayer(_allPlayerInfo, _playerInfo, CurPlayer, newIdx, out var nextPlayerId, out var next))
        {
            return;
        }

        SetCurrentPlayer(next);
        CurPlayer = nextPlayerId;

        SaveAllPlayers();
        SaveCurrentPlayer();

        PlayerChangeOrchestrator.Instance.ApplyAfterPlayerSwitch();
    }

    public void LevelUp(GameObject name_, float exp_) //Abs,Arm,Back,Chest,Leg,SHoulder
    {
        if (_playerInfo.Count == 0) return;
        PlayerRuntimeDataSync.ApplyLevelUp(_playerInfo[0], name_, exp_);
    }

    /// <summary>
    /// 매 10초마다 자동 저장
    /// Enabled 시 자동 저장 코루틴 시작
    /// Disabled 시 자동 저장 코루틴 중지 및 즉시 저장
    /// </summary>
    [SerializeField] float autosaveInterval = 10f;
    Coroutine _autosave;
    void OnEnable() { if (_autosave == null) _autosave = StartCoroutine(AutoSaveLoop()); }
    void OnDisable() { if (_autosave != null) { StopCoroutine(_autosave); _autosave = null; } SaveCurrentPlayer(); }
    //void OnApplicationPause(bool p) { if (p) SaveCurrentPlayer(); }

    IEnumerator AutoSaveLoop()
    {
        var wait = new WaitForSeconds(autosaveInterval);
        while (true) { yield return wait; SaveCurrentPlayer(); }
    }

    private bool _legacyPlayerFileMigrated;
    private bool _legacyAllPlayerFileMigrated;

    public void SaveCurrentPlayer()    //Save Load state(player_Info -> json player_info)
    {
        EnsureRepositories();
        _playerRepository.Save(PlayerInfoFileName, _playerInfo);
    }

    public void SaveAllPlayers()    //save Load state(Allplayer_Info -> json Allplayer_info)
    {
        EnsureRepositories();
        _playerRepository.Save(AllPlayerInfoFileName, _allPlayerInfo);
    }


    void LoadPlayerDataFromJson()    //진행상황 불러오기(json player_info -> 게임player_info)
    {
        EnsureRepositories();
        bool legacy;
        ReplaceCurrentPlayerInfo(_playerRepository.Load(PlayerInfoFileName, out legacy));
        if (legacy && !_legacyPlayerFileMigrated)
        {
            _legacyPlayerFileMigrated = true;
            SaveCurrentPlayer();
        }
    }

    void LoadAllPlayerDataFromJson()    //진행상황 불러오기(json Allplayer -> 게임 Allplayer)
    {
        EnsureRepositories();
        bool legacy;
        ReplaceAllPlayerInfo(_playerRepository.Load(AllPlayerInfoFileName, out legacy));
        if (legacy && !_legacyAllPlayerFileMigrated)
        {
            _legacyAllPlayerFileMigrated = true;
            SaveAllPlayers();
        }
    }

    public void SavePlayerItem(int slot)
    {
        EnsureRepositories();
        switch (slot)
        {
            case 1:
                PlayerRuntimeDataSync.SaveEquippedItemSlot(_itemRepository, CurItemInfo1FileName, 1);
                break;
            case 2:
                PlayerRuntimeDataSync.SaveEquippedItemSlot(_itemRepository, CurItemInfo2FileName, 2);
                break;
            case 3:
                PlayerRuntimeDataSync.SaveEquippedItemSlot(_itemRepository, CurItemInfo3FileName, 3);
                break;
        }
    }

    public void FormatAllPlayerData()
    {
        EnsureRepositories();
        _allPlayerInfo.Clear();
        _playerInfo.Clear();
        PlayerRuntimeDataSync.ClearEquipmentAndPersistEmpty(
            _fileStore,
            _itemRepository,
            CurItemInfo1FileName,
            CurItemInfo2FileName,
            CurItemInfo3FileName);

        ReplaceAllPlayerInfo(_playerDataService.CreateDefaultPlayers());

        SaveAllPlayers();
        var target = _playerDataService.EnsureCurrentPlayer(_allPlayerInfo, CurPlayer, out var resolvedCurPlayer);
        CurPlayer = resolvedCurPlayer;
        SetCurrentPlayer(target);
        SaveCurrentPlayer();
        //need end game button
    }

    private void ReplaceAllPlayerInfo(List<PlayerData> source)
    {
        _allPlayerInfo.Clear();
        if (source == null) return;
        _allPlayerInfo.AddRange(source);
    }

    private void ReplaceCurrentPlayerInfo(List<PlayerData> source)
    {
        _playerInfo.Clear();
        if (source == null) return;
        _playerInfo.AddRange(source);
    }

    private void SetCurrentPlayer(PlayerData player)
    {
        _playerInfo.Clear();
        if (player != null) _playerInfo.Add(player);
    }

    public void SetResourceProvider(IDataResourceProvider provider)
    {
        _resourceProvider = provider ?? new UnityDataResourceProvider();
    }

    public void SetItemStore(IItemStore store)
    {
        if (_fileStore == null) EnsureRepositories();
        _itemRepository = store ?? new ItemRepository(_fileStore);
    }

    public void SetPlayerStore(IPlayerStore store)
    {
        if (_fileStore == null) EnsureRepositories();
        _playerRepository = store ?? new PlayerRepository(_fileStore);
    }

    private readonly struct InitStepResult
    {
        public string WarningMessage { get; }

        private InitStepResult(string warningMessage)
        {
            WarningMessage = warningMessage;
        }

        public static InitStepResult FromWarnings(List<string> warnings)
        {
            if (warnings == null || warnings.Count == 0) return new InitStepResult(null);
            return new InitStepResult(string.Join("\n", warnings));
        }
    }
}
