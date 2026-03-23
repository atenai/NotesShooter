using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	//シングルトンで作成（ゲーム中に１つのみにする）
	private static PlayerUI singletonInstance = null;
	public static PlayerUI SingletonInstance => singletonInstance;

	[Tooltip("レティクル画像")]
	[SerializeField] Image imageReticle;
	[Tooltip("クロスヘア画像")]
	[SerializeField] Image imageCrossHair;

	[Tooltip("ポーズ")]
	bool isPause = false;
	public bool IsPause => isPause;
	[Tooltip("ポーズ画像")]
	[SerializeField] GameObject panelPause;
	[SerializeField] AudioSource audioSource;

	[SerializeField] BaseGunUI redGunUI;
	public BaseGunUI RedGunUI
	{
		get { return redGunUI; }
		set { redGunUI = value; }
	}

	[SerializeField] BaseGunUI blueGunUI;
	public BaseGunUI BlueGunUI
	{
		get { return blueGunUI; }
		set { blueGunUI = value; }
	}

	[SerializeField] BaseGunUI purpleGunUI;
	public BaseGunUI PurpleGunUI
	{
		get { return purpleGunUI; }
		set { purpleGunUI = value; }
	}

	[SerializeField] Image redPanel;
	[SerializeField] Image bluePanel;

	[SerializeField] Button redShotButton;
	public Button RedShotButton => redShotButton;
	[SerializeField] Button redReloadButton;
	public Button RedReloadButton => redReloadButton;
	[SerializeField] Button blueShotButton;
	public Button BlueShotButton => blueShotButton;
	[SerializeField] Button blueReloadButton;
	public Button BlueReloadButton => blueReloadButton;
	[SerializeField] Button pauseButton;

	[Tooltip("ジョイスティック")]
	[SerializeField] FloatingJoystick floatingJoystick;
	public FloatingJoystick FloatingJoystick => floatingJoystick;

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


	void Start()
	{
		imageReticle.color = new Color(255.0f, 255.0f, 255.0f, 150);
		imageCrossHair.color = new Color(255.0f, 255.0f, 255.0f, 150);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		redShotButton.gameObject.SetActive(false);
		redReloadButton.gameObject.SetActive(false);
		blueShotButton.gameObject.SetActive(false);
		blueReloadButton.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(false);
#elif UNITY_ANDROID//端末がAndroidだった場合の処理
		pauseButton.onClick.AddListener(Pause);
		redPanel.gameObject.SetActive(false);
		bluePanel.gameObject.SetActive(false);
#endif//終了
	}

	void Update()
	{
		if (FPSCamera.SingletonInstance.IsTargethit == true)
		{
			imageReticle.color = new Color(255, 0, 0, 150);
			imageCrossHair.color = new Color(255, 0, 0, 150);
		}
		else
		{
			imageReticle.color = new Color(255, 255, 255, 150);
			imageCrossHair.color = new Color(255, 255, 255, 150);
		}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		if (Input.GetKeyDown(KeyCode.P))
		{
			Pause();
		}
#endif//終了
	}

	/// <summary>
	/// ポーズ
	/// </summary>
	void Pause()
	{
		isPause = isPause ? false : true;

		if (isPause == true)
		{
			Time.timeScale = 0f;
			panelPause.SetActive(true);
			// 一時停止
			audioSource.Pause();
		}
		else
		{
			Time.timeScale = 1f;
			panelPause.SetActive(false);
			// 一時停止解除
			audioSource.UnPause();
		}
	}
}
