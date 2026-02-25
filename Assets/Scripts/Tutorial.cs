using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Button a1;
    public Button a2_back;
    public Button a2_go;
    public Button a3_back;
    public Button a3_go;
    public Button a4;
    public Button end;

    public Scrollbar scroll;

    [SerializeField]
    private DialogueManager dialogueManager;

    public void Awake()
    {
        Transform canvas = PlayerLevelController.Instance.transform.parent;
        this.transform.SetParent(canvas.transform, false);
        scroll.value = 0;
    }

    public void Start()
    {
        if (scroll.value != 0) scroll.value = 0;
        a1.onClick.AddListener(plusScrollbarValue);
        a2_back.onClick.AddListener(minusScrollbarValue);
        a2_go.onClick.AddListener(plusScrollbarValue);
        a3_back.onClick.AddListener(minusScrollbarValue);
        a3_go.onClick.AddListener(plusScrollbarValue);
        a4.onClick.AddListener(minusScrollbarValue);
        end.onClick.AddListener(OnclickEND);
    }

    private void plusScrollbarValue()
    {
        scroll.value += 0.333f;
    }
    private void minusScrollbarValue()
    {
        scroll.value -= 0.333f;
    }

    private void OnclickEND() => Destroy(this.gameObject);
}
