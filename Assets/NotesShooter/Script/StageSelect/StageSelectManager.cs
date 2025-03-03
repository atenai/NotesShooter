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

    IEnumerator Direction()
    {
        if (playCount < totalStageCount)
        {
            StageSelectButton currentButton = stageSelectButtons[playCount - 1];

            Debug.Log("start");

            int i = 0;
            yield return new WaitWhile(() =>
            {
                Debug.Log("i : " + i);
                i++;
                currentButton.SetVerticalBarGauge(i);

                return i < 100;
            });

            Debug.Log("end");
        }

        yield return null;
    }
}
