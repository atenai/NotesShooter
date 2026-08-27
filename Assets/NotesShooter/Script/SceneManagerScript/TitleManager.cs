using UnityEngine;
using UnityEngine.UI;

public class TitleManager : BaseSceneManager
{
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

	new void Start()
	{
		base.Start();

		DisplayHighScore();
		DisplayVersion();
	}

	void Update()
	{
		FadeIn();
		FadeOut();
		PulseUnderline();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		//ESCキーでゲームを終了する
		if (Input.GetKeyDown(KeyCode.Escape) == true)
		{
			RequestQuit();
		}
#endif//終了
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
	/// 「ゲームスタート」から呼ばれる。フェードアウトしてステージセレクトへ移る
	/// </summary>
	public void RequestGameStart()
	{
		//フェード中の二重押しを弾く
		if (isFade == true)
		{
			return;
		}

		isFade = true;

		if (audioSource != null && audioClip != null)
		{
			audioSource.PlayOneShot(audioClip);
		}
	}

	/// <summary>
	/// 「ゲーム終了」から呼ばれる
	/// </summary>
	public void RequestQuit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
