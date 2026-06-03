using UnityEngine;

/// <summary>
/// RedGun（派生クラス）Gun（基底クラス）
/// </summary>
public class RedGun : IGun
{
	RedBullet bullet;

	[Header("残弾数")]
	int currentBullet;
	public int CurrentBullet => currentBullet;

	[Header("バレットSE")]
	GameObject bulletSEPrefab;
	float bulletSeEndtime = 1.0f;

	[Header("薬莢")]
	GameObject gunCartridgePrefab;
	float gunCartridgeDestroyTime = 3.0f;

	[Header("リロード")]
	[Tooltip("リロード後の弾のリセットした際の数")]
	int resetBulletNumber = 20;
	[Tooltip("リロードタイム")]
	float reloadTime = 0.0f;
	const float reloadTimeDefine = 1.0f;
	const int reloadTimeReset = 0;
	[Tooltip("リロードのオン/オフ")]
	bool isReloadTime = false;
	public bool IsReloadTime => isReloadTime;

	[Header("リロードSE")]
	GameObject reloadSEPrefab;
	float reloadSeEndtime = 1.0f;
	bool isReloadSE = false;

	public RedGun(RedBullet bullet, GameObject bulletSEPrefab, GameObject gunCartridgePrefab, GameObject reloadSEPrefab)
	{
		this.bullet = bullet;
		this.bulletSEPrefab = bulletSEPrefab;
		this.gunCartridgePrefab = gunCartridgePrefab;
		this.reloadSEPrefab = reloadSEPrefab;

		currentBullet = resetBulletNumber;//残弾数をリセット
		reloadTime = reloadTimeReset;//リロードタイムをリセット
		isReloadTime = false;//リロードのオフ
	}

	/// <summary>
	/// ショットシステム
	/// </summary>
	public void ShotSystem(GameObject gunObject)
	{
		if (currentBullet == 0)
		{
			return;
		}

		if (isReloadTime == true)
		{
			return;
		}

		currentBullet = currentBullet - 1;//残弾数を-1する

		//SEオブジェクトを生成する
		BulletSE(gunObject.transform);

		//弾オブジェクトを生成して前方向に飛ばす 
		GameObject newBullet = UnityEngine.Object.Instantiate(bullet.gameObject, gunObject.transform.position, gunObject.transform.rotation);
		newBullet.GetComponent<Rigidbody>().AddForce(gunObject.transform.forward * 7000.0f);//速すぎるとすり抜けてしまう
	}

	/// <summary>
	/// 弾発射のSE
	/// </summary>
	void BulletSE(Transform gunTransform)
	{
		GameObject BulletSE = UnityEngine.Object.Instantiate(bulletSEPrefab, gunTransform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(BulletSE, bulletSeEndtime);
	}

	/// <summary>
	/// 薬莢オブジェクトを生成して飛ばす
	/// </summary>
	void CreateGunCartridge(Transform gunTransform)
	{
		GameObject newCartridge = UnityEngine.Object.Instantiate(gunCartridgePrefab, new Vector3(gunTransform.position.x + 0.5f, gunTransform.position.y, gunTransform.position.z + 0.5f), Quaternion.identity);
		UnityEngine.Object.Destroy(newCartridge, gunCartridgeDestroyTime);
		newCartridge.GetComponent<Rigidbody>().AddForce(gunTransform.forward * 250.0f);//速すぎるとすり抜けてしまう
		newCartridge.GetComponent<Rigidbody>().AddForce(gunTransform.up * 100.0f);//速すぎるとすり抜けてしまう
		newCartridge.GetComponent<Rigidbody>().AddForce(gunTransform.right * 200.0f);//速すぎるとすり抜けてしまう
	}

	/// <summary>
	/// 自動リロードトリガー
	/// </summary>
	public void AutoReloadTrigger()
	{
		//リロード中で無く、かつ残弾数が0になったらリロード開始
		if (currentBullet == 0 && isReloadTime == false)
		{
			isReloadTime = true;//リロードのオン
			isReloadSE = true;
		}
	}

	/// <summary>
	/// 手動リロードトリガー
	/// </summary>
	public void ManualReloadTrigger()
	{
		//残弾数が満タンで無いとき、かつリロード中で無いとき
		if (currentBullet != resetBulletNumber && isReloadTime == false)
		{
			isReloadTime = true;//リロードのオン
			isReloadSE = true;
		}
	}

	/// <summary>
	/// リロードシステム
	/// </summary>
	public void ReloadSystem(GameObject gunObject)
	{
		//リロードがオンになったら
		if (isReloadTime == true)
		{
			if (isReloadSE == true)
			{
				//SEオブジェクトを生成する
				ReloadSE(gunObject.transform);
				isReloadSE = false;
			}

			reloadTime = reloadTime + Time.deltaTime;//リロードタイムをプラス
			if (reloadTimeDefine <= reloadTime)//リロードタイムがReloadTimeDefineより大きくなったら
			{
				currentBullet = resetBulletNumber;//残弾数をリセット
				reloadTime = reloadTimeReset;//リロードタイムをリセット
				isReloadTime = false;//リロードのオフ
			}
		}
	}

	/// <summary>
	/// リロードのSE
	/// </summary>
	void ReloadSE(Transform gunTransform)
	{
		GameObject ReloadSE = UnityEngine.Object.Instantiate(reloadSEPrefab, gunTransform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(ReloadSE, reloadSeEndtime);//SEをSE_Endtime後削除
	}
}
