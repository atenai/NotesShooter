using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour, IFadeSceneManager
{
    [Header("シーン遷移")]
    [Tooltip("タイトル")]
    const string Result_SceneName = "Result";

    [Header("フェード")]
    [Tooltip("フェード用の黒画像")]
    [SerializeField] Image fadeImage;
    [Tooltip("フェードの速度")]
    float fadeSpeed = 2.5f;
    [Tooltip("フェードインのアルファ値")]
    float fadeInAlfa = 1.0f;
    [Tooltip("フェードインが終わったか？")]
    bool isFadeInEnd = false;
    [Tooltip("フェードアウトのアルファ値")]
    float fadeOutAlfa = 0.0f;
    [Tooltip("フェードアウトが始まったか？")]
    bool isFadeOutStart = false;

    [Header("カウントダウン")]
    [Tooltip("カウントダウンの秒数（3,2,1と数える）")]
    const int countdownSecond = 3;
    [Tooltip("カウントダウンの後に「START」を出しておく秒数")]
    const float startTextTime = 0.6f;
    [Tooltip("カウントダウンが終わった時に出す文字")]
    const string startText = "START";
    [Tooltip("カウントダウンの残り秒数")]
    float countdownTimer = 0.0f;
    [Tooltip("カウントダウンが終わったか")]
    bool isCountdownEnd = false;
    [Tooltip("「START」を消すまでの残り秒数")]
    float startTextTimer = 0.0f;

    void Start()
    {
        InitFade();

        countdownTimer = countdownSecond;
        isCountdownEnd = false;
        startTextTimer = 0.0f;
    }

    public void InitFade()
    {
        //フェードインの初期化
        isFadeInEnd = false;
        fadeInAlfa = 1.0f;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeInAlfa);

        //フェードアウトの初期化
        isFadeOutStart = false;
        fadeOutAlfa = 0.0f;
    }

    void Update()
    {
        FadeIn();
        UpdateCountdown();

        if (Goal.singletonInstance.IsGoal == true)
        {
            isFadeOutStart = true;
        }

        FadeOut();
    }

    public void FadeIn()
    {
        if (isFadeInEnd == true)
        {
            return;
        }

        fadeInAlfa -= fadeSpeed * Time.deltaTime;

        const float min = 0.0f;
        if (fadeInAlfa <= min)
        {
            fadeInAlfa = min;
            isFadeInEnd = true;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeInAlfa);
    }

    /// <summary>
    /// プレイ開始前のカウントダウン。フェードインが終わってから数え始める
    /// </summary>
    void UpdateCountdown()
    {
        if (isCountdownEnd == true)
        {
            UpdateStartText();
            return;
        }

        //真っ暗な間に数え始めても見えないので、フェードインが終わるまで待つ
        if (isFadeInEnd == false)
        {
            return;
        }

        countdownTimer -= Time.deltaTime;

        const float end = 0.0f;
        if (end < countdownTimer)
        {
            //残り3.0〜2.0秒の間は「3」、2.0〜1.0秒は「2」…と出す
            SetCountdownText(Mathf.CeilToInt(countdownTimer).ToString());
            return;
        }

        //ここで初めて音楽が鳴り、プレイヤーと的とカメラが動き出す
        isCountdownEnd = true;
        startTextTimer = startTextTime;
        SetCountdownText(startText);
        GameManager.SingletonInstance.StartGame();
    }

    /// <summary>
    /// 「START」の文字を少し出してから消す
    /// </summary>
    void UpdateStartText()
    {
        const float end = 0.0f;
        if (startTextTimer <= end)
        {
            return;
        }

        startTextTimer -= Time.deltaTime;
        if (startTextTimer <= end)
        {
            HideCountdownText();
        }
    }

    void SetCountdownText(string text)
    {
        if (UIPresenter.SingletonInstance == null || UIPresenter.SingletonInstance.CommonUIView == null)
        {
            return;
        }

        UIPresenter.SingletonInstance.CommonUIView.SetCountdownText(text);
    }

    void HideCountdownText()
    {
        if (UIPresenter.SingletonInstance == null || UIPresenter.SingletonInstance.CommonUIView == null)
        {
            return;
        }

        UIPresenter.SingletonInstance.CommonUIView.HideCountdown();
    }

    public void FadeOut()
    {
        if (isFadeOutStart == true)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeOutAlfa);
            fadeOutAlfa += fadeSpeed * Time.deltaTime;
        }

        const float max = 1.0f;
        if (max <= fadeOutAlfa)
        {
            isFadeOutStart = false;
            SceneChange(Result_SceneName);
        }
    }

    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
    }
}
