using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
	private static AdsManager singletonInstance = null;
	/// <summary>シングルトンで作成（ゲーム中に１つのみにする）</summary>
	public static AdsManager SingletonInstance => singletonInstance;

	[Tooltip("Unity DashboardのNotesShooterプロジェクトで発行したAndroid用のGame ID")]
	[SerializeField] string androidGameId = "";
	[Tooltip("Unity DashboardのNotesShooterプロジェクトで発行したiOS用のGame ID")]
	[SerializeField] string iOSGameId = "";

	private string gameId;
	[Tooltip("テストモード。本番の広告を配信する準備が整うまではtrueにしておく")]
	[SerializeField] private bool testMode = true;

	[SerializeField] AdsRewarded adsRewarded;
	public AdsRewarded AdsRewarded => adsRewarded;
	[SerializeField] AdsInterstitial adsInterstitial;
	public AdsInterstitial AdsInterstitial => adsInterstitial;
	[SerializeField] AdsBanner adsBanner;
	[Tooltip("バナー広告を出さないシーン名。プレイ中は画面を隠してしまうので出さない")]
	[SerializeField] string[] bannerHiddenSceneNames = { "MasterStage", "Stage2" };

	int adsInterstitialCount = 0;
	const int Max_AdsInterstitial_Count = 3;

	void Awake()
	{
		//staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
		if (singletonInstance == null)
		{
			singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
			DontDestroyOnLoad(this.gameObject);//シーンを切り替えた時に破棄しない
		}
		else
		{
			// 重複したインスタンスは広告を初期化せずに破棄する。
			// ここで InitializeAds() まで走ると Advertisement.Initialize が二重に呼ばれ、
			// 初期化完了コールバックも二重に返ってくるため、バナーのロードが重なって
			// 「A Banner is already in use」になる。
			// ステージを先読みするたびに新しい AdsManager が生成されるので、
			// return しないと切り替えのたびに再発する
			Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
			return;
		}

		InitializeAds();
	}

	//広告の初期化処理
	public void InitializeAds()
	{
		//iOSかAndroidのどちらのプラットフォームかを取得して広告IDを取得する
		gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? iOSGameId : androidGameId;

		//Game IDが未設定のまま初期化すると必ず失敗するので、設定されるまでは何もしない
		if (string.IsNullOrEmpty(gameId) == true)
		{
			Debug.LogWarning("Unity AdsのGame IDが未設定なので広告を初期化しません。AdsManagerのインスペクターで設定してください");
			return;
		}

		//広告の初期化処理(第一引数に広告ID, 第二引数にテストモードかどうか?, 第三引数はわからない)
		Advertisement.Initialize(gameId, testMode, this);
	}

	//初期化処理が完了した際に実行する
	public void OnInitializationComplete()
	{
		Debug.Log("Unity Ads initialization complete");
		//リワード広告をロードする
		adsRewarded.LoadAd();
		//インターステーショナル広告をロードする
		adsInterstitial.LoadAd();
		//バナー広告をロードする。表示するかどうかはロード完了後にシーンを見て決める
		adsBanner.LoadAd();
		UpdateBannerVisibility(SceneManager.GetActiveScene().name);
	}

	//初期化処理が失敗した場合に実行する
	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	/// <summary>
	/// シーンが切り替わるたびにバナー広告を出すか消すかを切り替える
	/// </summary>
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		UpdateBannerVisibility(scene.name);
	}

	void UpdateBannerVisibility(string sceneName)
	{
		if (adsBanner == null)
		{
			return;
		}

		if (IsBannerHiddenScene(sceneName) == true)
		{
			adsBanner.HideBannerAd();
		}
		else
		{
			adsBanner.ShowBannerAd();
		}
	}

	bool IsBannerHiddenScene(string sceneName)
	{
		if (bannerHiddenSceneNames == null)
		{
			return false;
		}

		for (int i = 0; i < bannerHiddenSceneNames.Length; i++)
		{
			if (bannerHiddenSceneNames[i] == sceneName)
			{
				return true;
			}
		}

		return false;
	}

	public void ShowAdsInterstitialCount()
	{
		adsInterstitialCount++;
		if (Max_AdsInterstitial_Count <= adsInterstitialCount)
		{
			adsInterstitialCount = 0;
			adsInterstitial.ShowAd();
		}
	}
}
