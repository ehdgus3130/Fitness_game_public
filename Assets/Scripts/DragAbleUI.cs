using UnityEngine;
using UnityEngine.EventSystems;


public class DragAbleUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas _canvas;
    public Transform PreviousParent { get; private set; }   //프로퍼티
    private RectTransform rect;         //UI Position
    private CanvasGroup canvasGroup;    //UI detail
    public bool WasDropped { get; set; }
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        if (!_canvas) _canvas = FindFirstObjectByType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData) //드래그 시작
    {
        WasDropped = false;
        PreviousParent = transform.parent;      //처음 위치 지정

        transform.SetParent(_canvas.transform, true);            //전체화면을 부모로 지정
        transform.SetAsLastSibling();


        if (canvasGroup)
        {
            canvasGroup.alpha = 0.6f;           //드래그 중 표시
            canvasGroup.blocksRaycasts = false;
        }

    }

    public void OnDrag(PointerEventData eventData)  //드래그 중
    {
        if (!_canvas || !rect) return;
        var canvasRect = _canvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out var local))
        {
            rect.anchoredPosition = local;
        }

    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if (!WasDropped && PreviousParent) //드랍실패하는 경우
        {
            transform.SetParent(PreviousParent, true);
            var prevRoot = PreviousParent.GetComponent<SlotIndex>()?.GetContentRoot() as RectTransform
                            ?? PreviousParent as RectTransform;
            if (prevRoot) rect.anchoredPosition = Vector2.zero;

        }
        if (canvasGroup) { canvasGroup.alpha = 1.0f; canvasGroup.blocksRaycasts = true; }
    }
    public bool TryGetPrevSlotIndex(out int from)
    {
        from = -1;
        return PreviousParent &&
               PreviousParent.TryGetComponent<SlotIndex>(out var si) &&
               (from = si.Index) >= 0;
    }
}
