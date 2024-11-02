using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(BlueGunUI・RedGunUI)
/// </summary>
public class BaseGunUI : MonoBehaviour
{
    [Tooltip("リロード画像")]
    [SerializeField] protected Image imageReload;
    protected Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    [Tooltip("フェードのスピード")]
    float fadeSpeed = 2.0f;
    [Tooltip("リロード画像の回転スピード")]
    float imageReloadRotateSpeed = -500.0f;
    [Tooltip("残段数テキスト")]
    [SerializeField] protected TextMeshProUGUI textBullet;

    protected void Start()
    {
        reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
        imageReload.color = reloadColor;
    }

    protected void Update()
    {

    }

    protected float FadeIn(float reloadColorAlpha)
    {
        //リロード画像を回転
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, imageReloadRotateSpeed * Time.deltaTime);

        if (reloadColorAlpha <= 1)
        {
            reloadColorAlpha = reloadColorAlpha + (fadeSpeed * Time.deltaTime); //アルファ値を徐々に+する
        }
        return reloadColorAlpha;
    }

    protected float FadeOut(float reloadColorAlpha)
    {
        //リロード画像を回転
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, imageReloadRotateSpeed * Time.deltaTime);

        if (reloadColorAlpha >= 0)
        {
            reloadColorAlpha = reloadColorAlpha - (fadeSpeed * Time.deltaTime); //アルファ値を徐々に-する
        }
        return reloadColorAlpha;
    }
}
