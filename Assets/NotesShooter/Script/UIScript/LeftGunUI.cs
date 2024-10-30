
public class LeftGunUI : UI
{
    new void Start()
    {
        base.Start();
        textBulletNum.text = LeftGun.singletonInstance.bulletNum.ToString();
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

        textBulletNum.text = LeftGun.singletonInstance.bulletNum.ToString();
    }
}
