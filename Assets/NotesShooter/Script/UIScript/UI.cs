using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(LeftGunUI・RightGunUI)
/// </summary>
public class UI : MonoBehaviour
{
    [SerializeField] protected RawImage imageReload;
    protected Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    [Tooltip("フェードのスピード")]
    float fadeSpeed = 2.0f;
    float imageReloadRotateSpeed = -500.0f;
    [SerializeField] protected Text textBulletNum;

    void Awake()
    {
        reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    }

    protected void Start()
    {
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
            reloadColorAlpha += fadeSpeed * Time.deltaTime; //アルファ値を徐々に+する
        }
        return reloadColorAlpha;
    }

    protected float FadeOut(float reloadColorAlpha)
    {
        //リロード画像を回転
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, imageReloadRotateSpeed * Time.deltaTime);

        if (reloadColorAlpha >= 0)
        {
            reloadColorAlpha -= fadeSpeed * Time.deltaTime; //アルファ値を徐々に-する
        }
        return reloadColorAlpha;
    }
}
