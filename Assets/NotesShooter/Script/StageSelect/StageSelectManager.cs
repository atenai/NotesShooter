using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour, IFadeSceneManager
{
	/// <summary>
	/// ステージ1つ分の紹介文。左側の説明欄に出す
	/// </summary>
	[System.Serializable]
	public class StageInformation
	{
		[Tooltip("大きく出すステージ名")]
		public string stageName = "ステージ 1";
		[Tooltip("小さく出す英語の見出し")]
		public string subName = "STAGE 01";
		[Tooltip("ステージの説明")]
		[TextArea] public string description = "";
		[Tooltip("ステージ情報に出す難易度")]
		public string difficulty = "★☆☆";
	}

	[Header("オーディオ")]
	[SerializeField] AudioClip audioClip;
	[SerializeField] AudioSource audioSource;

	[Header("シーン遷移")]
	[Tooltip("ステージボタンから飛ぶシーン名")]
	const string Stage_SceneName = "Stage2";
	[Tooltip("ボーナスステージボタンから飛ぶシーン名")]
	const string BonusStage_SceneName = "PrototypeStage";
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


	private const int First_Stage = 1;
	private const int Total_Stage = 4;

	[SerializeField] private ScrollRect scrollRect;
	[SerializeField] private GameObject content;
	[Tooltip("ステージボタンのプレハブ")]
	[SerializeField] private GameObject stageSelectButtonPrefab;
	[Tooltip("ボーナスステージボタンのプレハブ")]
	[SerializeField] private GameObject bonusStageSelectButtonPrefab;
	[SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
	List<StageSelectButtonBase> stageSelectButtons = new List<StageSelectButtonBase>();

	[Tooltip("左側に出す、今遊べるステージの大きい見出し")]
	[SerializeField] private Text textStageName;
	[Tooltip("左側に出す、今遊べるステージの小さい見出し")]
	[SerializeField] private Text textStageSubName;
	[Tooltip("左側に出す、今遊べるステージの説明文")]
	[SerializeField] private Text textDescription;
	[Tooltip("今が何ステージ目かの表示")]
	[SerializeField] private Text textProgress;
	[Tooltip("ステージ情報のハイスコア")]
	[SerializeField] private Text textInfoHighScore;
	[Tooltip("ステージ情報の状態")]
	[SerializeField] private Text textInfoState;
	[Tooltip("ステージ情報の難易度")]
	[SerializeField] private Text textInfoDifficulty;
	[Tooltip("バージョンの表示")]
	[SerializeField] private Text textVersion;
	[Tooltip("ステージごとの紹介文。1番目のステージから順に入れる")]
	[SerializeField] private StageInformation[] stageInformations;

	/// <summary>
	/// 何ステージ目まで進んだか。
	/// 以前はstaticな変数に持つだけで、アプリを閉じると必ず最初に戻っていた。
	/// 記録に残して、次に起動した時も続きから遊べるようにしている
	/// </summary>
	public static int playCount
	{
		get { return Mathf.Clamp(ScoreRecord.PlayCount, First_Stage, Total_Stage); }
		private set { ScoreRecord.SavePlayCount(Mathf.Clamp(value, First_Stage, Total_Stage)); }
	}

	/// <summary>
	/// 進んだステージ数を1つ進める。
	/// 一度クリアしたステージをもう一度遊んだ時は進めない。
	/// ステージ数を超えると該当するボタンが無くなるので上限で止める
	/// </summary>
	/// <param name="playedStageNumber">今から遊ぶステージの番号</param>
	public static void AdvancePlayCount(int playedStageNumber)
	{
		if (playedStageNumber < playCount)
		{
			return;
		}

		playCount = Mathf.Min(playCount + 1, Total_Stage);
	}

	void Start()
	{
		InitFade();

		InitVerticalLayoutGroupPadding();
		CreateStageButtons();

		DisplayCurrentStage();
		DisplayProgress();
		DisplayVersion();

		//リザルト画面はシーン名しか知らず「Stage2」としか出せないので、
		//ここで選んでいるステージの表示名を覚えさせておく
		RememberStageDisplayName();

		//以下演出。一度見せた解除の演出は繰り返さない
		if (IsDirectionAlreadySeen() == true)
		{
			StartCoroutine(SkipDirection());
		}
		else
		{
			StartCoroutine(Direction());
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

	/// <summary>
	/// ステージボタンの縦方向の余白を調整する
	/// 最初のステージの時は下に余白を作る。
	/// 最後のステージの時は上に余白を作る。
	/// </summary>
	void InitVerticalLayoutGroupPadding()
	{
		if (playCount == First_Stage)
		{
			verticalLayoutGroup.padding.bottom = 500;
		}
		else if (playCount == Total_Stage)
		{
			verticalLayoutGroup.padding.top = 500;
		}
	}

	/// <summary>
	/// ステージボタン生成
	/// </summary>
	void CreateStageButtons()
	{
		for (int i = First_Stage; i <= Total_Stage - 1; i++)
		{
			// プレハブからステージボタンをInstantiateしてContentの子オブジェクトに配置
			GameObject stageSelectButtonGameObject = Instantiate(stageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
			StageSelectButton stageSelectButton = stageSelectButtonGameObject.GetComponent<StageSelectButton>();
			stageSelectButton.Initialize(i, Total_Stage, playCount);
			stageSelectButtons.Add(stageSelectButton);
		}

		// プレハブからボーナスステージボタンをInstantiateしてContentの子オブジェクトに配置
		GameObject bonusStageSelectButtonGameObject = Instantiate(bonusStageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
		BonusStageSelectButton bonusStageSelectButton = bonusStageSelectButtonGameObject.GetComponent<BonusStageSelectButton>();
		bonusStageSelectButton.Initialize(Total_Stage, Total_Stage, playCount);
		stageSelectButtons.Add(bonusStageSelectButton);
	}

	/// <summary>
	/// 今遊べるステージの見出しと説明とステージ情報を左側に出す
	/// </summary>
	void DisplayCurrentStage()
	{
		int currentStage = Mathf.Clamp(playCount, First_Stage, Total_Stage);

		StageInformation information = null;
		if (stageInformations != null && currentStage - 1 < stageInformations.Length)
		{
			information = stageInformations[currentStage - 1];
		}

		if (textStageName != null)
		{
			textStageName.text = information != null ? information.stageName : "ステージ " + currentStage.ToString();
		}

		if (textStageSubName != null)
		{
			textStageSubName.text = information != null ? information.subName : "STAGE " + currentStage.ToString("00");
		}

		if (textDescription != null)
		{
			textDescription.text = information != null ? information.description : string.Empty;
		}

		string sceneName = currentStage == Total_Stage ? BonusStage_SceneName : Stage_SceneName;
		int highScore = ScoreRecord.GetHighScore(sceneName);

		if (textInfoHighScore != null)
		{
			textInfoHighScore.text = highScore.ToString();
		}

		if (textInfoState != null)
		{
			textInfoState.text = 0 < highScore ? "クリア済み" : "未プレイ";
		}

		if (textInfoDifficulty != null)
		{
			textInfoDifficulty.text = information != null ? information.difficulty : "★☆☆";
		}
	}

	/// <summary>
	/// 今が何ステージ目かを出す。
	/// playCountはTotal_Stageで頭打ちにしているので、クリア数として出すと
	/// 全部遊んでも「3 / 4」までしか進まず正しくない
	/// </summary>
	void DisplayProgress()
	{
		if (textProgress == null)
		{
			return;
		}

		int currentStage = Mathf.Clamp(playCount, First_Stage, Total_Stage);
		textProgress.text = currentStage.ToString() + " / " + Total_Stage.ToString();
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
	/// 今遊ぼうとしているステージの表示名を、リザルト画面の為に覚えておく
	/// </summary>
	void RememberStageDisplayName()
	{
		int currentStage = Mathf.Clamp(playCount, First_Stage, Total_Stage);

		string displayName = "ステージ " + currentStage.ToString();
		if (stageInformations != null && currentStage - 1 < stageInformations.Length && stageInformations[currentStage - 1] != null)
		{
			displayName = stageInformations[currentStage - 1].stageName;
		}

		ScoreRecord.SetPlayingStageDisplayName(displayName);
	}

	/// <summary>
	/// 今遊べるステージの解除の演出を、もう見せたか。
	/// playCountは最後のステージで止まるので、覚えておかないと
	/// ボーナスステージの演出をここへ来る度に毎回見せてしまう
	/// </summary>
	bool IsDirectionAlreadySeen()
	{
		return playCount <= ScoreRecord.DirectedStageNumber;
	}

	/// <summary>
	/// 演出を飛ばして、演出が終わった後と同じ見た目にする。
	/// ゲージを満たし、今遊べるステージの下線を光らせ、その位置に合わせておく
	/// </summary>
	IEnumerator SkipDirection()
	{
		StageSelectButtonBase oldButton = stageSelectButtons.FirstOrDefault(button => button.ButtonNumber == playCount - 1);
		if (oldButton != null)
		{
			//演出で伸ばすはずだったゲージ
			oldButton.SetVerticalBarGauge(100);
		}

		StageSelectButtonBase currentButton = stageSelectButtons.FirstOrDefault(button => button.ButtonNumber == playCount);
		if (currentButton != null)
		{
			currentButton.SetFrameLineToCurrent();
		}

		//ボタンを並べた直後は、スクロール量の計算に使う高さがまだ決まっていない。
		//1フレーム待ってレイアウトを確定させてから位置を合わせる
		yield return null;
		Canvas.ForceUpdateCanvases();

		if (currentButton != null)
		{
			//見せる演出ではないので、動かさずにその位置から始める
			const float immediately = 0.0f;
			scrollRect.ScrollToCentering(currentButton.gameObject, immediately);
		}
	}

	/// <summary>
	/// 演出。1つ前のステージまでスクロールしてゲージを伸ばし、
	/// 今遊べるステージまで上がって下線を光らせる
	/// </summary>
	/// <returns></returns>
	IEnumerator Direction()
	{
		scrollRect.verticalNormalizedPosition = 0 / Total_Stage;

		StageSelectButtonBase oldButton = stageSelectButtons.FirstOrDefault(button => button.ButtonNumber == playCount - 1);
		if (oldButton != null)
		{
			bool isoldButtonScrollCompleted = false;
			scrollRect.ScrollToCentering(oldButton.gameObject, 1, () =>
			{
				isoldButtonScrollCompleted = true;
			});
			yield return new WaitUntil(() => isoldButtonScrollCompleted);

			float i = 0;
			yield return new WaitWhile(() =>
			{
				i++;
				oldButton.SetVerticalBarGauge(i);
				return i < 100;
			});
		}

		StageSelectButtonBase currentButton = stageSelectButtons.FirstOrDefault(button => button.ButtonNumber == playCount);
		if (currentButton != null)
		{
			bool isCurrentScrollButtonCompleted = false;
			scrollRect.ScrollToCentering(currentButton.gameObject, 1, () =>
			{
				isCurrentScrollButtonCompleted = true;
			});
			yield return new WaitUntil(() => isCurrentScrollButtonCompleted);

			currentButton.SetFrameLineToCurrent();

			yield return new WaitForSeconds(1.0f);
		}

		//ここまで見せたので、次に来た時は繰り返さない
		ScoreRecord.SaveDirectedStageNumber(playCount);

		yield return null;
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
	/// ステージへ遷移ボタン
	/// </summary>
	public void StageButton()
	{
		RequestSceneChange(Stage_SceneName);
	}

	/// <summary>
	/// ボーナスステージへ遷移ボタン
	/// </summary>
	public void BonusStageButton()
	{
		RequestSceneChange(BonusStage_SceneName);
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
