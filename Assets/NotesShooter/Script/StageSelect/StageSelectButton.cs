using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectButton : StageSelectButtonBase
{
    [SerializeField] GameObject stageSelectButton;
    [SerializeField] GameObject verticalBar;
    [SerializeField] Image frameLine;
    [SerializeField] GameObject buttonGameObject;
    [SerializeField] Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [Tooltip("完了マーク")]
    [SerializeField] private GameObject completeMarkGameObject;
    int buttonNumber;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="buttonNumber"></param>
    /// <param name="totalNumber"></param>
    public void Initialize(int buttonNumber, int totalNumber, int playCount)
    {
        this.buttonNumber = buttonNumber;
        SetButtonText(buttonNumber.ToString());
        Reduction();
        SetVerticalBarGauge(0);
        SetFrameLineColor(Color.gray);
        SetBackgroundColor(Color.white);

        //現在の日なら
        if (buttonNumber == playCount)
        {
            Expansion();
            SetFrameLineColor(Color.red);
        }

        //前の日なら
        if (buttonNumber < playCount)
        {
            SetVerticalBarGauge(100);
            SetFrameLineColor(Color.red);
            SetCompleteMark(true);
        }

        //最後の日なら
        if (buttonNumber == totalNumber)
        {
            HideVerticalBar();
            SetVerticalBarGauge(0);
        }
    }

    /// <summary>
    /// 縦棒を消す
    /// </summary>
    public void HideVerticalBar()
    {
        verticalBar.gameObject.SetActive(false);
    }

    /// <summary>
    /// 枠線画像のカラーを設定
    /// </summary>
    /// <param name="color"></param>
    public void SetFrameLineColor(Color color)
    {
        frameLine.color = color;
    }

    /// <summary>
    /// ボタンの表示/非表示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetButtonGameObject(bool isActive)
    {
        buttonGameObject.SetActive(isActive);
    }

    /// <summary>
    /// ボタンのテキストに文字をセット
    /// </summary>
    /// <param name="text"></param>
    public void SetButtonText(string text)
    {
        buttonText.text = text;
    }

    /// <summary>
    /// クリア済みマークの表示/非表示
    /// </summary>
    /// <param name="isCompleted"></param>
    public void SetCompleteMark(bool isCompleted)
    {
        completeMarkGameObject.SetActive(isCompleted);
    }

    /// <summary>
    /// ボタンのサイズをセット
    /// </summary>
    /// <param name="x">横幅</param>
    /// <param name="y">縦幅</param>
    public void SetStageSelectButtonSize(float x = 500, float y = 300)
    {
        Vector2 sizeDelta = stageSelectButton.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = x;
        sizeDelta.y = y;
        stageSelectButton.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }

    /// <summary>
    /// 拡大
    /// </summary>
    public void Expansion()
    {
        SetStageSelectButtonSize(y: 300);

        SetButtonGameObject(true);
    }

    /// <summary>
    /// 縮小
    /// </summary>
    public void Reduction()
    {
        SetStageSelectButtonSize(450, 200);

        SetButtonGameObject(false);
    }

    void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Debug.Log(buttonNumber);

        StageSelectManager.playCount++;
        SceneManager.LoadScene("StageSelect");
    }

    void Update()
    {

    }
}
