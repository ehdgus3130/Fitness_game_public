using UnityEngine;
using UnityEngine.UI;

public class Settings : Singleton<Settings>
{
    [Header("Sound")]
    public Image[] Sound;
    [SerializeField] private Sprite SoundOn;
    [SerializeField] private Sprite SoundOff;
    private bool IsSoundClicked = true;
    public bool IsSOUNDClicked
    {
        get { return IsSoundClicked; }
    }

    [Header("BackSound")]
    public Image[] BackSound;
    [SerializeField] private Sprite BackSoundOn;
    [SerializeField] private Sprite BackSoundOff;
    private bool IsBackShoundClicked = true;
    public bool IsBACKSOUNDClicked
    {
        get { return IsBackShoundClicked; }
    }
    private AudioSource BGM;

    [Header("Vibration")]
    public Image[] Vibration;
    [SerializeField] private Sprite VibrationOn;
    [SerializeField] private Sprite VibrationOff;
    private bool IsVibrationClicked = true;
    public bool IsVIBRATIONClicked
    {
        get { return IsVibrationClicked; }
    }
    [Header("Reset")]
    public GameObject request;
    [Header("Tutorial")]
    public GameObject tutorial;
    [Header("Link")]
    private string LinkUrl;

    [SerializeField] private Sprite L_GreenImg;
    [SerializeField] private Sprite L_RedImg;
    [SerializeField] private Sprite R_GreenImg;
    [SerializeField] private Sprite R_RedImg;
    void Awake()
    {
        LinkUrl = "https://github.com/ehdgus3130";
    }
    void Start()
    {
        GameObject bgm = GameObject.Find("BGM");
        BGM = bgm.GetComponent<AudioSource>();

    }
    public void OnClickBackSound()
    {
        IsBackShoundClicked = !IsBackShoundClicked;
        Sprite selected = IsBackShoundClicked ? BackSoundOn : BackSoundOff;
        BackSound[0].sprite = selected;
        BGM.mute = IsBackShoundClicked ? false : true;
        if (IsBackShoundClicked)
        {
            BackSound[1].sprite = L_GreenImg;
            BackSound[2].sprite = R_RedImg;
        }
        else
        {
            BackSound[1].sprite = L_RedImg;
            BackSound[2].sprite = R_GreenImg;
        }
    }

    public void OnClickSound()
    {
        IsSoundClicked = !IsSoundClicked;
        Sprite selected = IsSoundClicked ? SoundOn : SoundOff;
        Sound[0].sprite = selected;
        if (IsSoundClicked)
        {
            Sound[1].sprite = L_GreenImg;
            Sound[2].sprite = R_RedImg;
        }
        else
        {
            Sound[1].sprite = L_RedImg;
            Sound[2].sprite = R_GreenImg;
        }
    }
    public void OnClickVibration()
    {
        IsVibrationClicked = !IsVibrationClicked;
        Sprite selected = IsVibrationClicked ? VibrationOn : VibrationOff;
        Vibration[0].sprite = selected;
        if (IsVIBRATIONClicked)
        {
            Vibration[1].sprite = L_GreenImg;
            Vibration[2].sprite = R_RedImg;
        }
        else
        {
            Vibration[1].sprite = L_RedImg;
            Vibration[2].sprite = R_GreenImg;
        }
    }
    public void OnClickTutorial() => tutorial.SetActive(true);
    public void OnClickLink() => Application.OpenURL(LinkUrl);
    public void OnClickGameOver() => Application.Quit();
    public void OnClickReset() { DataManager.Instance.FormatAllPlayerData(); Application.Quit(); }
}
