using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
	[SerializeField] string _androidAdUnitId = "Interstitial_Android";
	[SerializeField] string _iOSAdUnitId = "Interstitial_iOS";
	string _adUnitId;

	[Tooltip("広告の準備ができているか。できていない時に出そうとしても失敗するだけなので見張る")]
	bool isLoaded = false;
	[Tooltip("今ロードを頼んでいる最中か。二重に頼まない為")]
	bool isLoading = false;

	[Tooltip("今この広告が画面を覆っているか。覆っている間の演出は見えないので、待ちたい側が見る")]
	static bool isShowing = false;
	public static bool IsShowing => isShowing;

	[Tooltip("ロードに失敗した時、次に試すまで待つ秒数")]
	const float retryDelay = 5.0f;

	void Awake()
	{
		//iOSかAndroidのどちらのプラットフォームかを取得して広告IDを取得する
		_adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSAdUnitId : _androidAdUnitId;
	}

	void OnDisable()
	{
		//シーンをまたいでも残る値なので、消える時は必ず戻しておく
		isShowing = false;
	}

	//インターステーショナル広告をロードする
	public void LoadAd()
	{
		if (isLoading == true || isLoaded == true)
		{
			return;
		}

		isLoading = true;
		Debug.Log("Loading Ad: " + _adUnitId);
		Advertisement.Load(_adUnitId, this);
	}

	/// <summary>
	/// インターステーショナル広告を表示する。
	/// 準備ができていなければ出さずにロードだけ頼み、falseを返す
	/// </summary>
	public bool ShowAd()
	{
		if (isLoaded == false)
		{
			//準備できていないのにShowを呼んでも失敗するだけ。次に備えて読み込んでおく
			Debug.Log("インターステーシャル広告がまだ準備できていないので表示しません");
			LoadAd();
			return false;
		}

		//一度出した広告はそのままでは再利用できない
		isLoaded = false;

		Debug.Log("Showing Ad: " + _adUnitId);
		Advertisement.Show(_adUnitId, this);
		return true;
	}

	//インターステーショナル広告を正常にロードできた場合に実行する
	public void OnUnityAdsAdLoaded(string adUnitId)
	{
		isLoading = false;
		isLoaded = true;
		Debug.Log("インターステーシャル広告の準備ができました");
	}

	//インターステーショナル広告のロードに失敗した場合に実行する
	public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
	{
		Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");

		isLoading = false;
		isLoaded = false;

		//一度失敗したきり読み直さないと、以後ずっと広告が出せなくなる
		RetryLoadLater();
	}

	//インターステーショナル広告の表示に失敗した場合に実行する
	public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
	{
		Debug.Log($"Error showing Ad Unit: {adUnitId} - {error.ToString()} - {message}");

		isShowing = false;

		//失敗した時も読み直す。ここを抜かすと一度の失敗で以後ずっと出なくなる
		LoadAd();
	}

	//広告が実際に画面を覆い始めた時に実行する
	public void OnUnityAdsShowStart(string adUnitId)
	{
		//Showを呼んだ時点ではなく、本当に出た時に立てる。
		//出せなかった場合にフラグが立ちっぱなしになるのを防ぐ
		isShowing = true;
	}

	public void OnUnityAdsShowClick(string adUnitId) { }

	//インターステーショナル広告を正常に表示完了後に実行する
	public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
	{
		Debug.Log("<color=blue>あなたはインターステーショナル広告をゲットしました。</color>");

		isShowing = false;

		//次に備えてロードしておく
		LoadAd();
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
}
