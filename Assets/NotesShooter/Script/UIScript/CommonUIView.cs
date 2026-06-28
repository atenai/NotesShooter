using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommonUIView : MonoBehaviour
{
    [Tooltip("レティクル画像")]
    [SerializeField] Image imageReticle;
    [Tooltip("クロスヘア画像")]
    [SerializeField] Image imageCrossHair;
    [Tooltip("ポーズ画像")]
    [SerializeField] GameObject panelPause;

    void Start()
    {
        InitReticleANdCrossHair();
    }

    void InitReticleANdCrossHair()
    {
        imageReticle.color = new Color(255.0f, 255.0f, 255.0f, 150);
        imageCrossHair.color = new Color(255.0f, 255.0f, 255.0f, 150);
    }

    void Update()
    {

    }

    public void ChangeRedColorReticleAndCrossHair(bool isRayCasthit)
    {
        if (isRayCasthit == true)
        {
            ChangeRedColorReticleAndCrossHair();
        }
        else
        {
            ChangeWhiteColorReticleAndCrossHair();
        }
    }

    void ChangeRedColorReticleAndCrossHair()
    {
        imageReticle.color = new Color(255, 0, 0, 150);
        imageCrossHair.color = new Color(255, 0, 0, 150);
    }

    void ChangeWhiteColorReticleAndCrossHair()
    {
        imageReticle.color = new Color(255, 255, 255, 150);
        imageCrossHair.color = new Color(255, 255, 255, 150);
    }

    /// <summary>
    /// ポーズ
    /// </summary>
    public void Pause(bool isPause)
    {
        panelPause.SetActive(isPause);
    }
}
