using UnityEngine;

[DisallowMultipleComponent]
public class SlotIndex : MonoBehaviour
{
    public int Index;                    // LevelUpScreen.Slot[] 인덱스(0..23)
    public RectTransform ContentRoot;    // 아이템을 붙일 자리(없으면 자기 자신)
    public RectTransform GetContentRoot()
        => ContentRoot ? ContentRoot : (RectTransform)transform;
}

