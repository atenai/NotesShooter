using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RightGunUI : UI
{
    // Start is called before the first frame update
    void Start()
    {
        // オブジェクトの取得
        ImageReload_object = GameObject.Find("RightReloadImage");
        ImageReload_object.GetComponent<RawImage>().color = ReloadColor;

        //Textコンポーネント取得
        BulletNum_text = GameObject.Find("RightBulletText").GetComponent<Text>();
        //テキストに残段数の文字をstringに変換して入力
        BulletNum_text.text = RightGun.RightBulletNum.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (RightGun.b_RightReloadTime == true)
        {
            ReloadColor.a = FadeIn(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える
        }

        if (RightGun.b_RightReloadTime == false)
        {
            ReloadColor.a = FadeOut(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える
        }

        //テキストに残段数の文字をstringに変換して入力
        BulletNum_text.text = RightGun.RightBulletNum.ToString();
    }
}
