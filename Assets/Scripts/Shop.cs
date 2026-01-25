using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : Singleton<Shop>
{
    [Header("EventCheck")]
    public GameObject Already_Itm_get;
    public GameObject Itm_get;
    public GameObject itempre;
    public GameObject Itm_Requ;
    public GameObject Cnt_buy;
    public GameObject Ads_EventPanel;
    public delegate GameObject _Random(int a);
    public _Random userandom;

    [Header("Time2x")]
    [SerializeField] private Transform x2BtnPos;
    private Button x2btn;
    private CanvasGroup x2BtnCG;

    [Header("GIFTBOX")]
    [SerializeField] private Transform GiftBtnPos;
    private Button Giftbtn;
    private GameObject[] GiftBar; //max 5
    private int Giftcnt;

    [Header("Multiple RandomBOX")]
    public Transform[] TimeItemTr;
    private GameObject[] TimeItem;
    public TMP_Text Timetext;
    private float timeLeftSec;
    private bool refreshedThisCycle = false;

    [Header("RandomBOX")]
    public Button RandomBox;
    public GameObject RandomBoxFee;

    [Header("InAppPurchase")]
    public Button CASH;

    [Header("SOUND")]
    public AudioSource ShopSound;
    [SerializeField] private TMP_FontAsset uiFont;


    void Awake()
    {
        x2btn = x2BtnPos.GetComponent<Button>();
        x2BtnCG = x2BtnPos.GetComponent<CanvasGroup>();

        Giftbtn = GiftBtnPos.GetComponent<Button>();
        GiftBar = new GameObject[5];
        int count = GiftBtnPos.GetChild(1).childCount;
        for (int i = 0; i < count; i++)
            GiftBar[i] = GiftBtnPos.GetChild(1).GetChild(i).gameObject;

        TimeItem = new GameObject[6];
        for (int j = 0; j < TimeItem.Length; j++)
        {
            TimeItem[j] = TimeItemTr[j].gameObject;
        }

    }

    void Start()
    {
        StartCoroutine(ExecuteEveryTenMinutes());

        Giftcnt = 1;
        UpdateGiftBarUI();


        timeLeftSec = 3600f; // 1시간
        Timetext.text = "01 : 00 : 00";

        for (int i = 0; i < TimeItem.Length; i++)
        {
            var newone = Instantiate(itempre, TimeItem[i].transform);

            if (!newone.TryGetComponent<Sell_Item>(out var si))
            {
                Destroy(newone);
                Debug.LogError("[Shop] Sell_Item 컴포넌트가 없습니다.");
                return;
            }
            si.UseRandom(6);
            newone.name = si.coreimg.name;
            Debug.Log(si.coreimg.name);

            TimeItem[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClick_1HTimeSale(newone);
            });
        }
    }

    /// <summary>
    /// Sound and Vib func
    /// </summary>
    public void OnClickReactEvent()
    {
        if (!Settings.Instance.IsSOUNDClicked) ShopSound.Play();
        if (!Settings.Instance.IsVIBRATIONClicked) Handheld.Vibrate();
    }
    void Update()
    {
        // 1) 시간 감소
        if (timeLeftSec > 0f)
        {
            timeLeftSec -= Time.deltaTime;
            if (timeLeftSec < 0f) timeLeftSec = 0f;
        }

        // 2) 텍스트 표시
        var t = Mathf.CeilToInt(timeLeftSec);
        int h = t / 3600; int m = (t % 3600) / 60; int s = t % 60;
        Timetext.text = $"{h:D2} : {m:D2} : {s:D2}";

        // 3) 만료 처리(한 번만)
        if (timeLeftSec == 0f && !refreshedThisCycle)
        {
            refreshedThisCycle = true;
            RefreshTimeSaleItems();   // 슬롯 재생성
            timeLeftSec = 3600f;      // 재시작
            refreshedThisCycle = false;
        }
    }


    /// <summary>
    /// Time Up Event button func
    /// </summary>
    public void OnClick_TImeUp()
    {
        OnClickReactEvent();
        Ads_EventPanel.SetActive(true);
        Ads_EventPanel.GetComponent<AdPanel>().ShowAdConfirmation
        ("광고를 시청하면 10초 동안\n 2배속 모드가 적용됩니다.\n시청하시겠습니까?",
        () => GoogleAds.Instance.ShowRewardedAd1());
    }

    public IEnumerator RewardOfTime1()
    {
        Time.timeScale = 2.0f;
        x2BtnCG.alpha = 0.6f;
        x2btn.interactable = false;
        yield return new WaitForSecondsRealtime(10f);
        x2BtnCG.alpha = 1.0f;
        Time.timeScale = 1.0f;
        x2btn.interactable = false;
    }
    public void GiftBoxCntUP()
    {
        if (Giftcnt < GiftBar.Length)
            Giftcnt++;

        UpdateGiftBarUI();
    }
    private void UpdateGiftBarUI()
    {
        for (int i = 0; i < GiftBar.Length; i++)
            GiftBar[i].GetComponent<CanvasGroup>().alpha = (i < Giftcnt ? 1f : 0.2f);

        bool ready = (Giftcnt >= GiftBar.Length);

        Giftbtn.interactable = ready;
        Giftbtn.image.raycastTarget = ready;
    }
    /// <summary>
    /// GIft BUtton func
    /// 5번의 기회가 생긴 순간에만 클릭가능하다
    /// </summary>
    public void OnClick_GiftBox()
    {
        OnClickReactEvent();

        Giftcnt = 0;            //Reset ALL
        UpdateGiftBarUI();

        var newone = Instantiate(itempre); //Execption
        if (!newone.TryGetComponent<Sell_Item>(out var si))
        {
            Destroy(newone);
            Debug.LogError("[Shop] Sell_Item 컴포넌트가 없습니다.");
            return;
        }
        si.UseRandom(1);
        si.name = si.coreimg.sprite.name;

        //parsing the price
        int qty = 1;
        TMP_Text pri = newone.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        var raw = pri != null ? (pri.text ?? "").Trim().TrimStart('x', 'X', ' ') : "1";
        if (!int.TryParse(raw, out qty)) qty = 1; //String to int

        if (IfOverlapItem(newone))    //중복: “이미 보유” UI + 보상(경험치 루틴 지급)
        {
            Sell_Item it = si.GetComponent<Sell_Item>();

            Already_Itm_get.SetActive(true);
            Already_Itm_get.GetComponent<AL_Item_Get>().SetName(newone.name);
            Already_Itm_get.GetComponent<AL_Item_Get>().itmCore.sprite = it.coreimg.sprite;
            for (int i = 0; i < qty; i++)
                LevelUpScreen.Instance.EndRoutin(it.ItemPrice);
        }
        else                        //신규: “획득” UI + 인벤토리에 지급
        {
            Itm_get.SetActive(true);
            Itm_get.GetComponent<Item_Get>().SetName(newone.name);
            EquipmentScreen.Instance.GetItem(newone.name, true);
        }

        Destroy(newone);
    }

    void OnClick_1HTimeSale(GameObject item)
    {
        OnClickReactEvent();

        var si = item.GetComponent<Sell_Item>();
        if (!si) { Debug.LogError("[Shop] Sell_Item 없음"); return; }

        int qty = 1;
        TMP_Text pri = item.GetComponent<Sell_Item>().priceText;
        var raw = pri != null ? (pri.text ?? "").Trim().TrimStart('x', 'X', ' ') : "1";
        if (!int.TryParse(raw, out qty)) qty = 1; //String to int


        if (IfOverlapItem(item)) //겹치는 경우
        {
            Cnt_buy.SetActive(true);
            return;

        }
        else
        {
            // 3) 재화/가격 결정 (대소문자 무시)
            string feeName = si.ITEMPRICE ? si.ITEMPRICE.name : "";
            string feeLower = feeName.ToLowerInvariant();
            float unitCost =
                feeLower.Contains("red") ? 600f :
                feeLower.Contains("yellow") ? 250f :
                feeLower.Contains("green") ? 100f : 30f;

            // 4) 부위 번호 추출 (끝 한 글자 의존 ↓ 최소한의 가드)
            string number = feeName.Length > 0 ? feeName.Substring(feeName.Length - 1) : "0";

            // 5) 구매 시도 → 성공 시 지급
            if (Buy(item.name, number, qty, unitCost, item)) //구매가능
                EquipmentScreen.Instance.GetItem(item.name, true);
            else
                Cnt_buy.SetActive(true); //재화부족

            return;
        }
    }

    /// <summary>
    /// 종류별 구매물품 생성후 실제 경험치에서 가감하기
    /// </summary>
    /// <param name="name">아이템의 이름</param>
    /// <param name="number">지불한 부위</param>
    /// <param name="many">지불한 부위의 양</param>
    /// <param name="exp">지불한 부위의 값</param>
    /// <param name="place">해당 Sell Item</param>
    /// <returns>살지에 대한 여부</returns>
    bool Buy(string name, string number, int many, float exp, GameObject place)
    {   //to buy thing / Fee / Fee / clicked button

        var p = DataManager.Instance.Player_Info[0];
        var btn = place ? place.GetComponent<Button>() : null;
        switch (number)
        {
            case "1":
                if (TryBuyPart(p.AbsEXP, p.AbsLV, p.AbsMax, 0, name, many, exp, btn, out var newExp))
                {
                    p.AbsEXP = newExp;
                    return true;
                }
                break;
            case "5":
                if (TryBuyPart(p.ArmEXP, p.ArmLV, p.ArmMax, 1, name, many, exp, btn, out var newExp1))
                {
                    p.ArmEXP = newExp1;
                    return true;
                }
                break;
            case "4":
                if (TryBuyPart(p.BackEXP, p.BackLV, p.BackMax, 2, name, many, exp, btn, out var newExp2))
                {
                    p.BackEXP = newExp2;
                    return true;
                }
                break;
            case "3":
                if (TryBuyPart(p.ChestEXP, p.ChestLV, p.ChestMax, 3, name, many, exp, btn, out var newExp3))
                {
                    p.ChestEXP = newExp3;
                    return true;
                }
                break;
            case "2":
                if (TryBuyPart(p.LegEXP, p.LegLV, p.LegMax, 4, name, many, exp, btn, out var newExp4))
                {
                    p.LegEXP = newExp4;
                    return true;
                }
                break;
            case "0":
                if (TryBuyPart(p.ShoulderEXP, p.ShoulderLV, p.ShoulderMax, 5, name, many, exp, btn, out var newExp5))
                {
                    p.ShoulderEXP = newExp5;
                    return true;
                }
                break;
            default: return false;
        }
        return false;
    }
    private bool TryBuyPart(
        float expField, float level, float max, int fillIdx,
        string itemName, int qty, float unitCost, Button placeBtn, out float newExp)
    {
        float price = qty * unitCost;
        if ((level - 1) * max > price)
        {                 // 기존 판정 유지
            newExp = -price;                            // (당장 유지, 아래 ‘개선안’ 참고)
            Itm_get.SetActive(true);
            Itm_get.GetComponent<Item_Get>().SetName(itemName);
            if (placeBtn) placeBtn.interactable = false;
            LevelUpScreen.Instance.Set_FillAmount(newExp, max, LevelUpScreen.Instance.fills[fillIdx]);
            return true;
        }
        Cnt_buy.SetActive(true);
        newExp = expField;
        return false;
    }


    /// <summary>
    /// Time Ad button func
    /// </summary>
    public void Onclick_TimeSaleBtn()
    {
        OnClickReactEvent();
        Ads_EventPanel.SetActive(true);
        Ads_EventPanel.GetComponent<AdPanel>().ShowAdConfirmation
        ("광고를 시청하면\n 아이템 상점이 새로고침됩니다.\n시청하시겠습니까?",
        () => GoogleAds.Instance.ShowRewardedAd2());
    }

    public void RewardOfTime2()
    {
        RefreshTimeSaleItems();
        timeLeftSec = 3600f;
    }

    public void OnClick_RandomBox()
    {
        OnClickReactEvent();

        var newone = Instantiate(itempre);

        if (!newone.TryGetComponent<Sell_Item>(out var si))
        {
            Destroy(newone);
            Debug.LogError("[Shop] Sell_Item 컴포넌트가 없습니다.");
            return;
        }
        si.UseRandom(1);
        string displayName = si.coreimg != null && si.coreimg.sprite != null
        ? si.coreimg.sprite.name
        : newone.name;

        int qty = 1;
        TMP_Text pri = newone.GetComponent<Sell_Item>().priceText;
        var raw = pri != null ? (pri.text ?? "").Trim().TrimStart('x', 'X', ' ') : "1";
        if (!int.TryParse(raw, out qty)) qty = 1; //String to int

        // 3) 재화/가격 결정 (대소문자 무시)
        string feeName = si.ITEMPRICE ? si.ITEMPRICE.name : "";
        string feeLower = feeName.ToLowerInvariant();
        float unitCost =
            feeLower.Contains("red") ? 600f :
            feeLower.Contains("yellow") ? 250f :
            feeLower.Contains("green") ? 100f : 30f;

        // 4) 부위 번호 추출 (끝 한 글자 의존 ↓ 최소한의 가드)
        string number = feeName.Length > 0 ? feeName.Substring(feeName.Length - 1) : "0";

        if (IfOverlapItem(newone)) //겹치는 경우
        {
            Already_Itm_get.SetActive(true);
            var Already = Already_Itm_get.GetComponent<AL_Item_Get>();
            Already.SetName(newone.name);
            Already.itmCore = si.coreimg;

            for (int i = 0; i < qty; i++)
                LevelUpScreen.Instance.EndRoutin(si.ItemPrice);
        }
        else //겹치지 않는 경우 가격 확인
        {

            if (Buy(newone.name, number, qty, unitCost, RandomBox.gameObject)) //구매가능
            {
                Itm_get.SetActive(true);
                Itm_get.GetComponent<Item_Get>().SetName(newone.name);
                EquipmentScreen.Instance.GetItem(newone.name, true);
            }
            else
                Cnt_buy.SetActive(true); //재화부족
            Destroy(newone);
            return;
        }

    }

    private void RefreshTimeSaleItems()
    {
        for (int i = 0; i < TimeItem.Length; i++)
        {
            var holder = TimeItem[i];
            if (holder != null)
            {
                var child0 = holder.transform.childCount > 0 ? holder.transform.GetChild(0) : null;
                if (child0) { Destroy(child0.gameObject); }//{ Destroy(holder.gameObject); TimeItem[i] = TimeItemTr[i].gameObject; } // 기존 코드 준수
            }

            var newone = Instantiate(itempre, TimeItem[i].transform);//TimeItem[i].transform

            if (!newone.TryGetComponent<Sell_Item>(out var si))
            {
                Destroy(newone);
                Debug.LogError("[Shop] Sell_Item 컴포넌트가 없습니다.");
                return;
            }
            si.UseRandom(6);

            var localNew = newone;
            var btn = TimeItem[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { OnClick_1HTimeSale(localNew); });

        }
    }


    private IEnumerator ExecuteEveryTenMinutes()
    {
        var wait = new WaitForSecondsRealtime(600f);
        while (true)
        {
            int tier = UnityEngine.Random.Range(0, 4);
            GameObject src = tier switch
            {
                0 => QueueRoutin.Instance.Lv1[UnityEngine.Random.Range(0, QueueRoutin.Instance.Lv1.Length)],
                1 => QueueRoutin.Instance.Lv2[UnityEngine.Random.Range(0, QueueRoutin.Instance.Lv2.Length)],
                2 => QueueRoutin.Instance.Lv3[UnityEngine.Random.Range(0, QueueRoutin.Instance.Lv3.Length)],
                _ => QueueRoutin.Instance.Lv4[UnityEngine.Random.Range(0, QueueRoutin.Instance.Lv4.Length)],
            };
            var ui = RandomBoxFee.GetComponent<Image>();
            var srcImg = src.GetComponent<Image>();
            if (ui && srcImg) ui.sprite = srcImg.sprite;

            yield return wait;
        }
    }

    bool IfOverlapItem(GameObject item)
    {
        switch (int.Parse(DataManager.Instance.CurPlayer))
        {
            case 1:
                Itm thing = EquipmentScreen.Instance.ItmSlot1.Find(x => x.name == item.name);
                if (thing != null) return true;
                else return false;
            case 2:
                Itm thing1 = EquipmentScreen.Instance.ItmSlot2.Find(x => x.name == item.name);
                if (thing1 != null) return true;
                else return false;
            case 3:
                Itm thing2 = EquipmentScreen.Instance.ItmSlot3.Find(x => x.name == item.name);
                if (thing2 != null) return true;
                else return false;
        }
        return false;
    }
}
