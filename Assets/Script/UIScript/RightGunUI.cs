using UnityEngine;
using UnityEngine.UI;

public class RightGunUI : UI
{
    RightGun rightGun;

    void Start()
    {
        // オブジェクトの取得
        ImageReload_object = GameObject.Find("RightReloadImage");
        ImageReload_object.GetComponent<RawImage>().color = ReloadColor;

        rightGun = GameObject.Find("FPSCamera").GetComponent<RightGun>();

        //Textコンポーネント取得
        BulletNum_text = GameObject.Find("RightBulletText").GetComponent<Text>();
        BulletNum_text.text = rightGun.rightBulletNum.ToString();
    }

    void Update()
    {
        if (rightGun.isRightReloadTime == true)
        {
            ReloadColor.a = FadeIn(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える
        }

        if (rightGun.isRightReloadTime == false)
        {
            ReloadColor.a = FadeOut(ReloadColor.a);

            // 画像の透明度を変える
            ImageReload_object.GetComponent<RawImage>().color = ReloadColor; //画像の透明度を変える
        }

        BulletNum_text.text = rightGun.rightBulletNum.ToString();
    }
}
