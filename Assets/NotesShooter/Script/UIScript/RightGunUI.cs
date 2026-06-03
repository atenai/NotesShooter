using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右手の銃のUI
/// </summary>
public class RightGunUI : BaseGunUI
{

	new void Start()
	{
		base.Start();
	}

	new void Update()
	{
		base.Update();

		UpdateReloadImage();
		UpdateBulletText();
	}

	void UpdateReloadImage()
	{
		if (gun.IsReloadTime == true)
		{
			reloadColor.a = FadeIn(reloadColor.a);

			imageReload.color = reloadColor; //画像の透明度を変える
		}

		if (gun.IsReloadTime == false)
		{
			reloadColor.a = FadeOut(reloadColor.a);

			imageReload.color = reloadColor; //画像の透明度を変える
		}
	}

	/// <summary>
	/// 残弾数テキストを更新
	/// </summary>
	void UpdateBulletText()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		textBulletWindows.text = gun.CurrentBullet.ToString();
#elif UNITY_ANDROID//端末がAndroidだった場合の処理
		textBulletAndroid.text = gun.CurrentBullet.ToString();
#endif//終了
	}
}
