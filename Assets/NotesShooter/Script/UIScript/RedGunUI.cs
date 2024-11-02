
public class RedGunUI : BaseGunUI
{
    new void Start()
    {
        base.Start();
        textBullet.text = RedGun.singletonInstance.CurrentBullet.ToString();
    }

    new void Update()
    {
        base.Update();

        if (RedGun.singletonInstance.IsReloadTime == true)
        {
            reloadColor.a = FadeIn(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
        }

        if (RedGun.singletonInstance.IsReloadTime == false)
        {
            reloadColor.a = FadeOut(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
        }

        textBullet.text = RedGun.singletonInstance.CurrentBullet.ToString();
    }
}
