using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject stageSelectButtonPrefab;  // ステージボタンのプレハブ
    [SerializeField] private GameObject bonusStageSelectButtonPrefab;
    int totalStageCount = 4;
    List<StageSelectButton> stageSelectButtons = new List<StageSelectButton>();
    BonusStageSelectButton bonusStageSelectButton;

    void Start()
    {
        CreateStageButtons();
    }

    void CreateStageButtons()
    {
        for (int i = 1; i <= totalStageCount - 1; i++)
        {
            // プレハブからボタンをInstantiateしてContentの子オブジェクトに配置
            GameObject stageSelectButtonGameObject = Instantiate(stageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
            StageSelectButton stageSelectButton = stageSelectButtonGameObject.GetComponent<StageSelectButton>();
            stageSelectButton.Initialize(i, totalStageCount);
            stageSelectButtons.Add(stageSelectButton);
        }

        GameObject bonusStageSelectButtonGameObject = Instantiate(bonusStageSelectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, content.transform);
        bonusStageSelectButton = bonusStageSelectButtonGameObject.GetComponent<BonusStageSelectButton>();
        bonusStageSelectButton.Initialize(totalStageCount, totalStageCount);
    }

    void Update()
    {

    }
}
