using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(LeftGunUI・RightGunUI)
/// </summary>
public class UI : MonoBehaviour
{
    float fadeSpeed = 1.0f; //フェードのスピード
    protected Color ReloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    public GameObject ImageReload_object;// オブジェクトの取得

    public Text BulletNum_text;
    protected int BulletNum;

    void Awake()
    {
        ReloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    }

    protected float FadeIn(float ReloadColorAlpha)
    {
        //リロード画像を回転
        ImageReload_object.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, -5.0f);

        if (ReloadColorAlpha <= 1)
        {
            ReloadColorAlpha += fadeSpeed * Time.deltaTime; //アルファ値を徐々に＋する
        }
        return ReloadColorAlpha;
    }

    protected float FadeOut(float ReloadColorAlpha)
    {
        //リロード画像を回転
        ImageReload_object.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, -5.0f);

        if (ReloadColorAlpha >= 0)
        {
            ReloadColorAlpha -= fadeSpeed * Time.deltaTime; //アルファ値を徐々に-する
        }
        return ReloadColorAlpha;
    }
}
