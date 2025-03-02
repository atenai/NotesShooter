using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusStageSelectButton : MonoBehaviour
{
    [SerializeField] GameObject bonusStageSelectButton;
    [SerializeField] GameObject verticalBar;
    [SerializeField] Image frameLine;
    [SerializeField] Image main;
    [SerializeField] GameObject buttonGameObject;
    [SerializeField] Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] GameObject mark;
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
        SetFrameLineColor(Color.clear);
        SetMainColor(Color.gray);

        //現在の日なら
        if (buttonNumber == playCount)
        {
            Expansion();
            SetFrameLineColor(Color.red);
            SetMainColor(Color.white);
        }

        //前の日なら
        if (buttonNumber < playCount)
        {
            SetMainColor(Color.gray);
        }

        if (buttonNumber == totalNumber)
        {
            HideVerticalBar();
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
    /// メイン画像のカラーを設定
    /// </summary>
    /// <param name="color"></param>
    public void SetMainColor(Color color)
    {
        main.color = color;
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
    /// 縮小
    /// </summary>
    public void Reduction()
    {
        Vector2 sizeDelta = bonusStageSelectButton.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 200;
        bonusStageSelectButton.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        Vector2 pos = mark.GetComponent<RectTransform>().anchoredPosition;
        pos.y = 25;
        mark.GetComponent<RectTransform>().anchoredPosition = pos;

        SetButtonGameObject(false);
    }

    /// <summary>
    /// 拡大
    /// </summary>
    public void Expansion()
    {
        Vector2 sizeDelta = bonusStageSelectButton.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 400;
        bonusStageSelectButton.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        SetButtonGameObject(true);
    }

    /// <summary>
    /// 前倒しアンロック 
    /// </summary>
    public void AdvanceUnlock()
    {
        Expansion();
        SetMainColor(Color.white);
    }

    void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Debug.Log(buttonNumber);
    }

    void Update()
    {

    }
}
