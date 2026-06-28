using UnityEngine;

/// <summary>
/// BlueGun（派生クラス）Gun（基底クラス）
/// </summary>
public class BlueGun : IGun
{
	BlueBullet bullet;

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
	bool isReloadTrigger = false;
	public bool IsReloadTrigger => isReloadTrigger;

	[Header("リロードSE")]
	GameObject reloadSEPrefab;
	float reloadSeEndtime = 1.0f;
	bool isReloadSE = false;

	public BlueGun(BlueBullet bullet, GameObject bulletSEPrefab, GameObject gunCartridgePrefab, GameObject reloadSEPrefab)
	{
		this.bullet = bullet;
		this.bulletSEPrefab = bulletSEPrefab;
		this.gunCartridgePrefab = gunCartridgePrefab;
		this.reloadSEPrefab = reloadSEPrefab;

		currentBullet = resetBulletNumber;//残弾数をリセット
		reloadTime = reloadTimeReset;//リロードタイムをリセット
		isReloadTrigger = false;//リロードのオフ
	}

	/// <summary>
	/// ショットシステム
	/// </summary>
	public void ShotSystem(GameObject gunObject, GameObject shootPoint, GameObject cartridgePoint)
	{
		if (currentBullet == 0)//残弾数が0じゃないとき
		{
			return;
		}
		if (isReloadTrigger == true)
		{
			return;
		}
		currentBullet = currentBullet - 1;//残弾数を-1する

		//SEオブジェクトを生成する
		BulletSE(gunObject.transform);
		CreateBullet(shootPoint.transform);
		CreateGunCartridge(cartridgePoint.transform);
	}

	/// <summary>
	/// 弾オブジェクトを生成して前方向に飛ばす
	/// </summary>
	/// <param name="shootTransform"></param>
	void CreateBullet(Transform shootTransform)
	{
		GameObject newBullet = UnityEngine.Object.Instantiate(bullet.gameObject, shootTransform.position, shootTransform.rotation);
		newBullet.GetComponent<Rigidbody>().AddForce(shootTransform.forward * 7000.0f);//速すぎるとすり抜けてしまう
	}

	/// <summary>
	/// 薬莢オブジェクトを生成して飛ばす
	/// </summary>
	void CreateGunCartridge(Transform cartridgeTransform)
	{
		GameObject newGunCartridge = UnityEngine.Object.Instantiate(gunCartridgePrefab, new Vector3(cartridgeTransform.position.x - 0.5f, cartridgeTransform.position.y, cartridgeTransform.position.z + 0.5f), Quaternion.identity);
		UnityEngine.Object.Destroy(newGunCartridge, gunCartridgeDestroyTime);
		newGunCartridge.GetComponent<Rigidbody>().AddForce(cartridgeTransform.forward * 250.0f);//速すぎるとすり抜けてしまう
		newGunCartridge.GetComponent<Rigidbody>().AddForce(cartridgeTransform.up * 100.0f);//速すぎるとすり抜けてしまう
		newGunCartridge.GetComponent<Rigidbody>().AddForce(cartridgeTransform.right * -200.0f);//速すぎるとすり抜けてしまう
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
	/// 自動リロードトリガー
	/// </summary>
	public void AutoReloadTrigger()
	{
		//リロード中で無く、かつ残弾数が0になったらリロード開始
		if (currentBullet == 0 && isReloadTrigger == false)
		{
			isReloadTrigger = true;//リロードのオン
			isReloadSE = true;
		}
	}

	/// <summary>
	/// 手動リロードトリガー
	/// </summary>
	public void ManualReloadTrigger()
	{
		//残弾数が満タンで無いとき、かつリロード中で無いとき
		if (currentBullet != resetBulletNumber && isReloadTrigger == false)
		{
			isReloadTrigger = true;//リロードのオン
			isReloadSE = true;
		}
	}

	/// <summary>
	/// リロードシステム
	/// </summary>
	public void ReloadSystem(GameObject gunObject)
	{
		//リロードがオンになったら
		if (isReloadTrigger == true)
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
				isReloadTrigger = false;//リロードのオフ
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
