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
        isPause = isPause ? false : true;

        if (isPause == true)
        {
            Time.timeScale = 0f;
            // 一時停止
            MusicManager.SingletonInstance.AudioSource.Pause();
        }
        else
        {
            Time.timeScale = 1f;
            // 一時停止解除
            MusicManager.SingletonInstance.AudioSource.UnPause();
        }
    }
}
