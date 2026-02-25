using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;         //생성되어있지않아도 스크립트로 호출가능
    [SerializeField] Image progressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene");   //비동기 방식으로 불러옴 (다른 행동 가능)
        op.allowSceneActivation = false;    //잠시 멈춤(너무 빨라서, 에셋번들 등의 불러옴)
        float timer = 0.0f;
        while (!op.isDone)  //아직 안끝남
        {
            yield return null;  //제어권 넘김(안하면 화면 갱신 안됨)
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }   
            else            //페이크 로딩
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1.0f)
                {
                    op.allowSceneActivation = true; //로딩창 진행
                    yield break;
                }
            }
        }
    }
}