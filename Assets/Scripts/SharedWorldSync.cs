using UnityEngine;
using UnityEngine.UI;

public class SharedWorldSync : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scroll;                 // Horizontal ScrollRect
    public RectTransform viewport;            // Scroll View > Viewport
    public RectTransform content;             // Scroll View > Viewport > Content
    public RectTransform worldRoot;           // SharedLayer > WorldRoot
    public RectTransform midGame;             // WorldRoot > Mid_Game
    //public RectTransform character;           // WorldRoot > Character

    [Header("Page Indexes")]
    public int levelUpIndex = 1;              // LevelUp 페이지 인덱스
    public int gameIndex = 2;              // Game 페이지 인덱스 (levelUpIndex+1)

    float pageWidth;

    void Start()
    {
        // 1) 페이지 폭 = Viewport 폭
        pageWidth = viewport.rect.width;

        // 2) Mid_Game의 폭을 "두 칸"으로 설정
        float twoPagesWidth = (gameIndex - levelUpIndex + 1) * pageWidth; // 여기선 2 * pageWidth
        var size = midGame.sizeDelta;
        size.x = twoPagesWidth;
        midGame.sizeDelta = size;

        // 3) Mid_Game의 x 위치를 LevelUp의 왼쪽으로 정렬
        //    (WorldRoot 좌표계 기준: 페이지 0의 왼쪽이 x=0)
        var pos = midGame.anchoredPosition;
        pos.x = levelUpIndex * pageWidth;
        midGame.anchoredPosition = pos;
    }

    void LateUpdate()
    {
        // Content가 움직인 만큼 WorldRoot도 동일하게 이동시켜
        // => SharedLayer(마스크 밖)인데도 Content와 완벽히 겹쳐 보임
        worldRoot.anchoredPosition = content.anchoredPosition;
    }
}
