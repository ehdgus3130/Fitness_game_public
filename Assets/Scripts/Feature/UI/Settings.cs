using UnityEngine;
using UnityEngine.UI;

public class Settings : Singleton<Settings>
{
    [Header("Sound")]
    public Image[] Sound;
    [SerializeField] private Sprite SoundOn;
    [SerializeField] private Sprite SoundOff;
    private bool _isSoundEnabled = true;
    public bool IsSoundEnabled
    {
        get { return _isSoundEnabled; }
    }

    [Header("BackSound")]
    public Image[] BackSound;
    [SerializeField] private Sprite BackSoundOn;
    [SerializeField] private Sprite BackSoundOff;
    private bool _isBgmEnabled = true;
    public bool IsBgmEnabled
    {
        get { return _isBgmEnabled; }
    }
    private AudioSource _bgm;

    [Header("Vibration")]
    public Image[] Vibration;
    [SerializeField] private Sprite VibrationOn;
    [SerializeField] private Sprite VibrationOff;
    private bool _isVibrationEnabled = true;
    public bool IsVibrationEnabled
    {
        get { return _isVibrationEnabled; }
    }
    [Header("Reset")]
    public GameObject request;
    [Header("Tutorial")]
    public GameObject tutorial;
    [Header("Link")]
    private string _linkUrl;

    [SerializeField] private Sprite L_GreenImg;
    [SerializeField] private Sprite L_RedImg;
    [SerializeField] private Sprite R_GreenImg;
    [SerializeField] private Sprite R_RedImg;
    void Awake()
    {
        _linkUrl = "https://github.com/ehdgus3130";
    }
    void Start()
    {
        GameObject bgm = GameObject.Find("BGM");
        _bgm = bgm.GetComponent<AudioSource>();

    }
    public void OnClickBackSound()
    {
        _isBgmEnabled = !_isBgmEnabled;
        Sprite selected = _isBgmEnabled ? BackSoundOn : BackSoundOff;
        BackSound[0].sprite = selected;
        _bgm.mute = !_isBgmEnabled;
        if (_isBgmEnabled)
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
        _isSoundEnabled = !_isSoundEnabled;
        Sprite selected = _isSoundEnabled ? SoundOn : SoundOff;
        Sound[0].sprite = selected;
        if (_isSoundEnabled)
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
        _isVibrationEnabled = !_isVibrationEnabled;
        Sprite selected = _isVibrationEnabled ? VibrationOn : VibrationOff;
        Vibration[0].sprite = selected;
        if (_isVibrationEnabled)
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
    public void OnClickLink() => Application.OpenURL(_linkUrl);
    public void OnClickGameOver() => Application.Quit();
    public void OnClickReset() { DataManager.Instance.FormatAllPlayerData(); Application.Quit(); }
}
