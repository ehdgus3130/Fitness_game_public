using GoogleMobileAds.Api;
using UnityEngine;
using System.Collections.Concurrent;
using System;
using System.Collections;
using GoogleMobileAds.Ump.Api;

public class GoogleAds : Singleton<GoogleAds>
{
#if UNITY_ANDROID
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    string _adUnitId = "<AD_UNIT_ID>";
#else
    private string _adUnitId = "unused";
#endif
#elif UNITY_IPHONE
#if UNITY_EDITOR || DEVELOPMENT_BUILD
  private string _adUnitId = "<AD_UNIT_ID>";
#else
    private string _adUnitId = "unused";
#endif
#endif
    private static readonly ConcurrentQueue<Action> _main = new();
    static void RunOnMainThread(Action a) { if (a != null) _main.Enqueue(a); }
    void Update()
    {
        while (_main.TryDequeue(out var a)) try { a(); } catch (Exception e) { Debug.LogException(e); }
    }
    private RewardedAd rewardedAd;


    void Awake() { DontDestroyOnLoad(gameObject); }
    IEnumerator Start()
    {
        var req = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false
#if UNITY_EDITOR || DEVELOPMENT_BUILD
          ,
            ConsentDebugSettings = new ConsentDebugSettings { DebugGeography = DebugGeography.EEA, TestDeviceHashedIds = new() { "<해시>" } }
#endif
        };
        bool done = false;
        ConsentInformation.Update(req, _ => done = true); yield return new WaitUntil(() => done);
        done = false; ConsentForm.LoadAndShowConsentFormIfRequired(_ => done = true); yield return new WaitUntil(() => done);
        MobileAds.Initialize(_ => RunOnMainThread(LoadRewardedAd));
    }


    bool _show1AfterLoad, _show2AfterLoad;
    public bool IsRewardedReady =>
        rewardedAd != null && rewardedAd.CanShowAd();
    // These ad units are configured to always serve test ads.
    public void ShowRewardedAd1()
    {
        if (IsRewardedReady)
        {
            rewardedAd.Show(_ => RunOnMainThread(() => StartCoroutine(Shop.Instance.RewardOfTime1())));
        }
        else
        {
            _show1AfterLoad = true;
            LoadRewardedAd();
            RunOnMainThread(() => ShowToast("We are preparing an ad.\n Please try again in a moment."));
        }
    }

    public void ShowRewardedAd2()
    {
        if (IsRewardedReady)
        {
            rewardedAd.Show(_ => RunOnMainThread(Shop.Instance.RewardOfTime2));
        }
        else
        {
            _show2AfterLoad = true;
            LoadRewardedAd();
            RunOnMainThread(() => ShowToast("We are preparing an ad.\n Please try again in a moment."));
        }
    }
    int _backoff = 15;
    void RetryLoadWithBackoff()
    {
        CancelInvoke(nameof(LoadRewardedAd));
        Invoke(nameof(LoadRewardedAd), _backoff);
        _backoff = Mathf.Min(_backoff * 2, 300);  // 최대 5분
    }
    public void LoadRewardedAd()
    {
        if (rewardedAd != null) { rewardedAd.Destroy(); rewardedAd = null; }

        var req = new AdRequest(); // 필요 시 동의/키워드/테스트 디바이스 설정
        RewardedAd.Load(_adUnitId, req, (ad, err) =>
        {
            RunOnMainThread(() =>
            {
                if (err != null || ad == null)
                {
                    Debug.LogWarning($"[Ads] Load failed: {err?.GetMessage()} (code={err?.GetCode()})");
                    RetryLoadWithBackoff();
                    return;
                }
                rewardedAd = ad;
                RegisterReloadHandler(ad);

                if (_show1AfterLoad) { _show1AfterLoad = false; ShowRewardedAd1(); return; }
                if (_show2AfterLoad) { _show2AfterLoad = false; ShowRewardedAd2(); return; }

                Debug.Log("[Ads] Rewarded loaded & ready");
            });
        });
    }

    void RegisterReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => RunOnMainThread(LoadRewardedAd);
        ad.OnAdFullScreenContentFailed += _ => RunOnMainThread(LoadRewardedAd);
    }
    [SerializeField] CanvasGroup toastGroup;
    [SerializeField] TMPro.TMP_Text toastText;
    Coroutine _toastCo;
    public void ShowToast(string msg)
    {
        RunOnMainThread(() =>
        {
            if (!toastGroup || !toastText) { Debug.LogWarning("Toast refs missing"); return; }
            if (_toastCo != null) StopCoroutine(_toastCo);
            _toastCo = StartCoroutine(ToastRoutine(msg));
        });
    }

    IEnumerator ToastRoutine(string msg)
    {
        yield return null; // Ready next frame
        toastGroup.gameObject.SetActive(true);
        toastGroup.alpha = 1f;
        toastText.text = msg;

        // 1second wait and fade out
        yield return new WaitForSecondsRealtime(1f);
        float t = 0f, d = 2f;
        while (t < d)
        {
            t += Time.unscaledDeltaTime;
            toastGroup.alpha = 1f - t / d;
            yield return null;
        }
        toastGroup.gameObject.SetActive(false);
    }

}