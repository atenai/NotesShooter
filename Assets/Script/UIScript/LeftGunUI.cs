using UnityEngine;
using UnityEngine.UI;

public class LeftGunUI : UI
{
    new void Start()
    {
        base.Start();
        textBulletNum.text = LeftGun.singletonInstance.leftBulletNum.ToString();
    }

    void Update()
    {
        if (LeftGun.singletonInstance.isLeftReloadTime == true)
        {
            ReloadColor.a = FadeIn(ReloadColor.a);

            imageReload.color = ReloadColor; //画像の透明度を変える   
        }

        if (LeftGun.singletonInstance.isLeftReloadTime == false)
        {
            ReloadColor.a = FadeOut(ReloadColor.a);

            imageReload.color = ReloadColor; //画像の透明度を変える
        }

        textBulletNum.text = LeftGun.singletonInstance.leftBulletNum.ToString();
    }
}
