
public class RedGunUI : BaseGunUI
{
    new void Start()
    {
        base.Start();
        textBullet.text = RightGun.singletonInstance.bulletNum.ToString();
    }

    new void Update()
    {
        base.Update();

        if (RightGun.singletonInstance.isReloadTime == true)
        {
            reloadColor.a = FadeIn(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
        }

        if (RightGun.singletonInstance.isReloadTime == false)
        {
            reloadColor.a = FadeOut(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
        }

        textBullet.text = RightGun.singletonInstance.bulletNum.ToString();
    }
}
