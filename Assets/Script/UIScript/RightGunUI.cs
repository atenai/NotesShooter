
public class RightGunUI : UI
{
    new void Start()
    {
        base.Start();
        textBulletNum.text = RightGun.singletonInstance.bulletNum.ToString();
    }

    void Update()
    {
        if (RightGun.singletonInstance.isReloadTime == true)
        {
            ReloadColor.a = FadeIn(ReloadColor.a);

            imageReload.color = ReloadColor; //画像の透明度を変える
        }

        if (RightGun.singletonInstance.isReloadTime == false)
        {
            ReloadColor.a = FadeOut(ReloadColor.a);

            imageReload.color = ReloadColor; //画像の透明度を変える
        }

        textBulletNum.text = RightGun.singletonInstance.bulletNum.ToString();
    }
}
