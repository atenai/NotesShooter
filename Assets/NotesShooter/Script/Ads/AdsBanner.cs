using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsBanner : MonoBehaviour
{
	[SerializeField] string _androidAdUnitId = "Banner_Android";
	[SerializeField] string _iOSAdUnitId = "Banner_iOS";
	string _adUnitId = null;

	[Tooltip("バナーの準備ができているか")]
	bool isLoaded = false;
	[Tooltip("今ロードを頼んでいる最中か。二重に頼まない為")]
	bool isLoading = false;
	[Tooltip("今の画面がバナーを出して良い画面か。ロードが後から終わった時の判断に使う")]
	bool isVisibleRequested = false;

	[Tooltip("ロードに失敗した時、次に試すまで待つ秒数")]
	const float retryDelay = 5.0f;

	void Awake()
	{
		//iOSかAndroidのどちらのプラットフォームかを取得して広告IDを取得する
		_adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSAdUnitId : _androidAdUnitId;
	}

	//バナー広告をロードする
	public void LoadAd()
	{
		if (isLoading == true || isLoaded == true)
		{
			return;
		}

		isLoading = true;
		Debug.Log("Loading Ad: " + _adUnitId);

		BannerLoadOptions options = new BannerLoadOptions
		{
			loadCallback = OnBannerLoaded,
			errorCallback = OnBannerError
		};

		//位置は初期化が済んだこの時点で入れる。Awakeで入れても初期化前だと残らない事がある
		Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
		Advertisement.Banner.Load(_adUnitId, options);
	}

	//バナー広告のロードを完了した際に実行する
	void OnBannerLoaded()
	{
		Debug.Log("Banner loaded");

		isLoading = false;
		isLoaded = true;

		//ロードが終わるのはプレイ中かもしれない。
		//無条件に出すとゲーム画面にバナーがかぶるので、出して良い画面の時だけ出す
		if (isVisibleRequested == true)
		{
			ShowBannerAd();
		}
	}

	//バナー広告がエラーの場合に実行する
	void OnBannerError(string message)
	{
		Debug.Log($"Banner Error: {message}");

		isLoading = false;
		isLoaded = false;

		//一度失敗したきり読み直さないと、以後ずっとバナーが出せなくなる
		RetryLoadLater();
	}

	/// <summary>
	/// バナー広告を表示する。プレイ中は画面を隠してしまうのでAdsManagerから制御する
	/// </summary>
	public void ShowBannerAd()
	{
		isVisibleRequested = true;

		if (isLoaded == false)
		{
			//準備できていないのにShowを呼んでも出ない。読み込んでおいて、
			//終わったらOnBannerLoadedから改めて出す
			LoadAd();
			return;
		}

		BannerOptions options = new BannerOptions
		{
			clickCallback = OnBannerClicked,
			hideCallback = OnBannerHidden,
			showCallback = OnBannerShown
		};

		Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
		Advertisement.Banner.Show(_adUnitId, options);
	}

	/// <summary>
	/// バナー広告を非表示にする
	/// </summary>
	public void HideBannerAd()
	{
		isVisibleRequested = false;

		//バナー広告を非表示にする
		Advertisement.Banner.Hide();
	}

	/// <summary>
	/// 少し待ってからロードし直す。すぐ呼び直すと失敗が続いた時に回り続けてしまう
	/// </summary>
	void RetryLoadLater()
	{
		if (isActiveAndEnabled == false)
		{
			return;
		}

		StartCoroutine(RetryLoadCoroutine());
	}

	IEnumerator RetryLoadCoroutine()
	{
		yield return new WaitForSecondsRealtime(retryDelay);
		LoadAd();
	}

	void OnBannerClicked() { }
	void OnBannerShown() { Debug.Log("Banner shown"); }
	void OnBannerHidden() { }
}
