using System;
using TMPro;
using UnityEngine;
[System.Serializable]
public class PlayerData //json for player information
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
            DataManager.Instance.LevelUp(RoutineQueueManager.Instance.Lv1[0], 0);
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
            DataManager.Instance.LevelUp(RoutineQueueManager.Instance.Lv1[1], 0);

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
            DataManager.Instance.LevelUp(RoutineQueueManager.Instance.Lv1[2], 0);

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
            DataManager.Instance.LevelUp(RoutineQueueManager.Instance.Lv1[3], 0);
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
            DataManager.Instance.LevelUp(RoutineQueueManager.Instance.Lv1[4], 0);
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
            DataManager.Instance.LevelUp(RoutineQueueManager.Instance.Lv1[5], 0);
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



    public PlayerData(string name, string energy, string fatigue, string dAYLV, string item1, string item2, string item3, float shoulderLv,
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

