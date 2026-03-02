using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("데이터 & UI 연결")]
    [SerializeField] private DialogueData dialogueData; // message
    [SerializeField] private TMP_Text dialogueText;     // UI Text
    [SerializeField] private GameObject MSG_Box;
    [SerializeField] private Scrollbar PagePos;
    [SerializeField] private GameObject[] story_1;
    [SerializeField] private GameObject[] story_2;
    [SerializeField] private GameObject[] story_3;
    [SerializeField] private GameObject[] story_4;
    private int index = 0;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ScrollRect scrollRect;

    private void Awake()
    {
        for (int i = 0; i < story_1.Count(); i++)
        {
            story_1[i].SetActive(false);
        }
        for (int i = 0; i < story_2.Count(); i++)
        {
            story_2[i].SetActive(false);
        }
        for (int i = 0; i < story_3.Count(); i++)
        {
            story_3[i].SetActive(false);
        }
        for (int i = 0; i < story_4.Count(); i++)
        {
            story_4[i].SetActive(false);
        }

    }
    private void Start()
    {
        MSG_Box.SetActive(true);
        // 대화 초기 표시
        if (dialogueData.sentences.Length > 0)
        {
            dialogueText.text = dialogueData.sentences[0];
            index = 1;
        }

        PagePos.value = 0;
        scrollRect.scrollSensitivity = 0;
        scrollRect.horizontal = false;
        scrollRect.vertical = false;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }
    }
    // 버튼 혹은 클릭 이벤트에 연결할 함수
    public void NextDialogue()
    {
        if (index < dialogueData.sentences.Length)
        {
            SetPos(index);
            MakeHighlight(index);
            dialogueText.text = dialogueData.sentences[index++];
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueText.transform.parent.gameObject.SetActive(false);

        gameManager.EndTutorial();
    }


    private void SetPos(int where)
    {

        if (where == 9 || where == 14 || where == 21)
        {
            PagePos.GetComponent<Scrollbar>().value += 0.333f;
        }
    }
    private void MakeHighlight(int where)
    {
        if (where > 3 && where < 9)
        {
            story_1[where - 4].SetActive(true);
            if (where - 4 > 0) story_1[where - 5].SetActive(false);
        }
        else if (where > 9 && where < 13)
        {
            story_2[where - 10].SetActive(true);
            if (where - 10 > 0) story_2[where - 11].SetActive(false);
        }
        else if (where > 14 && where < 21)
        {
            story_3[where - 15].SetActive(true);
            if (where - 15 > 0) story_3[where - 16].SetActive(false);
        }
        else if (where > 21 && where < 25)
        {
            story_4[where - 22].SetActive(true);
            if (where - 22 > 0) story_4[where - 23].SetActive(false);
        }
    }
}
