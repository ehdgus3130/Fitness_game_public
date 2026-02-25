using UnityEngine;

public class RequestScreen : MonoBehaviour
{
    public void OnClickReset()
    {
        DataManager.Instance.FormatAllPlayerData();
        Application.Quit();
    }
}
