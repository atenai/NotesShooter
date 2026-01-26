
public class BlueGunUI : BaseGunUI
{
	new void Start()
	{
		base.Start();
		textBullet.text = BlueGun.SingletonInstance.CurrentBullet.ToString();
	}

	new void Update()
	{
		base.Update();

		if (BlueGun.SingletonInstance.IsReloadTime == true)
		{
			reloadColor.a = FadeIn(reloadColor.a);

			imageReload.color = reloadColor; //画像の透明度を変える   
		}

		if (BlueGun.SingletonInstance.IsReloadTime == false)
		{
			reloadColor.a = FadeOut(reloadColor.a);

			imageReload.color = reloadColor; //画像の透明度を変える
		}

		textBullet.text = BlueGun.SingletonInstance.CurrentBullet.ToString();
	}
}
