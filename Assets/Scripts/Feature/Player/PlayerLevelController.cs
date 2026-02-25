using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelController : Singleton<PlayerLevelController>
{
    [Header("Mid Game")]
    public Image NightMap;

    [Range(0.0f, 100.0f)]
    private float Energy;
    public float ene
    {
        get { return Energy; }
        set
        {
            if (TimeDay)
            {
                Set_Fill_Amount(7.0f, 7.0f);
            }
            if (Energy < 0)
            {
                Energy = 0;
            }
            else if (Energy > 100)
            {
                Energy = 100;
            }
        }
    }

    [Range(0.0f, 100.0f)]
    private float Fatigue;
    public float fat
    {
        get { return Fatigue; }
        set
        {
            if (Fatigue < 0)
            {
                Fatigue = 0;
            }
            else if (Fatigue > 100)
            {
                Fatigue = 100;
            }
        }
    }

    private int Lv;         //Lv = Day

    [Header("Time")]
    public int Timer;             //Time limit
    public float Timer_current;     //current time
    public float Timer_Day;         //Real current time
    public bool TimeDay;    //trigger
    private Image Timer_Img;

    private float hitene;
    private float hitfat;
    private float hittime;


    [Header("FillBar")]
    public Image Energy_Fill;
    public Image Fatigue_Fill;
    public TMP_Text Lv_Text;

    private Coroutine _timeLoop;

    void Awake()
    {
        Timer_Img = transform.GetChild(0).GetComponent<Image>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
        DataManager.Instance && DataManager.Instance.IsInitialized &&
        DataManager.Instance.Player_Info != null &&
        DataManager.Instance.Player_Info.Count > 0);

        TimeDay = false;
        Timer = 20;
        hitene = 0;
        hitfat = 0;
        hittime = 0;
        Lv_Text.text = Lv.ToString();
        Energy_Fill.fillAmount = Energy / 100;
        Fatigue_Fill.fillAmount = Fatigue / 100;
        Timer_Img.fillAmount = 0;
        Timer_Img.fillOrigin = (int)Image.OriginHorizontal.Left;

        SetStartFeature(DataManager.Instance.Player_Info, 0);

        SetTimeDay(false);
        NightMap.color = new Color32(40, 40, 40, 150);
    }
    void Update()
    {
        Timer_current += Time.deltaTime;
        Timer_Day += Time.deltaTime;

        // if (TimeDay)
        // {                               //Day

        //     StopCoroutine(isTimePause());
        //     StartCoroutine(isTimePass());
        // }
        // else
        // {                               //Night

        //     StopCoroutine(isTimePass());
        //     StartCoroutine(isTimePause());
        // }
    }

    private void RestartTimeLoop()
    {
        if (_timeLoop != null) { StopCoroutine(_timeLoop); _timeLoop = null; }
        _timeLoop = StartCoroutine(TimeDay ? isTimePass() : isTimePause());
    }

    void FixedUpdate()
    {
        if (TimeDay) //Day
        {
            if (Timer_Img.fillOrigin == (int)Image.OriginHorizontal.Right)
            {
                Timer_Img.fillOrigin = (int)Image.OriginHorizontal.Left;
            }
            Timer_Img.fillAmount = Timer_Day / Timer;
        }
        else        //night
        {
            if (Timer_Img.fillOrigin == (int)Image.OriginHorizontal.Left)
            {
                Timer_Img.fillAmount = 1.0f;
                Timer_Img.fillOrigin = (int)Image.OriginHorizontal.Right;
            }
            Timer_Img.fillAmount = (Timer - Timer_Day) / Timer;
        }
    }
    private void SetTimeDay(bool a)
    {
        TimeDay = a;
        RestartTimeLoop();
        if (a)
        {
            RoutineQueueManager.Instance.StartLoop();
            Lv_Text.color = Color.black;
        }
        else
        {
            RoutineQueueManager.Instance.StopLoop();
            Lv_Text.color = Color.black;
        }
    }
    private void Set_Fill_Amount(float val1, float val2)
    {
        if (TimeDay == true)
        {
            Energy -= val1;
            Fatigue += val2;
            Energy_Fill.fillAmount = Energy / 100;
            Fatigue_Fill.fillAmount = Fatigue / 100;
        }
        else if (TimeDay == false)
        {
            Energy += val1;
            Fatigue -= val2;
            Energy_Fill.fillAmount = Energy / 100;
            Fatigue_Fill.fillAmount = Fatigue / 100;
        }
    }

    private IEnumerator isTimePass()
    {
        while (TimeDay)
        {
            if (Timer_Day >= Timer + hittime)
            {
                NightMap.color = new Color32(40, 40, 40, 150);
                Timer_current = Random.Range(1.0f, 9.0f);
                Timer_Day = 0;
                Lv++;
                Lv_Text.text = Lv.ToString();
                SetTimeDay(false);
                yield break;
            }
            else if (Timer_current > 5.0f)
            {
                Set_Fill_Amount(2.0f, 1.0f);
                Timer_current = 0;
            }
            yield return null;
        }
    }

    private IEnumerator isTimePause()
    {
        while (!TimeDay)
        {
            if (Timer_Day >= Timer + hittime)
            {
                Data_Day_Save();
                NightMap.color = new Color32(0, 0, 0, 0);
                Timer_Day = 0;
                Timer_current = 0;
                SetTimeDay(true);
                yield break;
            }
            else if (Timer_current > 5.0f)
            {
                Set_Fill_Amount(5.0f + hitene, 5.0f + hitfat);
                Timer_current = 0;
            }
            yield return null;
        }
    }

    public void ItemSkill(bool Isin)
    {
        float a, b;
        if (EquipmentScreen.Instance.EXPS_.TryGetValue("ene", out a))
        {
            if (Isin) hitene += a;
            else hitene -= a;
        }

        if (EquipmentScreen.Instance.EXPS_.TryGetValue("fat", out b))
        {
            if (Isin) hitfat += b;
            else hitfat -= b;
        }
        if (EquipmentScreen.Instance.EXPS_.TryGetValue("time", out b))
        {
            if (Isin) hittime += b;
            else hittime -= b;
        }
    }
    public void Data_Day_Save() //수정 필요
    {
        //DataManager.Instance.Player_Info[Idx].DAYLV = Lv.ToString();
        var dm = DataManager.Instance;
        dm.Player_Info[0].DAYLV = Lv.ToString();           // 현재 플레이어
        int idx = int.Parse(dm.CurPlayer) - 1;
        dm.AllPlayer_Info[idx].DAYLV = dm.Player_Info[0].DAYLV; // 전체 목록에도 반영
    }

    /// <summary>
    /// Set Start Feature of PlayerData 
    /// it excute once at start
    /// </summary>
    /// <param name="player"> player list data </param>
    /// <param name="num"> choosen player number </param>
    private void SetStartFeature(List<PlayerData> player, int num)
    {
        this.Energy = float.Parse(player[num].Energy);
        this.Fatigue = float.Parse(player[num].Fatigue);
        this.Lv = int.Parse(player[num].DAYLV);
    }
}

