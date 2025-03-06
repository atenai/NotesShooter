using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
	[SerializeField] private ScrollRect scrollRect;
	[SerializeField] private GameObject content;
	[Tooltip("ステージボタンのプレハブ")]
	[SerializeField] private GameObject stageSelectButtonPrefab;
	[SerializeField] private GameObject bonusStageSelectButtonPrefab;
	int totalStageCount = 4;
	List<StageSelectButton> stageSelectButtons = new List<StageSelectButton>();
	BonusStageSelectButton bonusStageSelectButton;

	public static int playCount = 1;

	void Start()
	{
		CreateStageButtons();

		//以下演出
		scrollRect.verticalNormalizedPosition = 0 / totalStageCount;

		StartCoroutine(Direction());
	}

	/// <summary>
	/// ステージボタン生成
	/// </summary>
	void CreateStageButtons()
	{
		for (int i = 1; i <= totalStageCount - 1; i++)
		{
			// プレハブからボタンをInstantiateしてContentの子オブジェクトに配置
			GameObject stageSelectButtonGameObject = Instantiate(stageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
			StageSelectButton stageSelectButton = stageSelectButtonGameObject.GetComponent<StageSelectButton>();
			stageSelectButton.Initialize(i, totalStageCount, playCount);
			stageSelectButtons.Add(stageSelectButton);
		}

		GameObject bonusStageSelectButtonGameObject = Instantiate(bonusStageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
		bonusStageSelectButton = bonusStageSelectButtonGameObject.GetComponent<BonusStageSelectButton>();
		bonusStageSelectButton.Initialize(totalStageCount, totalStageCount, playCount);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			//bonusStageSelectButton.Reduction();
			stageSelectButtons[1].Reduction();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			//bonusStageSelectButton.Expansion();
			stageSelectButtons[1].Expansion();
		}

		if (Input.GetKeyDown(KeyCode.U))
		{
			bonusStageSelectButton.AdvanceUnlock();
		}
	}

	/// <summary>
	/// 演出
	/// </summary>
	/// <returns></returns>
	private IEnumerator Direction()
	{
		StageSelectButton currentButton = stageSelectButtons.FirstOrDefault(button => button.ButtonNumber == playCount);

		if (currentButton != null)
		{
			bool isCurrentButtonCompleted = false;
			scrollRect.ScrollToCentering(currentButton.gameObject, 1, () =>
			{
				isCurrentButtonCompleted = true;
			});
			yield return new WaitUntil(() => isCurrentButtonCompleted);

			int i = 0;
			yield return new WaitWhile(() =>
			{
				i++;
				currentButton.SetVerticalBarGauge(i);
				return i < 100;
			});

			StageSelectButton nextButton = stageSelectButtons.FirstOrDefault(button => button.ButtonNumber == playCount + 1);
			if (nextButton != null)
			{
				bool isNextButtonCompleted = false;
				scrollRect.ScrollToCentering(nextButton.gameObject, 1, () =>
				{
					isNextButtonCompleted = true;
				});
				yield return new WaitUntil(() => isNextButtonCompleted);

				nextButton.SetFrameLineColor(Color.red);
			}
		}

		yield return null;
	}
}
