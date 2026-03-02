using System;
using TMPro;
using UnityEngine;

public static class PlayerDataRuntimeBridge
{
    public static void NotifyMuscleLevelUp(int muscleIndex)
    {
        var levelUp = LevelUpScreen.Instance;
        if (levelUp == null || levelUp.fills == null) return;
        if (muscleIndex < 0 || muscleIndex >= levelUp.fills.Length) return;

        var fill = levelUp.fills[muscleIndex];
        if (fill != null) levelUp.ShowLevelUP(fill.gameObject);
    }

    public static void NotifyGiftBoxCountUp()
    {
        var shop = Shop.Instance;
        if (shop != null) shop.GiftBoxCntUP();
    }

    public static void NotifyScoreChanged(string muscleName, float level)
    {
        var levelUp = LevelUpScreen.Instance;
        if (levelUp == null || levelUp.ScoreList == null) return;

        var textRoot = Array.Find(levelUp.ScoreList, x => x != null && x.name == muscleName);
        if (textRoot == null || textRoot.transform.childCount == 0) return;

        var text = textRoot.transform.GetChild(0).GetComponent<TMP_Text>();
        if (text != null) text.text = level.ToString();
    }

    public static void RecalculateMuscleLevel(int muscleIndex)
    {
        var queue = RoutineQueueManager.Instance;
        var data = DataManager.Instance;
        if (queue == null || queue.Lv1 == null || data == null) return;
        if (muscleIndex < 0 || muscleIndex >= queue.Lv1.Length) return;

        var muscle = queue.Lv1[muscleIndex];
        if (muscle == null) return;
        data.LevelUp(muscle, 0);
    }
}
