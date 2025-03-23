using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    private static PlayerUI singletonInstance = null;
    public static PlayerUI SingletonInstance => singletonInstance;

    [Tooltip("レティクル画像")]
    [SerializeField] Image imageReticle;
    [Tooltip("クロスヘア画像")]
    [SerializeField] Image imageCrossHair;

    [Tooltip("ポーズ")]
    bool isPause = false;
    public bool IsPause => isPause;
    [Tooltip("ポーズ画像")]
    [SerializeField] GameObject panelPause;
    [SerializeField] AudioSource audioSource;

    [SerializeField] BaseGunUI redGunUI;
    public BaseGunUI RedGunUI
    {
        get { return redGunUI; }
        set { redGunUI = value; }
    }

    [SerializeField] BaseGunUI blueGunUI;
    public BaseGunUI BlueGunUI
    {
        get { return blueGunUI; }
        set { blueGunUI = value; }
    }

    [SerializeField] BaseGunUI purpleGunUI;
    public BaseGunUI PurpleGunUI
    {
        get { return purpleGunUI; }
        set { purpleGunUI = value; }
    }

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
        imageReticle.color = new Color(255.0f, 255.0f, 255.0f, 150);
        imageCrossHair.color = new Color(255.0f, 255.0f, 255.0f, 150);
    }

    void Update()
    {
        if (FPSCamera.SingletonInstance.IsTargethit == true)
        {
            imageReticle.color = new Color(255, 0, 0, 150);
            imageCrossHair.color = new Color(255, 0, 0, 150);
        }
        else
        {
            imageReticle.color = new Color(255, 255, 255, 150);
            imageCrossHair.color = new Color(255, 255, 255, 150);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
    }

    /// <summary>
    /// ポーズ
    /// </summary>
    void Pause()
    {
        isPause = isPause ? false : true;

        if (isPause == true)
        {
            Time.timeScale = 0f;
            panelPause.SetActive(true);
            // 一時停止
            audioSource.Pause();
        }
        else
        {
            Time.timeScale = 1f;
            panelPause.SetActive(false);
            // 一時停止解除
            audioSource.UnPause();
        }
    }
}
