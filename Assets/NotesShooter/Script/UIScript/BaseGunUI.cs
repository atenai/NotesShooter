using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(BlueGunUI・RedGunUI)
/// </summary>
public class BaseGunUI : MonoBehaviour
{
    [Tooltip("リロード画像")]
    [SerializeField] protected Image imageReload = null;
    protected Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    [Tooltip("フェードのスピード")]
    float fadeSpeed = 2.0f;
    [Tooltip("リロード画像の回転スピード")]
    float imageReloadRotateSpeed = -500.0f;
    [Tooltip("残段数テキスト")]
    [SerializeField] protected TextMeshProUGUI textBullet = null;
    [Tooltip("ヒットレティクル")]
    [SerializeField] protected Image hitReticule;
    [Tooltip("ヒットレティクルが消失するスピード")]
    float hitReticuleSpeed = 10.0f;
    [Tooltip("ヒットレティクルを表示するか？？")]
    bool isHitReticule = false;
    public bool IsHitReticule
    {
        get { return isHitReticule; }
        set { isHitReticule = value; }
    }
    [Tooltip("ヒットレティクルカラー")]
    [SerializeField] protected Color color;

    protected void Start()
    {
        reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
        if (imageReload != null)
        {
            imageReload.color = reloadColor;
        }
    }

    protected void Update()
    {
        UpdateHitReticule();
    }

    protected float FadeIn(float reloadColorAlpha)
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

    protected float FadeOut(float reloadColorAlpha)
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
    /// ヒットレティクル
    /// </summary> 
    protected void UpdateHitReticule()
    {
        if (isHitReticule == true)
        {
            hitReticule.color = color;
        }

        if (isHitReticule == false)
        {
            hitReticule.color = Color.Lerp(hitReticule.color, Color.clear, Time.deltaTime * hitReticuleSpeed);
        }

        isHitReticule = false;
    }
}
