
public class RightGunUI : UI
{
    new void Start()
    {
        base.Start();
        textBulletNum.text = RightGun.singletonInstance.bulletNum.ToString();
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

        textBulletNum.text = RightGun.singletonInstance.bulletNum.ToString();
    }
}
