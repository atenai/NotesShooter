using UnityEngine;

public class GamePlayManager : BaseSceneManager
{
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

    new void Start()
    {
        base.Start();

        countdownTimer = countdownSecond;
        isCountdownEnd = false;
        startTextTimer = 0.0f;
    }

    void Update()
    {
        FadeIn();
        UpdateCountdown();

        if (Goal.singletonInstance.IsGoal == true)
        {
            isFade = true;
        }

        FadeOut();
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
}
