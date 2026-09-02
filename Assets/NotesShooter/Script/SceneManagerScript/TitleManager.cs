using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour, IFadeSceneManager
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

	[Tooltip("メニュー項目を選んだ時に強調表示する下線")]
	[SerializeField] Image imageMenuUnderline;
	[Tooltip("ハイスコアの表示")]
	[SerializeField] Text textHighScore;
	[Tooltip("バージョンの表示")]
	[SerializeField] Text textVersion;

	[Tooltip("下線を点滅させる速さ")]
	const float underlinePulseSpeed = 1.6f;
	[Tooltip("下線の一番薄い時の濃さ")]
	const float underlineMinAlfa = 0.35f;

	void Start()
	{
		InitFade();

		DisplayHighScore();
		DisplayVersion();
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
		PulseUnderline();
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

	/// <summary>
	/// 一番良いスコアを出す。まだ遊んでいなければ0になる
	/// </summary>
	void DisplayHighScore()
	{
		if (textHighScore == null)
		{
			return;
		}

		//直前に遊んだステージのハイスコアを出す。未プレイならキーが無いので0が返る
		int highScore = ScoreRecord.GetHighScore(ScoreRecord.LastStageName);
		textHighScore.text = "HIGH SCORE   " + highScore.ToString();
	}

	void DisplayVersion()
	{
		if (textVersion == null)
		{
			return;
		}

		textVersion.text = "v " + Application.version;
	}

	/// <summary>
	/// 選択中の項目の下線をゆっくり明滅させて、そこが選べる事を示す
	/// </summary>
	void PulseUnderline()
	{
		if (imageMenuUnderline == null)
		{
			return;
		}

		float pulse = (Mathf.Sin(Time.time * underlinePulseSpeed) + 1.0f) * 0.5f;
		float alfa = Mathf.Lerp(underlineMinAlfa, 1.0f, pulse);

		Color color = imageMenuUnderline.color;
		imageMenuUnderline.color = new Color(color.r, color.g, color.b, alfa);
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

	/// <summary>
	/// ゲーム終了ボタン
	/// </summary>
	public void QuitButton()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
