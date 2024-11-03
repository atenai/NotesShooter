using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
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
