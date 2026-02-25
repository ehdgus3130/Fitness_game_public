using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoutineQueueManager : Singleton<RoutineQueueManager>
{
    private Image image_fill;

    private float timeCoolTime = 2f;            //아이템으로 조정가능
    [Header("CoolTime")]
    public float timeCurrent;
    private bool isEnded = true;     //종료여부 true = 실행 못함 / false = 실행가능

    [Header("WorkOutRoutin")]
    [SerializeField] public List<GameObject> rou = new List<GameObject>();  //리스트
    private int MAX = 5;

    public GameObject P1;
    public GameObject P2;
    public GameObject P3;

    public GameObject[] Lv1;
    public GameObject[] Lv2;
    public GameObject[] Lv3;
    public GameObject[] Lv4;

    public Button Delete;
    public Button Lock;
    private Image LockCurImg;
    public bool LOCK;      //true = 락상태/ false = 해제상태
    private int lockcnt; //lock 인경우 진행되는 idx 위치 표기
    public Sprite LockImage;
    public Sprite UnLockImage;
    public AudioSource LockSource;

    private Coroutine IsCoroutine;

    public Transform Screen;
    public int Lockcnt
    {
        get { return lockcnt; }
        set
        {
            lockcnt = value;
            int n = transform.childCount;
            if (n > 0) lockcnt %= n;
            else lockcnt = 0;
            if (lockcnt < 0) lockcnt = 0;
        }
    }

    void Awake()
    {
        lockcnt = 0;
        LOCK = false;
        LockCurImg = Lock.GetComponent<Image>();
        Lv1 = Resources.LoadAll<GameObject>("Prefabs/Button_Block/Blue");
        Lv2 = Resources.LoadAll<GameObject>("Prefabs/Button_Block/Green");
        Lv3 = Resources.LoadAll<GameObject>("Prefabs/Button_Block/Yellow");
        Lv4 = Resources.LoadAll<GameObject>("Prefabs/Button_Block/Red");
    }
    public void StartLoop()
    {
        if (IsCoroutine == null)
            IsCoroutine = StartCoroutine(StartRoutin());
    }

    public void StopLoop()
    {
        if (IsCoroutine != null)
        {
            StopCoroutine(IsCoroutine);
            IsCoroutine = null;
        }
        Set_FillAmount(0);
    }
    public void OnDelClick()
    {
        if (PlayerLevelController.Instance.TimeDay) return;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        rou.Clear();
        lockcnt = 0;
    }
    public void OnLOCKClick()
    {
        if (!(PlayerLevelController.Instance.TimeDay))     //밤일경우만 선택가능
        {
            LOCK = !LOCK;
            if (LOCK)
            {
                LockCurImg.sprite = LockImage;
            }
            else
            {
                LockCurImg.sprite = UnLockImage;
            }
            if (!Settings.Instance.IsSoundEnabled)
            {
                LockSource.Play();                  //SOUND
            }

        }
    }

    public IEnumerator StartRoutin()        //루틴상 전체 과정 
    {
        while (true)
        {
            // 전제 조건 충족까지 대기
            yield return new WaitUntil(() =>
                rou.Count > 0 &&
                PlayerLevelController.Instance.TimeDay &&
                PlayerLevelController.Instance.ene > 10 &&
                transform.childCount > 0);

            // 3초 휴식
            yield return new WaitForSeconds(3f);

            int n = transform.childCount;
            if (n == 0) break;

            int idx = LOCK ? (Lockcnt % n) : 0;
            var target = transform.GetChild(idx);
            var bar = target.childCount > 0 ? target.GetChild(0).GetComponent<Image>() : null;
            if (!bar) break;


            image_fill = bar;
            PlayerController.Instance.PlayerGet(target.name);

            // 쿨타임 진행
            Reset_CoolTime();
            while (timeCurrent < timeCoolTime)
            {
                timeCurrent += Time.deltaTime;
                Set_FillAmount(timeCurrent);
                yield return null;
            }

            End_CoolTime(); // 단일 코루틴에서만 호출

        }

    }


    public bool CheckInput()
    {
        if (rou.Count > 0)
        {
            return true;
        }
        return false;
    }

    public void CheckCoolTime()   //쿨타임 체크
    {
        timeCurrent += Time.deltaTime;
        if (timeCurrent < timeCoolTime)//쿨타임 아직 안됨
        {
            Set_FillAmount(timeCurrent);
        }
        else if (!isEnded)      //쿨타임 다됬는데 안끝남
        {
            End_CoolTime();
        }
    }

    public void End_CoolTime()  //쿨타임 종료시
    {

        Set_FillAmount(timeCoolTime);
        isEnded = true;

        int n = transform.childCount;
        if (n == 0) return;

        int idx = LOCK ? (Lockcnt % n) : 0;
        var target = transform.GetChild(idx).gameObject;

        //인벤토리 처리
        LevelUpScreen.Instance.EndRoutin(target);

        // Transform/리스트를 같은 인덱스로 제거
        if (!LOCK)
        {
            Destroy(target);
            if (rou.Count > idx) rou.RemoveAt(idx);
        }
        else
        {

            // LOCK이면 유지, 다음 잠금 슬롯으로 이동
            Lockcnt++;
        }

        PlayerLevelController.Instance.ene -= 5;
        Set_FillAmount(0);
    }

    public void Reset_CoolTime()
    {
        timeCurrent = 0;
        Set_FillAmount(0f);
        isEnded = false;
    }

    public void Set_FillAmount(float value)
    {
        if (transform.childCount != 0)
            image_fill.fillAmount = value / timeCoolTime;
        else
        {
            return;
        }
    }


    /// <summary>
    /// OnSelectClick -> PickPrefabByName -> RandomBlock
    /// Instantiate new random block and insert in rou
    /// only for selection button
    /// </summary>
    /// <param name="name"> random block name </param>
    public void OnSelectClick(string name)
    {
        if (PlayerLevelController.Instance.TimeDay) return; //Only excute at night

        var prefab = PickPrefabByName(name);
        if (prefab == null) return;

        var inst = Instantiate(prefab, transform);

        inst.transform.SetSiblingIndex(0);
        rou.Insert(0, inst);


        var img = inst.GetComponent<Image>();
        var drag = inst.GetComponent<DraggableUI>();
        if (img) img.raycastTarget = false;
        if (drag) drag.enabled = false;



        if (rou.Count > MAX) // keep max count
        {
            var last = rou[rou.Count - 1];
            if (last) Destroy(last);
            rou.RemoveAt(rou.Count - 1);
        }

    }
    public GameObject PickPrefabByName(string name)
    {
        GameObject obj;
        switch (name)
        {
            case "Pre_Shoulder":
                obj = RandomBlock(name)[5];
                return obj;
            case "Pre_Abs":
                obj = RandomBlock(name)[0];
                return obj;
            case "Pre_Leg":
                obj = RandomBlock(name)[4];
                return obj;
            case "Pre_Chest":
                obj = RandomBlock(name)[3];
                return obj;
            case "Pre_Arm":
                obj = RandomBlock(name)[1];
                return obj;
            case "Pre_Back":
                obj = RandomBlock(name)[2];
                return obj;
        }
        return null;
    }
    public GameObject[] RandomBlock(string a)   //blue1 - green2 - yellow3 - red4
    {
        int rn = Random.Range(0, 100); //1~100

        if (rn <= 8 && PlayerLevelController.Instance.fat < 10)
        {
            return Lv4;
        }
        else if (rn <= 20 && PlayerLevelController.Instance.fat < 25)
        {
            return Lv3;
        }
        else if (rn <= 50 && PlayerLevelController.Instance.fat < 50)
        {
            return Lv2;
        }
        else // (50 <= rn && rn <= 100 && PlayerLevelController.Instance.fat > 50)
        {
            return Lv1;
        }
    }
}
