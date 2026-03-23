
public class BlueGunUI : BaseGunUI
{
	new void Start()
	{
		base.Start();
		UpdateBulletText();
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

		UpdateBulletText();
	}

	/// <summary>
	/// 残弾数テキストを更新
	/// </summary>
	void UpdateBulletText()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		textBulletWindows.text = BlueGun.SingletonInstance.CurrentBullet.ToString();
#elif UNITY_ANDROID//端末がAndroidだった場合の処理
		textBulletAndroid.text = BlueGun.SingletonInstance.CurrentBullet.ToString();
#endif//終了
	}
}
