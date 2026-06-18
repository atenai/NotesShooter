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

    [Tooltip("音楽開始からの経過時間を記録するための変数")]
    double musicStartDspTime;
    public double MusicStartDspTime => musicStartDspTime;

    void Start()
    {
        musicStartDspTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(musicStartDspTime);
    }

    void Update()
    {

    }
}
