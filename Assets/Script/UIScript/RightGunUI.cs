using UnityEngine;
using UnityEngine.UI;

public class RightGunUI : UI
{
    new void Start()
    {
        base.Start();

        textBulletNum.text = RightGun.singletonInstance.rightBulletNum.ToString();
    }

    void Update()
    {
        if (RightGun.singletonInstance.isRightReloadTime == true)
        {
            ReloadColor.a = FadeIn(ReloadColor.a);

            // 画像の透明度を変える
            imageReload.color = ReloadColor; //画像の透明度を変える
        }

        if (RightGun.singletonInstance.isRightReloadTime == false)
        {
            ReloadColor.a = FadeOut(ReloadColor.a);

            // 画像の透明度を変える
            imageReload.color = ReloadColor; //画像の透明度を変える
        }

        textBulletNum.text = RightGun.singletonInstance.rightBulletNum.ToString();
    }
}
