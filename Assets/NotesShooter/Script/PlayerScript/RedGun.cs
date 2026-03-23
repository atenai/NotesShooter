using UnityEngine;

/// <summary>
/// RedGun（派生クラス）Gun（基底クラス）
/// </summary>
public class RedGun : Gun
{
	//シングルトンで作成（ゲーム中に１つのみにする）
	private static RedGun singletonInstance = null;
	public static RedGun SingletonInstance => singletonInstance;

	[SerializeField] RedBullet redBullet;

	void Awake()
	{
		//staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
		if (singletonInstance == null)
		{
			singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
		}
		else
		{
			Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
		}
	}

	new void Start()
	{
		base.Start();
		PlayerUI.SingletonInstance.RedShotButton.onClick.AddListener(ShotSystem);
		PlayerUI.SingletonInstance.RedReloadButton.onClick.AddListener(ManualReloadTrigger);
	}

	void Update()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		//マウス右クリックが押されたとき
		if (Input.GetMouseButtonDown(1))
		{
			ShotSystem();
		}
#endif//終了

		AutoReloadTrigger();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		//Eキーが押されたとき
		if (Input.GetKey(KeyCode.E))
		{
			ManualReloadTrigger();
		}
#endif//終了

		ReloadSystem();
	}

	/// <summary>
	/// ショットシステム
	/// </summary>
	public void ShotSystem()
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
		BulletSE();

		//薬莢オブジェクトを生成して飛ばす
		GameObject newCartridge = Instantiate(gunCartridgePrefab, new Vector3(this.gameObject.transform.position.x + 0.5f, this.gameObject.transform.position.y, this.gameObject.transform.position.z + 0.5f), Quaternion.identity);
		Destroy(newCartridge, gunCartridgeDestroyTime);
		newCartridge.GetComponent<Rigidbody>().AddForce(this.transform.forward * 250.0f);//速すぎるとすり抜けてしまう
		newCartridge.GetComponent<Rigidbody>().AddForce(this.transform.up * 100.0f);//速すぎるとすり抜けてしまう
		newCartridge.GetComponent<Rigidbody>().AddForce(this.transform.right * 200.0f);//速すぎるとすり抜けてしまう

		//弾オブジェクトを生成して前方向に飛ばす 
		GameObject newBullet = Instantiate(redBullet.gameObject, this.transform.position, this.transform.rotation);
		newBullet.GetComponent<Rigidbody>().AddForce(this.transform.forward * 7000.0f);//速すぎるとすり抜けてしまう
	}

	/// <summary>
	/// 自動リロードトリガー
	/// </summary>
	void AutoReloadTrigger()
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
	void ReloadSystem()
	{
		//リロードがオンになったら
		if (isReloadTime == true)
		{
			if (isReloadSE == true)
			{
				//SEオブジェクトを生成する
				ReloadSE();
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
}
