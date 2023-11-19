using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(LeftGunUI・RightGunUI)
/// </summary>
public class UI : MonoBehaviour
{
    [SerializeField] protected RawImage imageReload;
    [SerializeField] protected Text textBulletNum;

    protected Color ReloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    protected int BulletNum;
    [Tooltip("フェードのスピード")]
    float fadeSpeed = 2.0f;

    void Awake()
    {
        ReloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    }

    protected void Start()
    {
        imageReload.color = ReloadColor;
    }

    protected float FadeIn(float ReloadColorAlpha)
    {
        //リロード画像を回転
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, -5.0f);

        if (ReloadColorAlpha <= 1)
        {
            ReloadColorAlpha += fadeSpeed * Time.deltaTime; //アルファ値を徐々に+する
        }
        return ReloadColorAlpha;
    }

    protected float FadeOut(float ReloadColorAlpha)
    {
        //リロード画像を回転
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, -5.0f);

        if (ReloadColorAlpha >= 0)
        {
            ReloadColorAlpha -= fadeSpeed * Time.deltaTime; //アルファ値を徐々に-する
        }
        return ReloadColorAlpha;
    }
}
