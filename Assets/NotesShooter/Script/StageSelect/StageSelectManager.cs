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
	[Tooltip("ボーナスステージボタンのプレハブ")]
	[SerializeField] private GameObject bonusStageSelectButtonPrefab;
	[SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
	List<StageSelectButtonBase> stageSelectButtons = new List<StageSelectButtonBase>();
	int totalStageCount = 4;
	public static int playCount = 1;

	void Start()
	{
		CreateStageButtons();

		//以下演出
		StartCoroutine(Direction());
	}

	/// <summary>
	/// ステージボタン生成
	/// </summary>
	void CreateStageButtons()
	{
		for (int i = 1; i <= totalStageCount - 1; i++)
		{
			// プレハブからステージボタンをInstantiateしてContentの子オブジェクトに配置
			GameObject stageSelectButtonGameObject = Instantiate(stageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
			StageSelectButton stageSelectButton = stageSelectButtonGameObject.GetComponent<StageSelectButton>();
			stageSelectButton.Initialize(i, totalStageCount, playCount);
			stageSelectButtons.Add(stageSelectButton);
		}

		// プレハブからボーナスステージボタンをInstantiateしてContentの子オブジェクトに配置
		GameObject bonusStageSelectButtonGameObject = Instantiate(bonusStageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
		BonusStageSelectButton bonusStageSelectButton = bonusStageSelectButtonGameObject.GetComponent<BonusStageSelectButton>();
		bonusStageSelectButton.Initialize(totalStageCount, totalStageCount, playCount);
		stageSelectButtons.Add(bonusStageSelectButton);
	}

	void Update()
	{
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
	/// 演出
	/// </summary>
	/// <returns></returns>
	private IEnumerator Direction()
	{
		verticalLayoutGroup.padding.top = 1000;
		verticalLayoutGroup.padding.bottom = 1000;

		scrollRect.verticalNormalizedPosition = 0 / totalStageCount;

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
		}

		yield return new WaitForSeconds(1.0f);

		verticalLayoutGroup.padding.top = 100;
		verticalLayoutGroup.padding.bottom = 100;
		// レイアウト更新を即座に反映
		LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());

		yield return null;
	}
}
