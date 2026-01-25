using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor; // EditorUtility 쓰려면 필요
#endif

[System.Serializable]
public class Itm    //json for Item Save data
{
    public string name; //name
    public string var;  //variable
    public int effect;  //effect scale
    public int rate;    //rate
    public string explain; //discription
    public Itm(string _name, string _where, int _how, int _rare, string _explain)
    {
        name = _name;
        var = _where;
        effect = _how;
        rate = _rare;
        explain = _explain;
    }
}


[System.Serializable]
public class PlayerSaveData
{
    public string name;
    public int energy;
    public int fatigue;
    public int dayLv;

    public string item1;
    public string item2;
    public string item3;

    public float[] lvs = new float[6];
    public float[] exps = new float[6];
    public float[] maxExps = new float[6];
}

[System.Serializable]
public class Player //json for player information
{
    public string name, Energy, Fatigue, DAYLV;

    public string Item1, Item2, Item3;

    private float[] Lvs = new float[6]; //Abs,Arm,Back,Chest,Leg,SHoulder

    private float AbsExp;
    public float AbsMax;
    public float AbsEXP
    {
        get { return AbsExp; }
        set
        {
            AbsExp += value;
            if (AbsExp < 0)
            {
                for (int i = 0; AbsExp < 0; i++)
                {
                    AbsLV -= 1;
                }

            }
            else if (AbsExp >= 2000 && Lvs[0] < 10)      //0 to 10
            {
                Lvs[0]++;
                AbsExp = AbsExp - AbsMax;
                AbsMax = 2000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[0].gameObject);
                Shop.Instance.GiftBoxCntUP();

            }
            else if (AbsExp >= 5000 && Lvs[0] < 25)   //10 to 25
            {
                Lvs[0]++;
                AbsExp = AbsExp - AbsMax;
                AbsMax = 5000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[0].gameObject);
                Shop.Instance.GiftBoxCntUP();

            }
            else if (AbsExp >= 10000 && Lvs[0] < 75)  //25 to 75
            {
                Lvs[0]++;
                AbsExp = AbsExp - AbsMax;
                AbsMax = 10000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[0].gameObject);
                Shop.Instance.GiftBoxCntUP();

            }
            else if (AbsExp >= 15000 && Lvs[0] < 90)    //75 to 90
            {
                Lvs[0]++;
                AbsExp = AbsExp - AbsMax;
                AbsMax = 15000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[0].gameObject);
                Shop.Instance.GiftBoxCntUP();

            }
            else if (AbsExp >= 20000 && Lvs[0] == 99)
            {
                Lvs[0] = 100;
                AbsExp = 0;
                AbsMax = 0;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[0].gameObject);
                Shop.Instance.GiftBoxCntUP();

            }
            else if (AbsExp >= 17000 && Lvs[0] < 100)   //90 to 100
            {
                Lvs[0]++;
                AbsExp = AbsExp - AbsMax;
                AbsMax = 17000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[0].gameObject);
                Shop.Instance.GiftBoxCntUP();

            }

            GameObject text_ = Array.Find(LevelUpScreen.Instance.ScoreList, x => x.name == "Abs");
            text_.transform.GetChild(0).GetComponent<TMP_Text>().text = Lvs[0].ToString();
        }
    }
    public float AbsLV      //minus func
    {
        get { return Lvs[0]; }
        set
        {
            Lvs[0] = value;
            if (Lvs[0] < 10)      //0 to 10
            {
                AbsMax = 2000;
                AbsExp = AbsMax + AbsExp;
            }
            else if (Lvs[0] < 25)   //10 to 25
            {
                AbsMax = 5000;
                AbsExp = AbsMax + AbsExp;
            }
            else if (Lvs[0] < 75)  //25 to 75
            {
                AbsMax = 10000;
                AbsExp = AbsMax + AbsExp;
            }
            else if (Lvs[0] < 90)    //75 to 90
            {
                AbsMax = 15000;
                AbsExp = AbsMax + AbsExp;
            }
            else if (Lvs[0] == 99)
            {
                AbsMax = 20000;
                AbsExp = AbsMax + AbsExp;
            }
            else if (Lvs[0] < 100)   //90 to 100
            {
                AbsMax = 17000;
                AbsExp = AbsMax + AbsExp;
            }
            DataManager.Instance.LevelUp(QueueRoutin.Instance.Lv1[0], 0);
        }
    }

    private float ArmExp;
    public float ArmMax;
    public float ArmEXP
    {
        get { return ArmExp; }
        set
        {
            ArmExp += value;
            if (ArmExp < 0)
            {
                for (int i = 0; ArmExp < 0; i++)
                {
                    ArmLV -= 1;
                }

            }
            else if (ArmExp >= 2000 && Lvs[1] < 10)      //0 to 10
            {
                Lvs[1]++;
                ArmExp = ArmExp - ArmMax;
                ArmMax = 2000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[1].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ArmExp >= 5000 && Lvs[1] < 25)   //10 to 25
            {
                Lvs[1]++;
                ArmExp = ArmExp - ArmMax;
                ArmMax = 5000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[1].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ArmExp >= 10000 && Lvs[1] < 75)  //25 to 75
            {
                Lvs[1]++;
                ArmExp = ArmExp - ArmMax;
                ArmMax = 10000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[1].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ArmExp >= 15000 && Lvs[1] < 90)    //75 to 90
            {
                Lvs[1]++;
                ArmExp = ArmExp - ArmMax;
                ArmMax = 15000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[1].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ArmExp >= 20000 && Lvs[1] == 99)
            {
                Lvs[1] = 100;
                ArmExp = 0;
                ArmMax = 0;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[1].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ArmExp >= 17000 && Lvs[1] < 100)   //90 to 100
            {
                Lvs[1]++;
                ArmExp = ArmExp - ArmMax;
                ArmMax = 17000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[1].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            GameObject text_ = Array.Find(LevelUpScreen.Instance.ScoreList, x => x.name == "Arm");
            text_.transform.GetChild(0).GetComponent<TMP_Text>().text = Lvs[1].ToString();
        }
    }
    public float ArmLV
    {
        get { return Lvs[1]; }
        set
        {
            Lvs[1] = value;
            if (Lvs[1] < 10)      //0 to 10
            {
                ArmMax = 2000;
                ArmExp = ArmMax + ArmExp;
            }
            else if (Lvs[1] < 25)   //10 to 25
            {
                ArmMax = 5000;
                ArmExp = ArmMax + ArmExp;
            }
            else if (Lvs[1] < 75)  //25 to 75
            {
                ArmMax = 10000;
                ArmExp = ArmMax + ArmExp;
            }
            else if (Lvs[1] < 90)    //75 to 90
            {
                ArmMax = 15000;
                ArmExp = ArmMax + ArmExp;
            }
            else if (Lvs[1] == 99)
            {
                ArmMax = 20000;
                ArmExp = ArmMax + ArmExp;
            }
            else if (Lvs[1] < 100)   //90 to 100
            {
                ArmMax = 17000;
                ArmExp = ArmMax + ArmExp;
            }
            DataManager.Instance.LevelUp(QueueRoutin.Instance.Lv1[1], 0);

        }
    }

    private float BackExp;
    public float BackMax;
    public float BackEXP
    {
        get { return BackExp; }
        set
        {
            BackExp += value;
            if (BackExp < 0)
            {
                for (int i = 0; BackExp < 0; i++)
                {
                    BackLV -= 1;
                }

            }
            else if (BackExp >= 2000 && Lvs[2] < 10)      //0 to 10
            {
                Lvs[2]++;
                BackExp = BackExp - BackMax;
                BackMax = 2000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[2].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (BackExp >= 5000 && Lvs[2] < 25)   //10 to 25
            {
                Lvs[2]++;
                BackExp = BackExp - BackMax;
                BackMax = 5000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[2].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (BackExp >= 10000 && Lvs[2] < 75)  //25 to 75
            {
                Lvs[2]++;
                BackExp = BackExp - BackMax;
                BackMax = 10000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[2].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (BackExp >= 15000 && Lvs[2] < 90)    //75 to 90
            {
                Lvs[2]++;
                BackExp = BackExp - BackMax;
                BackMax = 15000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[2].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (BackExp >= 20000 && Lvs[2] == 99)
            {
                Lvs[2] = 100;
                BackExp = 0;
                BackMax = 0;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[2].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (BackExp >= 17000 && Lvs[2] < 100)   //90 to 100
            {
                Lvs[2]++;
                BackExp = BackExp - BackMax;
                BackMax = 17000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[2].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            GameObject text_ = Array.Find(LevelUpScreen.Instance.ScoreList, x => x.name == "Back");
            text_.transform.GetChild(0).GetComponent<TMP_Text>().text = Lvs[2].ToString();
        }
    }
    public float BackLV
    {
        get { return Lvs[2]; }
        set
        {
            Lvs[2] = value;
            if (Lvs[2] < 10)      //0 to 10
            {
                BackMax = 2000;
                BackExp = BackMax + BackExp;
            }
            else if (Lvs[2] < 25)   //10 to 25
            {
                BackMax = 5000;
                BackExp = BackMax + BackExp;
            }
            else if (Lvs[2] < 75)  //25 to 75
            {
                BackMax = 10000;
                BackExp = BackMax + BackExp;
            }
            else if (Lvs[2] < 90)    //75 to 90
            {
                BackMax = 15000;
                BackExp = BackMax + BackExp;
            }
            else if (Lvs[2] == 99)
            {
                BackMax = 20000;
                BackExp = BackMax + BackExp;
            }
            else if (Lvs[2] < 100)   //90 to 100
            {
                BackMax = 17000;
                BackExp = BackMax + BackExp;
            }
            DataManager.Instance.LevelUp(QueueRoutin.Instance.Lv1[2], 0);

        }
    }

    private float ChestExp;
    public float ChestMax;
    public float ChestEXP
    {
        get { return ChestExp; }
        set
        {
            ChestExp += value;
            if (ChestExp < 0)
            {
                for (int i = 0; ChestExp < 0; i++)
                {
                    ChestLV -= 1;
                }

            }
            else if (ChestExp >= 2000 && Lvs[3] < 10)      //0 to 10
            {
                Lvs[3]++;
                ChestExp = ChestExp - ChestMax;
                ChestMax = 2000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[3].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ChestExp >= 5000 && Lvs[3] < 25)   //10 to 25
            {
                Lvs[3]++;
                ChestExp = ChestExp - ChestMax;
                ChestMax = 5000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[3].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ChestExp >= 10000 && Lvs[3] < 75)  //25 to 75
            {
                Lvs[3]++;
                ChestExp = ChestExp - ChestMax;
                ChestMax = 10000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[3].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ChestExp >= 15000 && Lvs[3] < 90)    //75 to 90
            {
                Lvs[3]++;
                ChestExp = ChestExp - ChestMax;
                ChestMax = 15000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[3].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ChestExp >= 20000 && Lvs[3] == 99)
            {
                Lvs[3] = 100;
                ChestExp = 0;
                ChestMax = 0;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[3].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ChestExp >= 17000 && Lvs[3] < 100)   //90 to 100
            {
                Lvs[3]++;
                ChestExp = ChestExp - ChestMax;
                ChestMax = 17000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[3].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            GameObject text_ = Array.Find(LevelUpScreen.Instance.ScoreList, x => x.name == "Chest");
            text_.transform.GetChild(0).GetComponent<TMP_Text>().text = Lvs[3].ToString();
        }
    }
    public float ChestLV
    {
        get { return Lvs[3]; }
        set
        {
            Lvs[3] = value;
            if (Lvs[3] < 10)      //0 to 10
            {
                ChestMax = 2000;
                ChestExp = ChestMax + ChestExp;
            }
            else if (Lvs[3] < 25)   //10 to 25
            {
                ChestMax = 5000;
                ChestExp = ChestMax + ChestExp;
            }
            else if (Lvs[3] < 75)  //25 to 75
            {
                ChestMax = 10000;
                ChestExp = ChestMax + ChestExp;
            }
            else if (Lvs[3] < 90)    //75 to 90
            {
                ChestMax = 15000;
                ChestExp = ChestMax + ChestExp;
            }
            else if (Lvs[3] == 99)
            {
                ChestMax = 20000;
                ChestExp = ChestMax + ChestExp;
            }
            else if (Lvs[3] < 100)   //90 to 100
            {
                ChestMax = 17000;
                ChestExp = ChestMax + ChestExp;
            }
            DataManager.Instance.LevelUp(QueueRoutin.Instance.Lv1[3], 0);
        }
    }

    private float LegExp;
    public float LegMax;
    public float LegEXP
    {
        get { return LegExp; }
        set
        {
            LegExp += value;
            if (LegExp < 0)
            {
                for (int i = 0; LegExp < 0; i++)
                {
                    LegLV -= 1;
                }
            }
            else if (LegExp >= 2000 && Lvs[4] < 10)      //0 to 10
            {
                Lvs[4]++;
                LegExp = LegExp - LegMax;
                LegMax = 2000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[4].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (LegExp >= 5000 && Lvs[4] < 25)   //10 to 25
            {
                Lvs[4]++;
                LegExp = LegExp - LegMax;
                LegMax = 5000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[4].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (LegExp >= 10000 && Lvs[4] < 75)  //25 to 75
            {
                Lvs[4]++;
                LegExp = LegExp - LegMax;
                LegMax = 10000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[4].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (LegExp >= 15000 && Lvs[4] < 90)    //75 to 90
            {
                Lvs[4]++;
                LegExp = LegExp - LegMax;
                LegMax = 15000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[4].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (LegExp >= 20000 && Lvs[4] == 99)
            {
                Lvs[4] = 100;
                LegExp = 0;
                LegMax = 0;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[4].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (LegExp >= 17000 && Lvs[4] < 100)   //90 to 100
            {
                Lvs[4]++;
                LegExp = LegExp - LegMax;
                LegMax = 17000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[4].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            GameObject text_ = Array.Find(LevelUpScreen.Instance.ScoreList, x => x.name == "Leg");
            text_.transform.GetChild(0).GetComponent<TMP_Text>().text = Lvs[4].ToString();
        }
    }
    public float LegLV
    {
        get { return Lvs[4]; }
        set
        {
            Lvs[4] = value;
            if (Lvs[4] < 10)      //0 to 10
            {
                LegMax = 2000;
                LegExp = LegMax + LegExp;
            }
            else if (Lvs[4] < 25)   //10 to 25
            {
                LegMax = 5000;
                LegExp = LegMax + LegExp;
            }
            else if (Lvs[4] < 75)  //25 to 75
            {
                LegMax = 10000;
                LegExp = LegMax + LegExp;
            }
            else if (Lvs[4] < 90)    //75 to 90
            {
                LegMax = 15000;
                LegExp = LegMax + LegExp;
            }
            else if (Lvs[4] == 99)
            {
                LegMax = 20000;
                LegExp = LegMax + LegExp;
            }
            else if (Lvs[4] < 100)   //90 to 100
            {
                LegMax = 17000;
                LegExp = LegMax + LegExp;
            }
            DataManager.Instance.LevelUp(QueueRoutin.Instance.Lv1[4], 0);
        }
    }

    private float ShoulderExp;
    public float ShoulderMax;
    public float ShoulderEXP
    {
        get { return ShoulderExp; }
        set
        {
            ShoulderExp += value;
            if (ShoulderExp < 0)
            {
                for (int i = 0; ShoulderExp < 0; i++)
                {
                    ShoulderLV -= 1;
                }
            }
            else if (ShoulderExp >= 2000 && Lvs[5] < 10)      //0 to 10
            {
                Lvs[5]++;
                ShoulderExp = ShoulderExp - ShoulderMax;
                ShoulderMax = 2000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[5].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ShoulderExp >= 5000 && Lvs[5] < 25)   //10 to 25
            {
                Lvs[5]++;
                ShoulderExp = ShoulderExp - ShoulderMax;
                ShoulderMax = 5000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[5].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ShoulderExp >= 10000 && Lvs[5] < 75)  //25 to 75
            {
                Lvs[5]++;
                ShoulderExp = ShoulderExp - ShoulderMax;
                ShoulderMax = 10000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[5].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ShoulderExp >= 15000 && Lvs[5] < 90)    //75 to 90
            {
                Lvs[5]++;
                ShoulderExp = ShoulderExp - ShoulderMax;
                ShoulderMax = 15000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[5].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ShoulderExp >= 20000 && Lvs[5] == 99)
            {
                Lvs[5] = 100;
                ShoulderExp = 0;
                ShoulderMax = 0;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[5].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            else if (ShoulderExp >= 17000 && Lvs[5] < 100)   //90 to 100
            {
                Lvs[5]++;
                ShoulderExp = ShoulderExp - ShoulderMax;
                ShoulderMax = 17000;
                LevelUpScreen.Instance.ShowLevelUP(LevelUpScreen.Instance.fills[5].gameObject);
                Shop.Instance.GiftBoxCntUP();
            }
            GameObject text_ = Array.Find(LevelUpScreen.Instance.ScoreList, x => x.name == "Shoulder");
            text_.transform.GetChild(0).GetComponent<TMP_Text>().text = Lvs[5].ToString();
        }
    }
    public float ShoulderLV
    {
        get { return Lvs[5]; }
        set
        {
            Lvs[5] = value;
            if (Lvs[5] < 10)      //0 to 10
            {
                ShoulderMax = 2000;
                ShoulderExp = ShoulderMax + ShoulderExp;
            }
            else if (Lvs[5] < 25)   //10 to 25
            {
                ShoulderMax = 5000;
                ShoulderExp = ShoulderMax + ShoulderExp;
            }
            else if (Lvs[5] < 75)  //25 to 75
            {
                ShoulderMax = 10000;
                ShoulderExp = ShoulderMax + ShoulderExp;
            }
            else if (Lvs[5] < 90)    //75 to 90
            {
                ShoulderMax = 15000;
                ShoulderExp = ShoulderMax + ShoulderExp;
            }
            else if (Lvs[5] == 99)
            {
                ShoulderMax = 20000;
                ShoulderExp = ShoulderMax + ShoulderExp;
            }
            else if (Lvs[5] < 100)   //90 to 100
            {
                ShoulderMax = 17000;
                ShoulderExp = ShoulderMax + ShoulderExp;
            }
            DataManager.Instance.LevelUp(QueueRoutin.Instance.Lv1[5], 0);
        }
    }

    private static float DefaultMaxForLv(float lv)
    {
        if (lv < 10) return 2000;
        if (lv < 25) return 5000;
        if (lv < 75) return 10000;
        if (lv < 90) return 15000;
        if (lv == 99) return 20000;
        if (lv < 100) return 17000;
        return 0;
    }

    public PlayerSaveData ToSaveData()
    {
        var d = new PlayerSaveData();
        d.name = name;
        d.energy = int.TryParse(Energy, out var e) ? e : 0;
        d.fatigue = int.TryParse(Fatigue, out var f) ? f : 0;
        d.dayLv = int.TryParse(DAYLV, out var day) ? day : 0;

        d.item1 = Item1;
        d.item2 = Item2;
        d.item3 = Item3;

        d.lvs = new float[6] { Lvs[0], Lvs[1], Lvs[2], Lvs[3], Lvs[4], Lvs[5] };
        d.exps = new float[6] { AbsExp, ArmExp, BackExp, ChestExp, LegExp, ShoulderExp };
        d.maxExps = new float[6] { AbsMax, ArmMax, BackMax, ChestMax, LegMax, ShoulderMax };
        return d;
    }

    // SaveData 로딩은 "값 복원" 목적
    public void LoadFromSaveData(PlayerSaveData d, bool refreshUI)
    {
        if (d == null) return;

        name = d.name ?? name;
        Energy = d.energy.ToString();
        Fatigue = d.fatigue.ToString();
        DAYLV = d.dayLv.ToString();

        Item1 = d.item1 ?? "";
        Item2 = d.item2 ?? "";
        Item3 = d.item3 ?? "";

        var lvs = (d.lvs != null && d.lvs.Length >= 6) ? d.lvs : new float[6];
        for (int i = 0; i < 6; i++) Lvs[i] = lvs[i];

        var exps = (d.exps != null && d.exps.Length >= 6) ? d.exps : new float[6];
        AbsExp = exps[0];
        ArmExp = exps[1];
        BackExp = exps[2];
        ChestExp = exps[3];
        LegExp = exps[4];
        ShoulderExp = exps[5];

        var maxs = (d.maxExps != null && d.maxExps.Length >= 6) ? d.maxExps : new float[6];
        AbsMax = maxs[0] > 0 ? maxs[0] : DefaultMaxForLv(Lvs[0]);
        ArmMax = maxs[1] > 0 ? maxs[1] : DefaultMaxForLv(Lvs[1]);
        BackMax = maxs[2] > 0 ? maxs[2] : DefaultMaxForLv(Lvs[2]);
        ChestMax = maxs[3] > 0 ? maxs[3] : DefaultMaxForLv(Lvs[3]);
        LegMax = maxs[4] > 0 ? maxs[4] : DefaultMaxForLv(Lvs[4]);
        ShoulderMax = maxs[5] > 0 ? maxs[5] : DefaultMaxForLv(Lvs[5]);

        if (!refreshUI) return;
    }



    public Player(string name, string energy, string fatigue, string dAYLV, string item1, string item2, string item3, float shoulderLv,
        float chestLv, float armLv, float absLv, float backLv, float legLv)
    {
        this.name = name; Energy = energy; Fatigue = fatigue; DAYLV = dAYLV;

        Item1 = item1;
        Item2 = item2;
        Item3 = item3;

        Lvs[0] = absLv;
        Lvs[1] = armLv;
        Lvs[2] = backLv;
        Lvs[3] = chestLv;
        Lvs[4] = legLv;
        Lvs[5] = shoulderLv;

        AbsEXP = 0;
        ArmEXP = 0;
        BackEXP = 0;
        ChestEXP = 0;
        LegEXP = 0;
        ShoulderEXP = 0;


        AbsMax = 2000;
        ArmMax = 2000;
        BackMax = 2000;
        ChestMax = 2000;
        LegMax = 2000;
        ShoulderMax = 2000;

    }

}

public class DataManager : Singleton<DataManager>    //Information
{

#if UNITY_EDITOR
    [ContextMenu("Debug/Open Persistent Folder")]
    private void Ctx_OpenPersist() => UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
#endif


    public TextAsset FormatUser_DataBase; //All User data
    public List<Player> AllPlayer_Info, Player_Info;   //make into List 
    public List<Itm> Items = new List<Itm>();
    public ItemDatabaseSO itemDatabase;
    private readonly Dictionary<string, Itm> _itemByName = new Dictionary<string, Itm>();
    [System.Serializable]
    private class ItmList   //class for Parsing
    {
        public List<Itm> Items;
    }
    [System.Serializable]
    public class PlayerWrapper //class for Parsing
    {
        public List<Player> Items;
    }
    public Sprite[] muscleItem;
    public string CurPlayer;        //CUrrent Player
    private string _PersistentDataPath;


    [Serializable] private class ItmListWrap { public List<Itm> Items; }

    private enum ItemLoadStatus { MissingFile, Empty, Loaded, ParseError }

    private ItemLoadStatus LoadItemList(string fileName, List<Itm> target, bool clearTarget = false)
    {
        if (clearTarget) target.Clear();

        var path = Path.Combine(_PersistentDataPath, fileName);
        if (!File.Exists(path)) return ItemLoadStatus.MissingFile;

        var json = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(json)) return ItemLoadStatus.Empty;

        try
        {
            var s = json.TrimStart();

            // 혹시 과거 포맷 방어
            if (s.StartsWith("["))
            {
                var arr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Itm>>(json) ?? new List<Itm>();
                if (arr.Count == 0) return ItemLoadStatus.Empty;
                target.AddRange(arr);
                return ItemLoadStatus.Loaded;
            }
            else
            {
                var w = JsonUtility.FromJson<ItmListWrap>(json);
                var list = w?.Items ?? new List<Itm>();
                if (list.Count == 0) return ItemLoadStatus.Empty;
                target.AddRange(list);
                return ItemLoadStatus.Loaded;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadItemList parse error: {fileName}\n{e}\njson: {json}");
            return ItemLoadStatus.ParseError;
        }
    }


    private void SaveItemList(string fileName, List<Itm> src)
    {
        var json = JsonUtility.ToJson(new ItmListWrap { Items = src }, true);
        File.WriteAllText(Path.Combine(_PersistentDataPath, fileName), json);
    }

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

    private bool TryGetItemByName(string itemName, out Itm item)
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
    public bool IsInitialized { get; private set; }
    public bool ItemsLoaded { get; private set; }
    void Start() //private IEnumerator
    {   //전체 플레이어 리스트 불러오기
        _PersistentDataPath = Application.persistentDataPath; //edit path

#if UNITY_EDITOR
        Debug.Log("디버깅용 전체 포멧 진행");
        FormatAllPlayerData();
#endif

        Player_Info = new List<Player>();
        AllPlayer_Info = new List<Player>();


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
                Items.Add(def.ToItm());
            }
        }
        else // Fallback to JSON file
        {
            var itemTa = Resources.Load<TextAsset>("Json/Item_Info");
            if (itemTa != null)
            {
                var parsedItems = JsonUtility.FromJson<ItmList>(itemTa.text).Items;
                if (parsedItems != null)
                    foreach (var it in parsedItems)
                        Items.Add(new Itm(it.name, it.var, it.effect, it.rate, it.explain));
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
        if (File.Exists(Path.Combine(_PersistentDataPath, "AllPlayer_Info.json"))) //check file exist
        {
            LoadAllPlayerDataFromJson();
        }
        else if (allTa != null) //check Resources
        {
            File.WriteAllText(AllPlayerPath, allTa.text);
            LoadAllPlayerDataFromJson();
        }
        else //finally, create default
        {
            //default data
            AllPlayer_Info = new List<Player> {
        new Player("1","100","0","1","","","",0,0,0,0,0,0),
        new Player("2","100","0","1","","","",0,0,0,0,0,0),
        new Player("3","100","0","1","","","",0,0,0,0,0,0),
    };
            SavePlayerDataToAllPlayerJson();
        }


        //Call Main_Player Info
        var playerInfoPath = PlayerInfoPath;
        if (File.Exists(playerInfoPath)) //check file exist
        {
            LoadPlayerDataFromJson();
        }
        else //If not, create new file from AllPlayer
        {
            //Set Current Player to Player_Info
            if (string.IsNullOrEmpty(CurPlayer)) CurPlayer = "1";
            var target = AllPlayer_Info.Find(p => p.name == CurPlayer) ?? AllPlayer_Info[0];
            Player_Info = new List<Player> { target };
            SavePlayerDataToJson();
        }


        //Check itemEquipment
        var s1 = LoadItemList("CurItem_Info1.json", EquipmentScreen.Instance.ItmSlot1, clearTarget: true);
        if (s1 == ItemLoadStatus.Empty || EquipmentScreen.Instance.ItmSlot1.Count == 0)
        {
            EquipmentScreen.Instance.GetItem("스트랩", true);
        }

        var s2 = LoadItemList("CurItem_Info2.json", EquipmentScreen.Instance.ItmSlot2, clearTarget: true);
        var s3 = LoadItemList("CurItem_Info3.json", EquipmentScreen.Instance.ItmSlot3, clearTarget: true);

        //Initialize DataManager
        IsInitialized = true;
        Debug.Log("After Datamanager");
    }
    public void ChangePlayer(int newIdx)
    {
        int oldIdx = int.Parse(CurPlayer) - 1;
        if (Player_Info != null && Player_Info.Count > 0) AllPlayer_Info[oldIdx] = Player_Info[0];

        var next = AllPlayer_Info[newIdx - 1];
        if (Player_Info == null) Player_Info = new List<Player>(1);
        if (Player_Info.Count == 0) Player_Info.Add(next);
        else Player_Info[0] = next;
        // Player_Info.Clear();
        // Player_Info.Add(AllPlayer_Info[newIdx - 1]);
        CurPlayer = newIdx.ToString();

        SavePlayerDataToAllPlayerJson();
        SavePlayerDataToJson();

        PlayerCtrl[] pla = FindObjectsOfType<PlayerCtrl>();
        foreach (PlayerCtrl p1 in pla)
        {
            Destroy(p1.gameObject);
        }



        Instantiate(QueueRoutin.Instance.P2, QueueRoutin.Instance.Screen);
        Instantiate(QueueRoutin.Instance.P3, QueueRoutin.Instance.Screen);
        Instantiate(QueueRoutin.Instance.P1, QueueRoutin.Instance.Screen);

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

        for (int i = 0; i < 6; i++) LevelUp(QueueRoutin.Instance.Lv1[i], 0);

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

    /// <summary>
    /// 플레이어 리스트 직렬화
    /// 신형, 구형 둘다 진행
    /// </summary>
    /// <param name="json"> 직렬화할 json </param>
    /// <param name="usedLegacy"></param>
    /// <returns> 처리된 플레이어 리스트 </returns>
    private List<Player> DeserializePlayers(string json, ref bool usedLegacy)
    {
        usedLegacy = false;
        if (string.IsNullOrWhiteSpace(json)) return new List<Player>();

        try
        {
            var saveList = JsonConvert.DeserializeObject<List<PlayerSaveData>>(json);
            if (saveList != null && saveList.Count > 0)
            {
                var players = new List<Player>(saveList.Count);
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
            // ignore
        }

        // Legacy fallback: 과거 포맷(Player 직렬화) 호환. 
        // (다음 저장부터는 SaveData 포맷으로 자동 마이그레이션)
        try
        {
            usedLegacy = true;
            return JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
        }
        catch (Exception e)
        {
            Debug.LogError($"DeserializePlayers failed.\n{e}\njson: {json}");
            return new List<Player>();
        }
    }

    private Player PlayerFromSaveData(PlayerSaveData d)
    {
        var lvs = (d.lvs != null && d.lvs.Length >= 6) ? d.lvs : new float[6];

        var p = new Player(
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

    private string PlayerInfoPath => Path.Combine(_PersistentDataPath, "Player_Info.json");
    private string AllPlayerPath => Path.Combine(_PersistentDataPath, "AllPlayer_Info.json");

    public void SavePlayerDataToJson()    //Save Load state(player_Info -> json player_info)
    {
        var save = new List<PlayerSaveData>(Player_Info?.Count ?? 0);
        if (Player_Info != null) foreach (var p in Player_Info) save.Add(p.ToSaveData());

        var jdata = JsonConvert.SerializeObject(save, Formatting.Indented);
        File.WriteAllText(PlayerInfoPath, jdata);
    }

    public void SavePlayerDataToAllPlayerJson()    //save Load state(Allplayer_Info -> json Allplayer_info)
    {
        var save = new List<PlayerSaveData>(AllPlayer_Info?.Count ?? 0);
        if (AllPlayer_Info != null) foreach (var p in AllPlayer_Info) save.Add(p.ToSaveData());

        var jdata = JsonConvert.SerializeObject(save, Formatting.Indented);
        File.WriteAllText(AllPlayerPath, jdata);
    }

    void LoadPlayerDataFromJson()    //진행상황 불러오기(json player_info -> 게임player_info)
    {
        var jdata = File.ReadAllText(PlayerInfoPath);
        bool legacy = false;
        Player_Info = DeserializePlayers(jdata, ref legacy);
        if (legacy && !_legacyPlayerFileMigrated)
        {
            _legacyPlayerFileMigrated = true;
            SavePlayerDataToJson();
        }
    }

    void LoadAllPlayerDataFromJson()    //진행상황 불러오기(json Allplayer -> 게임 Allplayer)
    {
        var jdata = File.ReadAllText(AllPlayerPath);
        bool legacy = false;
        AllPlayer_Info = DeserializePlayers(jdata, ref legacy);
        if (legacy && !_legacyAllPlayerFileMigrated)
        {
            _legacyAllPlayerFileMigrated = true;
            SavePlayerDataToAllPlayerJson();
        }
    }

    public void SavePlayer1_Item() => SaveItemList("CurItem_Info1.json", EquipmentScreen.Instance.ItmSlot1);

    public void SavePlayer2_Item() => SaveItemList("CurItem_Info2.json", EquipmentScreen.Instance.ItmSlot2);

    public void SavePlayer3_Item() => SaveItemList("CurItem_Info3.json", EquipmentScreen.Instance.ItmSlot3);

    public void FormatAllPlayerData()
    {
        AllPlayer_Info.Clear();
        Player_Info.Clear();
        EquipmentScreen.Instance.ItmSlot1.Clear();
        EquipmentScreen.Instance.ItmSlot2.Clear();
        EquipmentScreen.Instance.ItmSlot3.Clear();

        var empty = JsonUtility.ToJson(new ItmListWrap { Items = new List<Itm>() });

        File.WriteAllText(Path.Combine(_PersistentDataPath, "CurItem_Info1.json"), empty);
        File.WriteAllText(Path.Combine(_PersistentDataPath, "CurItem_Info2.json"), empty);
        File.WriteAllText(Path.Combine(_PersistentDataPath, "CurItem_Info3.json"), empty);

        AllPlayer_Info.Add(new Player("1", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0));
        AllPlayer_Info.Add(new Player("2", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0));
        AllPlayer_Info.Add(new Player("3", "100", "0", "1", "", "", "", 0, 0, 0, 0, 0, 0));

        SavePlayerDataToAllPlayerJson();
        if (string.IsNullOrEmpty(CurPlayer)) CurPlayer = "1";
        var target = AllPlayer_Info.Find(p => p.name == CurPlayer) ?? AllPlayer_Info[0];
        Player_Info = new List<Player> { target };
        SavePlayerDataToJson();
        //need end game button
    }
}