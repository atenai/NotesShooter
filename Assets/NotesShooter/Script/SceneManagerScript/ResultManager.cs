using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour, IFadeSceneManager
{
	[Header("オーディオ")]
	[SerializeField] AudioClip audioClip;
	[SerializeField] AudioSource audioSource;

	[Header("シーン遷移")]
	[Tooltip("次のシーン名")]
	string nextSceneName = "";

	[Header("フェード")]
	[Tooltip("フェード用の黒画像")]
	[SerializeField] Image fadeImage;
	[Tooltip("フェードの速度")]
	float fadeSpeed = 2.5f;
	[Tooltip("フェードインのアルファ値")]
	float fadeInAlfa = 1.0f;
	[Tooltip("フェードインが終わったか？")]
	bool isFadeInEnd = false;
	[Tooltip("フェードアウトのアルファ値")]
	float fadeOutAlfa = 0.0f;
	[Tooltip("フェードアウトが始まったか？")]
	bool isFadeOutStart = false;

	void Start()
	{
		InitFade();

		//リザルトに来るたびに数え、規定回数ごとにインターステーシャル広告を出す
		if (AdsManager.SingletonInstance != null)
		{
			AdsManager.SingletonInstance.ShowAdsInterstitialCount();
		}
	}

	public void InitFade()
	{
		//フェードインの初期化
		isFadeInEnd = false;
		fadeInAlfa = 1.0f;
		fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeInAlfa);

		//フェードアウトの初期化
		isFadeOutStart = false;
		fadeOutAlfa = 0.0f;
	}

	void Update()
	{
		FadeIn();
		FadeOut();
	}

	public void FadeIn()
	{
		if (isFadeInEnd == true)
		{
			return;
		}

		fadeInAlfa -= fadeSpeed * Time.deltaTime;

		const float min = 0.0f;
		if (fadeInAlfa <= min)
		{
			fadeInAlfa = min;
			isFadeInEnd = true;
		}

		fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeInAlfa);
	}

	/// <summary>
	/// タイトルへ遷移ボタン
	/// </summary>
	public void TitleButton()
	{
		const string Title_SceneName = "Title";
		RequestSceneChange(Title_SceneName);
	}

	/// <summary>
	/// ステージセレクトへ遷移ボタン
	/// </summary>
	public void StageSelectButton()
	{
		const string StageSelect_SceneName = "StageSelect";
		RequestSceneChange(StageSelect_SceneName);
	}

	/// <summary>
	/// 行き先を決めてフェードアウトを始める
	/// </summary>
	void RequestSceneChange(string sceneName)
	{
		//連打で二重に遷移しないようにする
		if (isFadeOutStart == true)
		{
			return;
		}

		nextSceneName = sceneName;
		isFadeOutStart = true;

		if (audioSource != null && audioClip != null)
		{
			audioSource.PlayOneShot(audioClip);
		}
	}

	public void FadeOut()
	{
		if (isFadeOutStart == true)
		{
			fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeOutAlfa);
			fadeOutAlfa += fadeSpeed * Time.deltaTime;
		}

		const float max = 1.0f;
		if (max <= fadeOutAlfa)
		{
			isFadeOutStart = false;
			SceneChange(nextSceneName);
		}
	}

	public void SceneChange(string name)
	{
		SceneManager.LoadScene(name);
	}
}
