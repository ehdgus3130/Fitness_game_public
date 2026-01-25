using UnityEngine;

// 에디터 메뉴에서 Create → Dialogue → DialogueData 로 에셋 생성 가능
[CreateAssetMenu(menuName = "Dialogue/DialogueData", fileName = "New DialogueData")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] sentences;    // 대사 한 줄씩 배열로 저장
}
