using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftGunUISmartPhoneView : MonoBehaviour
{
    [Tooltip("リロード画像")]
    [SerializeField] Image imageReload = null;
    Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);

    [Tooltip("Android用残段数テキスト")]
    [SerializeField] TextMeshProUGUI textBulletAndroid = null;

    [Tooltip("フェードのスピード")]
    float fadeSpeed = 2.0f;
    [Tooltip("リロード画像の回転スピード")]
    float imageReloadRotateSpeed = -500.0f;
    [Tooltip("レフトショットボタン")]
    [SerializeField] Button leftShotButton;
    public Button LeftShotButton => leftShotButton;
    [Tooltip("レフトリロードボタン")]
    [SerializeField] Button leftReloadButton;
    public Button LeftReloadButton => leftReloadButton;

    void Start()
    {
        InitReload();
    }

    void InitReload()
    {
        reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
        if (imageReload != null)
        {
            imageReload.color = reloadColor;
        }
    }

    /// <summary>
    /// スマートフォン用UIを非表示にする
    /// </summary>
    public void HiddenSmartPhoneUI()
    {
        leftShotButton.gameObject.SetActive(false);
        leftReloadButton.gameObject.SetActive(false);
    }

    public void UpdateReloadImage(bool isReloadTime)
    {
        // 画像を不透明にする
        if (isReloadTime == true)
        {
            reloadColor.a = FadeIn(reloadColor.a);
            imageReload.color = reloadColor;
        }

        // 画像を透明にする
        if (isReloadTime == false)
        {
            reloadColor.a = FadeOut(reloadColor.a);
            imageReload.color = reloadColor;
        }
    }

    float FadeIn(float reloadColorAlpha)
    {
        if (imageReload != null)
        {
            //リロード画像を回転
            imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, imageReloadRotateSpeed * Time.deltaTime);
        }

        if (reloadColorAlpha <= 1)
        {
            //アルファ値を徐々に+する
            reloadColorAlpha = reloadColorAlpha + (fadeSpeed * Time.deltaTime);
        }
        return reloadColorAlpha;
    }

    float FadeOut(float reloadColorAlpha)
    {
        if (imageReload != null)
        {
            //リロード画像を回転
            imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, imageReloadRotateSpeed * Time.deltaTime);
        }

        if (reloadColorAlpha >= 0)
        {
            //アルファ値を徐々に-する
            reloadColorAlpha = reloadColorAlpha - (fadeSpeed * Time.deltaTime);
        }
        return reloadColorAlpha;
    }

    /// <summary>
    /// 残弾数テキストを更新
    /// </summary>
    public void UpdateBulletText(int currentBullet)
    {
        textBulletAndroid.text = currentBullet.ToString();
    }
}
