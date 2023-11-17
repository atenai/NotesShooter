using UnityEngine;
using UnityEngine.UI;

public class LeftGunUI : UI
{
    LeftGun leftGun;

    void Start()
    {
        // オブジェクトの取得
        ImageReload_object = GameObject.Find("LeftReloadImage");
        ImageReload_object.GetComponent<RawImage>().color = ReloadColor;

        leftGun = GameObject.Find("FPSCamera").GetComponent<LeftGun>();

        //Textコンポーネント取得
        BulletNum_text = GameObject.Find("LeftBulletText").GetComponent<Text>();
        BulletNum_text.text = leftGun.leftBulletNum.ToString();
    }

    void Update()
    {
        if (leftGun.isLeftReloadTime == true)
        {
            ReloadColor.a = FadeIn(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える   
        }

        if (leftGun.isLeftReloadTime == false)
        {
            ReloadColor.a = FadeOut(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える
        }

        BulletNum_text.text = leftGun.leftBulletNum.ToString();
    }
}
