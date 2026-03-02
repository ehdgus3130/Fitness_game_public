using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class EquipmentScreen : Singleton<EquipmentScreen>
{
    public List<ItemData> ItmSlot1 = new List<ItemData>();    //���� �÷��̾ ������ �ִ� �����۸� ǥ��
    public List<ItemData> ItmSlot2 = new List<ItemData>();    //���� �÷��̾ ������ �ִ� �����۸� ǥ��
    public List<ItemData> ItmSlot3 = new List<ItemData>();    //���� �÷��̾ ������ �ִ� �����۸� ǥ��

    public GameObject[] Item1_;
    public GameObject[] Item2_;
    public GameObject[] Item3_;

    public Sprite[] muscleItem;
    [Header("DeleteButton")]
    public Button Item1_DEL;
    public Button Item2_DEL;
    public Button Item3_DEL;

    [Header("CharacterChange")]
    public Button Character1; //change button
    public Button Character2;
    public Button Character3;
    public Image[] TabbtnImage; //tab button image component
    public Sprite[] TabbtnInputImage;
    public GameObject[] Character; //player background
    public Sprite IdleImg, SelectImg; //tabbutton image

    [Header("EXP")]
    public GameObject EXP_BAR;  //�� EXP�հ�
    public GameObject EXP_TEXT; //�� EXP�հ� ����

    public Sprite NormalImage;
    public GameObject PrefabItem;
    public GameObject Itm_Explain;

    private Transform ItmPos;

    public Sprite[] Bar_;   //��� bar �̹���

    [Header("SOUND")]
    public AudioSource EquipSound;

    public Dictionary<string, float> EXPS_ = new Dictionary<string, float>();
    enum exp { ene, fat, AbsEXP, ArmEXP, BackEXP, ChestEXP, LegEXP, ShoulderEXP };
    private PlayerData CurrentPlayer => DataManager.Instance.GetCurrentPlayer();

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
        DataManager.Instance && DataManager.Instance.IsInitialized);
        if (CurrentPlayer == null) yield break;
        TabClick(0);
        muscleItem = DataManager.Instance.MuscleItems;
        if (CurrentPlayer.Item1 != "" || CurrentPlayer.Item2 != ""
            || CurrentPlayer.Item3 != "")
        {
            if (CurrentPlayer.Item1 != "")
            {
                ItemData cur = DataManager.Instance.FindItemByName(CurrentPlayer.Item1);
                Equip_Item(cur, true);
            }
            if (CurrentPlayer.Item2 != "")
            {
                ItemData cur = DataManager.Instance.FindItemByName(CurrentPlayer.Item2);
                Equip_Item(cur, true);
            }
            if (CurrentPlayer.Item1 != "")
            {
                ItemData cur = DataManager.Instance.FindItemByName(CurrentPlayer.Item3);
                Equip_Item(cur, true);
            }
        }
        else
        {
            EXPS_.Add("ene", 0.0f);
            EXPS_.Add("fat", 0.0f);
            EXPS_.Add("AbsEXP", 0.0f);
            EXPS_.Add("ArmEXP", 0.0f);
            EXPS_.Add("BackEXP", 0.0f);
            EXPS_.Add("ChestEXP", 0.0f);
            EXPS_.Add("LegEXP", 0.0f);
            EXPS_.Add("ShoulderEXP", 0.0f);
            EXPS_.Add("time", 0.0f);
        }
        ItmPos = transform.GetChild(0).GetChild(0).GetChild(1);

        Item1_DEL.onClick.AddListener(() =>
        {
            OnDeleteClick(Item1_);
            OnClickReactEvent();
        });
        Item2_DEL.onClick.AddListener(() =>
        {
            OnDeleteClick(Item2_);
            OnClickReactEvent();
        });
        Item3_DEL.onClick.AddListener(() =>
        {
            OnDeleteClick(Item3_);
            OnClickReactEvent();
        });
        Character1.onClick.AddListener(() => DataManager.Instance.ChangePlayer(1));
        Character2.onClick.AddListener(() => DataManager.Instance.ChangePlayer(2));
        Character3.onClick.AddListener(() => DataManager.Instance.ChangePlayer(3));

        string name = DataManager.Instance.GetCurrentPlayerId();
        int num = int.Parse(name);
        DataManager.Instance.ChangePlayer(num);
    }

    /// <summary>
    /// Change PlayerData Character Btn
    /// </summary>
    /// <param name="n">PlayerData Number</param>
    public void TabClick(int n)
    {
        for (int i = 0; i < Character.Length; i++)
        {
            if (i == n) Character[i].transform.SetAsLastSibling();
            TabbtnImage[i].sprite = i == n ? SelectImg : IdleImg;

            switch (n)
            { //(1 off / on) (2 off / on) (3 off / on)
                case 0:
                    Character1.image.sprite = TabbtnInputImage[1];
                    Character2.image.sprite = TabbtnInputImage[2];
                    Character3.image.sprite = TabbtnInputImage[4];
                    break;
                case 1:
                    Character1.image.sprite = TabbtnInputImage[0];
                    Character2.image.sprite = TabbtnInputImage[3];
                    Character3.image.sprite = TabbtnInputImage[4];
                    break;
                case 2:
                    Character1.image.sprite = TabbtnInputImage[0];
                    Character2.image.sprite = TabbtnInputImage[2];
                    Character3.image.sprite = TabbtnInputImage[5];
                    break;
            }
        }
    }
    public void OnClickReactEvent()
    {
        if (!Settings.Instance.IsSoundEnabled) EquipSound.Play();
        if (!Settings.Instance.IsVibrationEnabled) Handheld.Vibrate();
    }
    public void GetItem(string NewItem, bool Isin)  //������ ȹ�� �� �̹���ȭ
    {
        if (Isin)   //Get item, Begin game 
        {

            ItemData _Get = DataManager.Instance.FindItemByName(NewItem);     //������ �������ִ��� Ȯ��
            if (CurrentPlayer.name == "1")
                ItmSlot1.Add(new ItemData(_Get.name, _Get.var, _Get.effect, _Get.rate, _Get.explain));
            else if (CurrentPlayer.name == "2")
                ItmSlot2.Add(new ItemData(_Get.name, _Get.var, _Get.effect, _Get.rate, _Get.explain));
            else if (CurrentPlayer.name == "3")
                ItmSlot3.Add(new ItemData(_Get.name, _Get.var, _Get.effect, _Get.rate, _Get.explain));

            GameObject getone = Instantiate(PrefabItem, ItmPos);
            getone.transform.GetChild(0).GetComponent<Image>().sprite = Array.Find(muscleItem, x => x.name == NewItem);
            getone.name = NewItem;

            getone.GetComponent<Button>().onClick.AddListener(() =>
            {
                Itm_Explain.SetActive(true);
                Itm_Explain.GetComponent<ItemExplain>().SetName(getone.name);
            });

            Data_Item_Save(getone.name, true, 4);   // to json
        }
        else
        {
            int childCount = ItmPos.childCount != 0 ? ItmPos.childCount : 0;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(ItmPos.GetChild(i).gameObject);
            }

            if (NewItem == "1")
            {
                for (int i = 0; i < ItmSlot1.Count; i++)
                {
                    GameObject newone = Instantiate(PrefabItem, ItmPos);
                    newone.name = ItmSlot1[i].name;
                    newone.transform.GetChild(0).GetComponent<Image>().sprite = Array.Find(muscleItem, x => x.name == newone.name);
                }

            }
            else if (NewItem == "2")
            {
                for (int i = 0; i < ItmSlot2.Count; i++)
                {
                    GameObject newone = Instantiate(PrefabItem, ItmPos);
                    newone.name = ItmSlot2[i].name;
                    newone.transform.GetChild(0).GetComponent<Image>().sprite = Array.Find(muscleItem, x => x.name == newone.name);
                }

            }
            else if (NewItem == "3")
            {
                for (int i = 0; i < ItmSlot3.Count; i++)
                {
                    GameObject newone = Instantiate(PrefabItem, ItmPos);
                    newone.name = ItmSlot3[i].name;
                    newone.transform.GetChild(0).GetComponent<Image>().sprite = Array.Find(muscleItem, x => x.name == newone.name);
                }

            }

            Button[] childButtons = ItmPos.GetComponentsInChildren<Button>();
            foreach (Button button in childButtons)
            {
                if (button.onClick.GetPersistentEventCount() > 0)
                {
                    continue;
                }
                else
                {
                    button.onClick.AddListener(() =>
                    {
                        Itm_Explain.SetActive(true);
                        Itm_Explain.GetComponent<ItemExplain>().SetName(button.name);
                    });
                }

            }

        }
    }

    public void Equip_Item(ItemData _name, bool JustDo)
    {
        if (JustDo)
        {
            if (CurrentPlayer.Item1 == "")
            {
                Data_Item_Save(_name.name, true, 1);
                Item1_[0].GetComponent<Image>().sprite = System.Array.Find(muscleItem, x => x.name == _name.name);
                Item1_[1].GetComponent<TMP_Text>().text = _name.name;
                Item1_[2].GetComponent<TMP_Text>().text = _name.explain;
                AddEXP(_name.var, _name.effect, true);
            }
            else
            {
                if (CurrentPlayer.Item2 == "")
                {
                    Data_Item_Save(_name.name, true, 2);
                    Item2_[0].GetComponent<Image>().sprite = System.Array.Find(muscleItem, x => x.name == _name.name);
                    Item2_[1].GetComponent<TMP_Text>().text = _name.name;
                    Item2_[2].GetComponent<TMP_Text>().text = _name.explain;
                    AddEXP(_name.var, _name.effect, true);
                }
                else
                {
                    if (CurrentPlayer.Item3 == "")
                    {
                        Data_Item_Save(_name.name, true, 3);
                        Item3_[0].GetComponent<Image>().sprite = System.Array.Find(muscleItem, x => x.name == _name.name);
                        Item3_[1].GetComponent<TMP_Text>().text = _name.name;
                        Item3_[2].GetComponent<TMP_Text>().text = _name.explain;
                        AddEXP(_name.var, _name.effect, true);
                    }
                }
            }
        }
        else
        {
            if (CurrentPlayer.Item1 == _name.name)
            {
                Data_Item_Save(_name.name, true, 1);
                Item1_[0].GetComponent<Image>().sprite = System.Array.Find(muscleItem, x => x.name == _name.name);
                Item1_[1].GetComponent<TMP_Text>().text = _name.name;
                Item1_[2].GetComponent<TMP_Text>().text = _name.explain;
                AddEXP(_name.var, _name.effect, true);
            }
            else
            {
                if (CurrentPlayer.Item2 == _name.name)
                {
                    Data_Item_Save(_name.name, true, 2);
                    Item2_[0].GetComponent<Image>().sprite = System.Array.Find(muscleItem, x => x.name == _name.name);
                    Item2_[1].GetComponent<TMP_Text>().text = _name.name;
                    Item2_[2].GetComponent<TMP_Text>().text = _name.explain;
                    AddEXP(_name.var, _name.effect, true);
                }
                else
                {
                    if (CurrentPlayer.Item3 == _name.name)
                    {
                        Data_Item_Save(_name.name, true, 3);
                        Item3_[0].GetComponent<Image>().sprite = System.Array.Find(muscleItem, x => x.name == _name.name);
                        Item3_[1].GetComponent<TMP_Text>().text = _name.name;
                        Item3_[2].GetComponent<TMP_Text>().text = _name.explain;
                        AddEXP(_name.var, _name.effect, true);
                    }
                }
            }
        }
    }

    public void AddEXP(string where, int how, bool Isin)
    {
        if (Isin)
        {
            foreach (exp ex in System.Enum.GetValues(typeof(exp)))
            {
                if (where == "ALL_EXP")
                    EXPS_[ex.ToString()] += how;
                else if (ex.ToString() == where)
                {
                    if (where == "ene" || where == "fat" || where == "time")
                    {
                        EXPS_[ex.ToString()] += how;
                        PlayerLevelController.Instance.ItemSkill(true);  //ene,fat,time�� ���
                    }
                    else
                        EXPS_[ex.ToString()] += how;
                }
            }
        }
        else
        {
            foreach (exp ex in System.Enum.GetValues(typeof(exp)))
            {
                if (where == "ALL_EXP")
                    EXPS_[ex.ToString()] -= how;
                else if (ex.ToString() == where)
                {
                    if (where == "ene" || where == "fat" || where == "time")
                    {
                        EXPS_[ex.ToString()] -= how;
                        PlayerLevelController.Instance.ItemSkill(false);  //ene,fat,time�� ���
                    }
                    else
                        EXPS_[ex.ToString()] -= how;
                }
            }
        }
        foreach (exp ex in System.Enum.GetValues(typeof(exp)))
        {
            if (ex.ToString() == "time")
            {
                continue;
            }
            else
            {
                EXP_TEXT.transform.Find(ex.ToString()).GetChild(0).GetComponent<TMP_Text>().text = EXPS_[ex.ToString()].ToString();

                Transform bar = EXP_BAR.transform.Find(ex.ToString());

                if (ex.ToString() == "ene" || ex.ToString() == "fat")
                {

                    if (1 <= EXPS_[ex.ToString()] && EXPS_[ex.ToString()] < 4)
                    {
                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[0];//tem_Explain.Instance.Bar[0];
                    }
                    else if (4 <= EXPS_[ex.ToString()] && EXPS_[ex.ToString()] < 7)
                    {
                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[1];//ItemExplain.Instance.Bar[3];
                        bar.GetChild(2).GetComponent<Image>().sprite = Bar_[2];//ItemExplain.Instance.Bar[4];
                    }
                    else if (7 <= EXPS_[ex.ToString()] && EXPS_[ex.ToString()] < 10)
                    {

                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[3];//ItemExplain.Instance.Bar[6];
                        bar.GetChild(2).GetComponent<Image>().sprite = Bar_[4];//ItemExplain.Instance.Bar[7];
                        bar.GetChild(1).GetComponent<Image>().sprite = Bar_[4];//ItemExplain.Instance.Bar[7];
                    }
                    else if (10 <= EXPS_[ex.ToString()])
                    {
                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[5];//ItemExplain.Instance.Bar[9];
                        bar.GetChild(2).GetComponent<Image>().sprite = Bar_[6]; //ItemExplain.Instance.Bar[10];
                        bar.GetChild(1).GetComponent<Image>().sprite = Bar_[6]; //ItemExplain.Instance.Bar[10];
                        bar.GetChild(0).GetComponent<Image>().sprite = Bar_[7]; //ItemExplain.Instance.Bar[11];
                    }
                }
                else
                {
                    if (1 <= EXPS_[ex.ToString()] && EXPS_[ex.ToString()] < 30)
                    {
                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[0];//ItemExplain.Instance.Bar[0];
                    }
                    else if (30 <= EXPS_[ex.ToString()] && EXPS_[ex.ToString()] < 50)
                    {
                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[1];//ItemExplain.Instance.Bar[3];
                        bar.GetChild(2).GetComponent<Image>().sprite = Bar_[2];//ItemExplain.Instance.Bar[4];
                    }
                    else if (50 <= EXPS_[ex.ToString()] && EXPS_[ex.ToString()] < 70)
                    {
                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[3];//ItemExplain.Instance.Bar[6];
                        bar.GetChild(2).GetComponent<Image>().sprite = Bar_[4];//ItemExplain.Instance.Bar[7];
                        bar.GetChild(1).GetComponent<Image>().sprite = Bar_[4];//ItemExplain.Instance.Bar[7];
                    }
                    else if (70 <= EXPS_[ex.ToString()])
                    {
                        bar.GetChild(3).GetComponent<Image>().sprite = Bar_[5];//ItemExplain.Instance.Bar[9];
                        bar.GetChild(2).GetComponent<Image>().sprite = Bar_[6];//ItemExplain.Instance.Bar[10];
                        bar.GetChild(1).GetComponent<Image>().sprite = Bar_[6];//ItemExplain.Instance.Bar[10];
                        bar.GetChild(0).GetComponent<Image>().sprite = Bar_[7];//ItemExplain.Instance.Bar[11];
                    }
                }
            }
        }
    }
    /// <summary>
    /// UnEquipment the item func
    /// </summary>
    /// <param name="Parent"> Item slot 1 or 2 or 3 </param>
    public void OnDeleteClick(GameObject[] Parent)
    {
        ItemData cur = DataManager.Instance.FindItemByName(Parent[1].GetComponent<TMP_Text>().text);
        if (cur != null) AddEXP(cur.var, cur.effect, false);

        if (Parent == Item1_) { Data_Item_Save(Parent[1].name, false, 1); }
        else if (Parent == Item2_) { Data_Item_Save(Parent[1].name, false, 2); }
        else if (Parent == Item3_) { Data_Item_Save(Parent[1].name, false, 3); }

        //make blank slot
        Parent[0].GetComponent<Image>().sprite = NormalImage;
        Parent[1].GetComponent<TMP_Text>().text = "이름";
        Parent[2].GetComponent<TMP_Text>().text = "설명";

    }


    void Data_Item_Save(string name, bool isIn, int where)   //������ ����/���� Json
    {
        if (isIn)        //������ ������
        {
            switch (where)
            {
                case 1:
                    DataManager.Instance.SetCurrentPlayerItem(1, name);
                    break;
                case 2:
                    DataManager.Instance.SetCurrentPlayerItem(2, name);
                    break;
                case 3:
                    DataManager.Instance.SetCurrentPlayerItem(3, name);
                    break;
                case 4:
                    if (int.TryParse(DataManager.Instance.GetCurrentPlayerId(), out var slot))
                    {
                        DataManager.Instance.SavePlayerItem(slot);
                    }
                    break;

            }
        }
        else        //������ ������
        {
            switch (where)
            {
                case 1:
                    DataManager.Instance.ClearCurrentPlayerItem(1);
                    break;
                case 2:
                    DataManager.Instance.ClearCurrentPlayerItem(2);
                    break;
                case 3:
                    DataManager.Instance.ClearCurrentPlayerItem(3);
                    break;
            }
        }
    }
}
