using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RightGunUISmartPhoneView : MonoBehaviour
{
    [Tooltip("リロード画像")]
    [SerializeField] Image imageReload = null;
    Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    [Tooltip("リロード中以外でもボタンのアイコンとして見える濃さ。このImageはボタンのアイコンを兼ねているので0にすると空のボタンに見えてしまう")]
    const float baseReloadAlpha = 0.55f;

    [Tooltip("Android用残段数テキスト")]
    [SerializeField] TextMeshProUGUI textBulletAndroid = null;

    [Tooltip("フェードのスピード")]
    float fadeSpeed = 2.0f;
    [Tooltip("リロード画像の回転スピード")]
    float imageReloadRotateSpeed = -500.0f;
    [Tooltip("ライトショットボタン")]
    [SerializeField] Button rightShotButton;
    public Button RightShotButton => rightShotButton;
    [Tooltip("ライトリロードボタン")]
    [SerializeField] Button rightReloadButton;
    public Button RightReloadButton => rightReloadButton;

    void Start()
    {
        InitReload();
    }

    void InitReload()
    {
        reloadColor = new Color(1.0f, 1.0f, 1.0f, baseReloadAlpha);
        if (imageReload != null)
        {
            imageReload.color = reloadColor;
            //回転したままだとアイコンが傾いて見えるので正面に戻す
            imageReload.rectTransform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// スマートフォン用UIを非表示にする
    /// </summary>
    public void HiddenSmartPhoneUI()
    {
        rightShotButton.gameObject.SetActive(false);
        rightReloadButton.gameObject.SetActive(false);
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

        //アルファ値を徐々に+する
        return Mathf.Min(reloadColorAlpha + (fadeSpeed * Time.deltaTime), 1.0f);
    }

    float FadeOut(float reloadColorAlpha)
    {
        //リロードが終わったら回転を止めて、アイコンを正面へ戻す
        if (imageReload != null)
        {
            imageReload.rectTransform.localRotation = Quaternion.RotateTowards(
                imageReload.rectTransform.localRotation, Quaternion.identity, Mathf.Abs(imageReloadRotateSpeed) * Time.deltaTime);
        }

        //アルファ値を徐々に-するが、ボタンのアイコンとして見える濃さまでで止める
        return Mathf.Max(reloadColorAlpha - (fadeSpeed * Time.deltaTime), baseReloadAlpha);
    }

    /// <summary>
    /// 残弾数テキストを更新
    /// </summary>
    public void UpdateBulletText(int currentBullet)
    {
        textBulletAndroid.text = currentBullet.ToString();
    }
}
