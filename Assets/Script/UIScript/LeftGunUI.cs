using UnityEngine;
using UnityEngine.UI;

public class LeftGunUI : UI
{
    void Start()
    {
        // オブジェクトの取得
        ImageReload_object = GameObject.Find("LeftReloadImage");
        ImageReload_object.GetComponent<RawImage>().color = ReloadColor;

        //Textコンポーネント取得
        BulletNum_text = GameObject.Find("LeftBulletText").GetComponent<Text>();
        //テキストに残段数の文字をstringに変換して入力
        BulletNum_text.text = LeftGun.leftBulletNum.ToString();
    }

    void Update()
    {
        if (LeftGun.isLeftReloadTime == true)
        {
            ReloadColor.a = FadeIn(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える   
        }

        if (LeftGun.isLeftReloadTime == false)
        {
            ReloadColor.a = FadeOut(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える
        }

        //テキストに残段数の文字をstringに変換して入力
        BulletNum_text.text = LeftGun.leftBulletNum.ToString();
    }
}
