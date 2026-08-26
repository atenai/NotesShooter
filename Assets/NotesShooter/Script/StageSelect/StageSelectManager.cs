using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
	[Tooltip("フェード用の黒画像")]
	[SerializeField] private Image fadeImage;
	[Tooltip("ステージボタンから飛ぶシーン名")]
	[SerializeField] private string stageSceneName = "Stage2";
	[Tooltip("ボーナスステージボタンから飛ぶシーン名")]
	[SerializeField] private string bonusStageSceneName = "MasterStage";

	[Tooltip("フェードの速さ")]
	private const float fadeSpeed = 2.5f;
	[Tooltip("フェードインで薄くしていく黒画像の濃さ")]
	private float fadeInAlfa = 1.0f;
	[Tooltip("フェードインが終わったか")]
	private bool isFadeInEnd = false;
	[Tooltip("フェードアウト中の黒画像の濃さ")]
	private float fadeOutAlfa = 0.0f;
	[Tooltip("フェードアウト中か")]
	private bool isFadeOut = false;
	[Tooltip("フェードアウトが終わったら読み込むシーン名")]
	private string nextSceneName = "";

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

	public static int playCount = 1;

	void Start()
	{
		//シーンの開始時は真っ黒にしておき、少しずつ透明にする
		fadeInAlfa = 1.0f;
		isFadeInEnd = false;
		isFadeOut = false;
		ApplyFadeAlfa(fadeInAlfa);

		InitVerticalLayoutGroupPadding();
		CreateStageButtons();

		//以下演出
		StartCoroutine(Direction());
	}

	void InitVerticalLayoutGroupPadding()
	{
		if (playCount == First_Stage)
		{
			verticalLayoutGroup.padding.bottom = 1000;
		}
		else if (playCount == Total_Stage)
		{
			verticalLayoutGroup.padding.top = 1000;
		}

		// レイアウト更新を即座に反映
		//LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());
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

	void Update()
	{
		FadeIn();
		FadeOut();

		if (Input.GetKeyDown(KeyCode.R))
		{
			//bonusStageSelectButton.Reduction();
			//stageSelectButtons[1].Reduction();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			//bonusStageSelectButton.Expansion();
			//stageSelectButtons[1].Expansion();
		}

		if (Input.GetKeyDown(KeyCode.U))
		{
			//stageSelectButtons[totalStageCount - 1].GetComponent<BonusStageSelectButton>().AdvanceUnlock();
		}
	}

	/// <summary>
	/// 黒画像の濃さを反映する
	/// </summary>
	private void ApplyFadeAlfa(float alfa)
	{
		if (fadeImage == null)
		{
			return;
		}

		Color color = fadeImage.color;
		fadeImage.color = new Color(color.r, color.g, color.b, alfa);
	}

	/// <summary>
	/// シーン開始時のフェードイン
	/// </summary>
	private void FadeIn()
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

		ApplyFadeAlfa(fadeInAlfa);
	}

	/// <summary>
	/// フェードアウトし切ったらシーンを切り替える
	/// </summary>
	private void FadeOut()
	{
		if (isFadeOut == false)
		{
			return;
		}

		fadeOutAlfa += fadeSpeed * Time.deltaTime;
		ApplyFadeAlfa(fadeOutAlfa);

		const float max = 1.0f;
		if (max <= fadeOutAlfa)
		{
			isFadeOut = false;
			UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
		}
	}

	/// <summary>
	/// ステージボタンから呼ばれる。フェードアウトしてからステージへ移る
	/// </summary>
	public void RequestStageStart()
	{
		RequestSceneChange(stageSceneName);
	}

	/// <summary>
	/// ボーナスステージボタンから呼ばれる
	/// </summary>
	public void RequestBonusStageStart()
	{
		RequestSceneChange(bonusStageSceneName);
	}

	private void RequestSceneChange(string sceneName)
	{
		//連打で二重に遷移しないようにする
		if (isFadeOut == true || string.IsNullOrEmpty(sceneName) == true)
		{
			return;
		}

		nextSceneName = sceneName;
		fadeOutAlfa = 0.0f;
		isFadeOut = true;
	}

	/// <summary>
	/// 演出
	/// </summary>
	/// <returns></returns>
	private IEnumerator Direction()
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

			currentButton.SetFrameLineColor(Color.red);

			yield return new WaitForSeconds(1.0f);
		}

		yield return null;
	}
}
