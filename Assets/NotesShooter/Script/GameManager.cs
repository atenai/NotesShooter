using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    static GameManager singletonInstance = null;
    public static GameManager SingletonInstance => singletonInstance;

    [Tooltip("ポーズ")]
    bool isPause = false;
    public bool IsPause => isPause;

    [Tooltip("カウントダウンが終わってプレイが始まったか")]
    bool isGameStarted = false;
    public bool IsGameStarted => isGameStarted;

    [Tooltip("ポーズを解除してから再開するまでの秒数（3,2,1と数える）")]
    const int resumeCountdownSecond = 3;
    [Tooltip("数え終わった後に「START」を出しておく秒数")]
    const float resumeStartTextTime = 0.6f;
    [Tooltip("数え終わった時に出す文字")]
    const string resumeStartText = "START";

    [Tooltip("ポーズ解除のカウントダウン中か")]
    bool isResumeCountdown = false;
    public bool IsResumeCountdown => isResumeCountdown;
    [Tooltip("カウントダウンの残り秒数")]
    float resumeCountdownTimer = 0.0f;
    [Tooltip("「START」を消すまでの残り秒数")]
    float resumeStartTextTimer = 0.0f;

    /// <summary>
    /// ポーズか再開待ちで時間が止まっているか。
    /// 止まっている間に撃たせると、弾が銃口に溜まって再開した瞬間にまとめて飛んでしまう
    /// </summary>
    public bool IsTimeStopped => isPause == true || isResumeCountdown == true;

    void Awake()
    {
        //staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
        if (singletonInstance == null)
        {
            singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
        }
        else
        {
            Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
        }
    }

    void Start()
    {

    }

    void Update()
    {
        UpdateResumeCountdown();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
#endif//終了
    }

    /// <summary>
    /// ポーズ
    /// </summary>
    public void Pause()
    {
        //カウントダウン中はポーズさせない
        if (isGameStarted == false)
        {
            return;
        }

        //再開を数えている最中は受け付けない。数え直しになってしまう
        if (isResumeCountdown == true)
        {
            return;
        }

        isPause = isPause ? false : true;

        if (isPause == true)
        {
            StopTime();
            return;
        }

        //解除してもすぐには動かさない。いきなり動き出すと構える間が無いので、
        //数えてから再開する。ポーズの画面はisPauseがfalseになった時点で閉じる
        BeginResumeCountdown();
    }

    /// <summary>
    /// 時間と音楽を止める
    /// </summary>
    void StopTime()
    {
        Time.timeScale = 0f;
        // 一時停止
        MusicManager.SingletonInstance.AudioSource.Pause();

        //dspTimeベースで的の移動を計算しているRhythmTargetMoverDSPのために、音楽時間の進行も止める
        MusicManager.SingletonInstance.NotifyPauseStateChanged(true);
    }

    /// <summary>
    /// 時間と音楽を動かす
    /// </summary>
    void ResumeTime()
    {
        Time.timeScale = 1f;
        // 一時停止解除
        MusicManager.SingletonInstance.AudioSource.UnPause();

        //止めていた分だけ音楽時間の基準をずらす。カウントダウンの分も止めていた扱いになり、
        //再開した時に的の位置が飛ばない
        MusicManager.SingletonInstance.NotifyPauseStateChanged(false);
    }

    /// <summary>
    /// ポーズ解除のカウントダウンを始める
    /// </summary>
    void BeginResumeCountdown()
    {
        isResumeCountdown = true;
        resumeCountdownTimer = resumeCountdownSecond;

        //待たせずに最初の数字を出す
        SetCountdownText(Mathf.CeilToInt(resumeCountdownTimer).ToString());
    }

    /// <summary>
    /// ポーズ解除のカウントダウンを進める。数え終わってから時間を動かす
    /// </summary>
    void UpdateResumeCountdown()
    {
        if (isResumeCountdown == false)
        {
            UpdateResumeStartText();
            return;
        }

        //時間を止めている間なので、止まらない方の時間で数える
        resumeCountdownTimer -= Time.unscaledDeltaTime;

        const float end = 0.0f;
        if (end < resumeCountdownTimer)
        {
            //残り3.0〜2.0秒の間は「3」、2.0〜1.0秒は「2」…と出す
            SetCountdownText(Mathf.CeilToInt(resumeCountdownTimer).ToString());
            return;
        }

        isResumeCountdown = false;
        resumeStartTextTimer = resumeStartTextTime;
        SetCountdownText(resumeStartText);

        ResumeTime();
    }

    /// <summary>
    /// 「START」の文字を少し出してから消す
    /// </summary>
    void UpdateResumeStartText()
    {
        const float end = 0.0f;
        if (resumeStartTextTimer <= end)
        {
            return;
        }

        resumeStartTextTimer -= Time.unscaledDeltaTime;
        if (resumeStartTextTimer <= end)
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

    /// <summary>
    /// カウントダウンが終わった時に呼ぶ。ここで初めて音楽が鳴り始め、プレイヤーと的が動き出す
    /// </summary>
    public void StartGame()
    {
        if (isGameStarted == true)
        {
            return;
        }

        isGameStarted = true;
        MusicManager.SingletonInstance.PlayMusic();
    }
}
