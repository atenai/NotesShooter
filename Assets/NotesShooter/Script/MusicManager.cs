using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    static MusicManager singletonInstance = null;
    public static MusicManager SingletonInstance => singletonInstance;

    [SerializeField] AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

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

    void OnEnable()
    {
        //AudioSourceのPlay On Awakeが有効だとカウントダウン中に音楽が鳴ってしまうので、確実に止めておく
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            if (isMusicStarted == false)
            {
                audioSource.Stop();
            }
        }
    }

    [Tooltip("音楽開始からの経過時間を記録するための変数")]
    double musicStartDspTime;
    public double MusicStartDspTime => musicStartDspTime;

    bool isPaused = false;
    double pauseBeganDspTime;

    [Tooltip("カウントダウンが終わって音楽が鳴り始めたか")]
    bool isMusicStarted = false;

    /// <summary>
    /// ポーズ時間を差し引いた、現在の音楽再生時間（秒）。
    /// AudioSettings.dspTimeはTime.timeScaleやAudioSource.Pauseの影響を受けず進み続けるため、
    /// ポーズ中はこの値を止めて辻褄を合わせる。
    /// </summary>
    /// カウントダウン中はまだ音楽が始まっていないので0を返す
    public double CurrentMusicTime => (isMusicStarted == false) ? 0.0 : (isPaused ? pauseBeganDspTime : AudioSettings.dspTime) - musicStartDspTime;

    /// <summary>
    /// カウントダウンが終わった時にGameManagerから呼ばれる。ここで初めて音楽を鳴らす
    /// </summary>
    public void PlayMusic()
    {
        if (isMusicStarted == true)
        {
            return;
        }

        isMusicStarted = true;
        musicStartDspTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(musicStartDspTime);
    }

    void Update()
    {

    }

    /// <summary>
    /// GameManagerのポーズ切り替えと連動して、音楽時間の進行を止める/再開する
    /// </summary>
    public void NotifyPauseStateChanged(bool paused)
    {
        if (paused == isPaused)
        {
            return;
        }

        if (paused)
        {
            pauseBeganDspTime = AudioSettings.dspTime;
        }
        else
        {
            //ポーズしていた分だけ基準時刻を後ろにずらし、再開後もCurrentMusicTimeが連続するようにする
            musicStartDspTime += AudioSettings.dspTime - pauseBeganDspTime;
        }

        isPaused = paused;
    }
}
