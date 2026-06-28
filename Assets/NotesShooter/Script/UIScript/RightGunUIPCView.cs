using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RightGunUIPCView : MonoBehaviour
{
    [Tooltip("背景パネル")]
    [SerializeField] Image rightPanel;

    [Tooltip("リロード画像")]
    [SerializeField] Image imageReload = null;
    Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);

    [Tooltip("Windows用残段数テキスト")]
    [SerializeField] TextMeshProUGUI textBulletWindows = null;

    [Tooltip("フェードのスピード")]
    float fadeSpeed = 2.0f;
    [Tooltip("リロード画像の回転スピード")]
    float imageReloadRotateSpeed = -500.0f;

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
    /// PC用UIを非表示にする
    /// </summary>
    public void HiddenPCUI()
    {
        rightPanel.gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public void UpdateReloadImage(bool isReloadTime)
    {
        if (isReloadTime == true)
        {
            reloadColor.a = FadeIn(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
        }

        if (isReloadTime == false)
        {
            reloadColor.a = FadeOut(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
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
            reloadColorAlpha = reloadColorAlpha + (fadeSpeed * Time.deltaTime); //アルファ値を徐々に+する
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
            reloadColorAlpha = reloadColorAlpha - (fadeSpeed * Time.deltaTime); //アルファ値を徐々に-する
        }
        return reloadColorAlpha;
    }

    /// <summary>
    /// 残弾数テキストを更新
    /// </summary>
    public void UpdateBulletText(int currentBullet)
    {
        textBulletWindows.text = currentBullet.ToString();
    }
}
