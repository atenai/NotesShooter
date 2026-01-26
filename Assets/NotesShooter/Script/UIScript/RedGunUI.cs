
public class RedGunUI : BaseGunUI
{
	new void Start()
	{
		base.Start();
		textBullet.text = RedGun.SingletonInstance.CurrentBullet.ToString();
	}

	new void Update()
	{
		base.Update();

		if (RedGun.SingletonInstance.IsReloadTime == true)
		{
			reloadColor.a = FadeIn(reloadColor.a);

			imageReload.color = reloadColor; //画像の透明度を変える
		}

		if (RedGun.SingletonInstance.IsReloadTime == false)
		{
			reloadColor.a = FadeOut(reloadColor.a);

			imageReload.color = reloadColor; //画像の透明度を変える
		}

		textBullet.text = RedGun.SingletonInstance.CurrentBullet.ToString();
	}
}
