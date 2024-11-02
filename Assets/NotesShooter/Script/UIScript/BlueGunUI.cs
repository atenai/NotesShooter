
public class BlueGunUI : BaseGunUI
{
    new void Start()
    {
        base.Start();
        textBullet.text = LeftGun.singletonInstance.bulletNum.ToString();
    }

    new void Update()
    {
        base.Update();

        if (LeftGun.singletonInstance.isReloadTime == true)
        {
            reloadColor.a = FadeIn(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える   
        }

        if (LeftGun.singletonInstance.isReloadTime == false)
        {
            reloadColor.a = FadeOut(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
        }

        textBullet.text = LeftGun.singletonInstance.bulletNum.ToString();
    }
}
