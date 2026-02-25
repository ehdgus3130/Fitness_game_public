using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    public Transform contentTr;
    public Slider tabSlider;
    public Image tabSliderImg;
    public RectTransform[] BtnRect, BtnImgRect;

    const int SIZE = 5; //Max Size
    float[] pos = new float[SIZE];
    float distance, targetPos, curPos;
    bool isDrag;
    int targetIndex; //Where am i?
    [SerializeField] private Sprite[] tabSliderImgs;

    void Start()
    {
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++) pos[i] = distance * i;
    }
    /// <summary>
    /// return the close space on the basic of mid distance (절반 거리를 기준으로 가까운 위치 반환)
    /// </summary>
    /// <returns></returns>
    float SetPos()
    {
        for (int i = 0; i < SIZE; i++)
        {
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        }
        return 0; //never return

    }

    public void OnBeginDrag(PointerEventData eventData) => curPos = SetPos();//Start
    public void OnDrag(PointerEventData eventData) => isDrag = true; //still Drag
    public void OnEndDrag(PointerEventData eventData)   //End
    {
        isDrag = false;

        targetPos = SetPos();

        if (curPos == targetPos)//절반거리를 넘지않아도 
        {
            if (eventData.delta.x > 18 && curPos - distance >= 0)
            {   //스크롤이 왼쪽으로 빠르게 이동시 목표 하나 감소
                --targetIndex;
                targetPos = curPos - distance;
            }
            else if (eventData.delta.x < -18 && curPos + distance <= 1.01f)
            {   //스크롤이 오른쪽으로 빠르게 이동시 목표 하나 증가
                ++targetIndex;
                targetPos = curPos + distance;
            }
        }

        for (int i = 0; i < SIZE; i++)
        {
            if (contentTr.GetChild(i).GetComponent<CustomScrollRect>() && curPos != pos[i] && targetPos == pos[i])
            {
                contentTr.GetChild(i).GetChild(1).GetComponent<Scrollbar>().value = 1;
            }
        }
    }

    void Update()
    {
        tabSlider.value = scrollbar.value; //탭슬라이더와 스크롤바 동기화

        if (!isDrag) //check the near by position and move on && btn size change
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);

            for (int i = 0; i < SIZE; i++) BtnRect[i].sizeDelta = new Vector2(i == targetIndex ? 240 : 120, BtnRect[i].sizeDelta.y);

            switch (targetIndex)
            {
                case 0: tabSliderImg.sprite = tabSliderImgs[0]; break;
                case 1: tabSliderImg.sprite = tabSliderImgs[1]; break;
                case 2: tabSliderImg.sprite = tabSliderImgs[1]; break;
                case 3: tabSliderImg.sprite = tabSliderImgs[1]; break;
                case 4: tabSliderImg.sprite = tabSliderImgs[2]; break;
            }
        }

        if (Time.time < 0.1f) return;

        for (int i = 0; i < SIZE; i++)
        {
            Vector3 BtnTargetPos = BtnRect[i].anchoredPosition3D;
            Vector3 BtnTargetScale = Vector3.one;
            bool textActive = false;

            if (i == targetIndex)
            {
                BtnTargetPos.y = -20f;
                BtnTargetScale = new Vector3(1.2f, 1.2f, 1);
                textActive = true;
            }
            BtnImgRect[i].anchoredPosition3D = Vector3.Lerp(BtnImgRect[i].anchoredPosition3D, BtnTargetPos, 0.25f);
            BtnImgRect[i].localScale = Vector3.Lerp(BtnImgRect[i].localScale, BtnTargetScale, 0.25f);
            BtnImgRect[i].transform.GetChild(0).gameObject.SetActive(textActive);
        }
    }

    /// <summary>
    /// click button OnClick function
    /// </summary>
    /// <param name="n"> which place? </param>
    public void TapClick(int n)
    {
        targetIndex = n;
        targetPos = pos[n];
    }
}
