using UnityEngine;
[CreateAssetMenu(fileName = "Default LevelSO", menuName = "Game/Level")]
public class LevelSO : ScriptableObject
{
    [Header("Level Properties")]
    public string levelName;
    public int levelID;
    public float completionTime;
    public int maxScore;
}
