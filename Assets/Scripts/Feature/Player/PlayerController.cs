using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Animator anim;
    private Transform tr;
    private Rigidbody2D rb;

    public float moveSpeed = 1;
    //private GameObject Screen;

    private Transform parentTr;
    int xmove;
    int ymove;
    int velocity = 50;

    public enum Routin { Idle, Shoulder, Chest, Arm, Abs, Back, Leg }
    public Routin Today = Routin.Idle;

    private readonly int hashStop = Animator.StringToHash("STOP");
    private readonly int XM = Animator.StringToHash("Xmove");
    private readonly int YM = Animator.StringToHash("Ymove");
    private readonly int work = Animator.StringToHash("IsWork");
    private readonly int abs = Animator.StringToHash("ABS");
    private readonly int arm = Animator.StringToHash("ARM");
    private readonly int back = Animator.StringToHash("BACK");
    private readonly int chest = Animator.StringToHash("CHEST");
    private readonly int leg = Animator.StringToHash("LEG");
    private readonly int shoulder = Animator.StringToHash("SHOULDER");
    private static readonly Dictionary<string, Routin> RoutinMap =
        new Dictionary<string, Routin>
    {
        { "Shoulder", Routin.Shoulder },
        { "Chest",    Routin.Chest    },
        { "Arm",      Routin.Arm      },
        { "Abs",      Routin.Abs      },
        { "Back",     Routin.Back     },
        { "Leg",      Routin.Leg      },
    };
    const float minX = -400f, maxX = 400f;
    const float minY = -300f, maxY = 250f;
    const float cx = (maxX + minX) * 0.5f;   // 45
    const float cy = (maxY + minY) * 0.5f;   // 0
    const float a = (maxX - minX) * 0.5f;   // 275
    const float b = (maxY - minY) * 0.5f;   // 150
    void Awake()
    {
        tr = GetComponent<Transform>();
        parentTr = tr.parent;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        //Screen = GameObject.Find("GameScreen");
        //transform.position = RoutineQueueManager.Instance.Screen.position;//this.transform.parent.position;
        //Invoke("Move", 5);      //every 5seconds
        StartCoroutine(MoveLoop());
        Today = Routin.Idle;
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            Move();
            yield return new WaitForSeconds(5f);
        }
    }

    void FixedUpdate()
    {
        bool hasInput = RoutineQueueManager.Instance.CheckInput();
        bool isDay = PlayerLevelController.Instance.TimeDay;
        if (!hasInput || !isDay)
        {
            anim.SetBool(work, false);
            anim.SetBool(hashStop, false);
            Vector2 delta = new Vector2(xmove, ymove).normalized * velocity * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + delta);

            // 한 번만 계산

            Vector2 local = parentTr.InverseTransformPoint(tr.position);

            // 마름모 내부 판정 (L1 노름)
            float l1 = Mathf.Abs((local.x - cx) / a) + Mathf.Abs((local.y - cy) / b);
            bool outside = l1 > 1f;

            if (outside)
            {
                // 1) 중앙으로 되돌리기 (현재 로직 유지)
                // transform.position = parent.position;

                // 2) 혹은, 마름모 **경계선 위로** 보정하고 싶으면:
                float t = 1f / l1; // 경계까지 비율
                float px = cx + (local.x - cx) * t;
                float py = cy + (local.y - cy) * t;
                transform.position = parentTr.TransformPoint(new Vector2(px, py));
            }

        }
        else if (hasInput && isDay)
        {
            anim.SetBool(work, true);
            anim.SetBool(hashStop, true);
        }
    }
    void Move()
    {
        xmove = Random.Range(-1, 2);
        ymove = Random.Range(-1, 2);
        if (xmove > 0)
        {
            tr.localScale = new Vector3(1f, 1f, 1f);
            anim.SetBool(hashStop, false);
            anim.SetFloat(XM, xmove);
            anim.SetBool(work, false);
        }
        else if (xmove < 0)
        {
            tr.localScale = new Vector3(-1f, 1f, 1f);
            anim.SetBool(hashStop, false);
            anim.SetFloat(XM, xmove);
            anim.SetBool(work, false);
        }
        else if (xmove == 0 && (ymove > 0 || ymove < 0))
        {
            anim.SetBool(work, false);
            anim.SetBool(hashStop, false);
            anim.SetFloat(XM, ymove);
        }
        else if (ymove == 0.0f && xmove == 0.0f)
        {
            anim.SetBool(hashStop, false);  //true
            anim.SetBool(work, false);
        }
        //Invoke("Move", 5);
    }

    public void PlayerGet(string name)
    {
        anim.ResetTrigger(shoulder);
        anim.ResetTrigger(chest);
        anim.ResetTrigger(arm);
        anim.ResetTrigger(abs);
        anim.ResetTrigger(back);
        anim.ResetTrigger(leg);

        Routin routin = Routin.Idle;

        // 2-1) 먼저 정확히 일치하는 키로 시도
        if (!RoutinMap.TryGetValue(name, out routin))
        {
            // 2-2) 호환성을 위해 Contains 패턴도 한 번 더 시도 (기존 이름이 "Btn_Shoulder" 이런 식이라면)
            foreach (var kv in RoutinMap)
            {
                if (name.Contains(kv.Key))
                {
                    routin = kv.Value;
                    break;
                }
            }
        }
        Today = routin;

        switch (routin)
        {
            case Routin.Shoulder:
                anim.SetTrigger(shoulder);
                break;

            case Routin.Chest:
                anim.SetTrigger(chest);
                break;

            case Routin.Arm:
                anim.SetTrigger(arm);
                break;

            case Routin.Abs:
                anim.SetTrigger(abs);
                break;

            case Routin.Back:
                anim.SetTrigger(back);
                break;

            case Routin.Leg:
                anim.SetTrigger(leg);
                break;

            case Routin.Idle:
            default:
                // 아무 것도 안 함 (Idle 상태 유지)
                break;
        }
    }
}
