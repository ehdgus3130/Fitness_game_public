using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private const string FirstPlayKey = "FirstPlay";
    [SerializeField] private RectTransform uiRootCanvas;
    [SerializeField] private GameObject Tutorial;

    void Start()
    {
#if UNITY_EDITOR
        //Tutorial.SetActive(true);
#endif
        if (!PlayerPrefs.HasKey(FirstPlayKey))
        {
            PlayerPrefs.SetInt(FirstPlayKey, 1);
            Tutorial.SetActive(true);
        }
        EquipmentScreen.Instance.Character1.interactable = true;
        EquipmentScreen.Instance.Character2.interactable = false;
        EquipmentScreen.Instance.Character3.interactable = false;

    }

    public void UnlockCharacter(string name)
    {
        switch (name)
        {
            case "1":
                EquipmentScreen.Instance.Character1.interactable = true;
                EquipmentScreen.Instance.Character2.transform.GetChild(0).gameObject.SetActive(false);
                EquipmentScreen.Instance.Character2.interactable = true;
                break;
            case "2":
                EquipmentScreen.Instance.Character1.interactable = true;
                EquipmentScreen.Instance.Character2.interactable = true;
                EquipmentScreen.Instance.Character2.transform.GetChild(0).gameObject.SetActive(false);
                EquipmentScreen.Instance.Character3.interactable = true;
                EquipmentScreen.Instance.Character3.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case "3":
                EquipmentScreen.Instance.Character1.interactable = true;
                EquipmentScreen.Instance.Character2.interactable = true;
                EquipmentScreen.Instance.Character2.transform.GetChild(0).gameObject.SetActive(false);
                EquipmentScreen.Instance.Character3.interactable = true;
                EquipmentScreen.Instance.Character3.transform.GetChild(0).gameObject.SetActive(false);
                break;
        }
    }

    public void EndTutorial()
    {
        Tutorial.SetActive(false);
    }
}
