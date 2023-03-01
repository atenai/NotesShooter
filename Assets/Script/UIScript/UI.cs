using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//基底クラス//
//派生クラスのリスト(LeftGunUI・RightGunUI)
public class UI : MonoBehaviour
{
    public float fadeSpeed = 0.05f; //フェードのスピード
    public Color ReloadColor = new Color(255.0f, 255.0f, 255.0f);//Area1ディレクターテキストのカラー変数
    public GameObject ImageReload_object;// オブジェクトの取得

    public Text BulletNum_text;
    public int BulletNum;

    void Awake()
    {
        //称号のカラーを取得してアルファを０に初期化
        //エリア1
        ReloadColor = new Color(255.0f, 255.0f, 255.0f);
        ReloadColor.a = 0;
    }

    public float FadeIn(float ReloadColorAlpha)
    {
        //リロード画像を回転
        ImageReload_object.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, -5.0f);

        if (ReloadColorAlpha <= 1)
        {
            ReloadColorAlpha += fadeSpeed; //アルファ値を徐々に＋する
        }
        return ReloadColorAlpha;
    }

    public float FadeOut(float ReloadColorAlpha)
    {
        if (ReloadColorAlpha >= 0)
        {
            ReloadColorAlpha -= fadeSpeed; //アルファ値を徐々に＋する
        }
        return ReloadColorAlpha;
    }
}
