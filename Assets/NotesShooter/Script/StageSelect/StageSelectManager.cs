using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
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
