using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AtomicDropHandler : MonoBehaviour, IDropHandler, IPointerExitHandler, IPointerEnterHandler
{
    private const string NestedScrollTag = "NestedScrollManager";
    private Image image;
    private RectTransform rect;         //UITransform control 
    Transform drag;
    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.gray;
    }
    public void OnDrop(PointerEventData eventData)  //Drop in to Slot position
    {
        drag = eventData.pointerDrag.GetComponent<DraggableUI>().PreviousParent;

        if (eventData.pointerDrag.tag == NestedScrollTag) Debug.Log("Can't move the screen");
        else if (eventData.pointerDrag != null)        //pointerDrag = Selected Item
        {
            eventData.pointerDrag.transform.SetParent(transform);//Set item's parent as new slot position
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;//set item's position as parent's position
            Destroy(eventData.pointerDrag);

            LevelUpScreen.Instance.LevelUP_Routin(eventData.pointerDrag);   //LV exp
            LevelUpScreen.Instance.OnDragMuscle(int.Parse(drag.name.Substring(drag.name.Length - 1)));

            LevelupSound(!Settings.Instance.IsSoundEnabled);    //sound check
        }
    }

    public void OnPointerExit(PointerEventData eventData)   //Pointer is go out from slot position
    {
        image.color = Color.white;
    }

    private void LevelupSound(bool Setting)
    {
        LevelUpScreen.Instance.PlusSound.Play();
    }

}
