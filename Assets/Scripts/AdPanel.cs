using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
public class AdPanel : MonoBehaviour
{
    [SerializeField] private Button OkBtn;
    [SerializeField] private TMP_Text ExplainText;


    public void ShowAdConfirmation(string message, Action onConfirm)
    {

        // 메시지 텍스트
        ExplainText.text = message;

        OkBtn.onClick.AddListener(() =>
        {


            if (!GoogleAds.Instance.IsRewardedReady)
            {
                GoogleAds.Instance.LoadRewardedAd();
                GoogleAds.Instance.ShowToast("We are preparing an ad.\n Please try again in a moment.");
                return;
            }
            this.gameObject.SetActive(false);
            // 준비됐으면 광고 실행
            onConfirm?.Invoke();

        });

    }
}
