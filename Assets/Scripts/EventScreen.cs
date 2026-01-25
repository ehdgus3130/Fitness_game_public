using UnityEngine;

public class EventScreen : MonoBehaviour
{
    public void OnClickReset()
    {
        DataManager.Instance.FormatAllPlayerData();
        Application.Quit();
    }
}
