
public class BlueGunUI : BaseGunUI
{
    new void Start()
    {
        base.Start();
        textBullet.text = BlueGun.singletonInstance.CurrentBullet.ToString();
    }

    new void Update()
    {
        base.Update();

        if (BlueGun.singletonInstance.IsReloadTime == true)
        {
            reloadColor.a = FadeIn(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える   
        }

        if (BlueGun.singletonInstance.IsReloadTime == false)
        {
            reloadColor.a = FadeOut(reloadColor.a);

            imageReload.color = reloadColor; //画像の透明度を変える
        }

        textBullet.text = BlueGun.singletonInstance.CurrentBullet.ToString();
    }
}
