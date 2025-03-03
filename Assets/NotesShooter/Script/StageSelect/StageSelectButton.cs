using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] GameObject stageSelectButton;
    [SerializeField] GameObject verticalBar;
    [SerializeField] Image frameLine;
    [SerializeField] Image main;
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
            SetFrameLineColor(Color.red);
            SetMainColor(Color.gray);
            SetCompleteMark(true);
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
    /// クリア済みマークの表示/非表示
    /// </summary>
    /// <param name="isCompleted"></param>
    public void SetCompleteMark(bool isCompleted)
    {
        completeMarkGameObject.SetActive(isCompleted);
    }

    /// <summary>
    /// 縮小
    /// </summary>
    public void Reduction()
    {
        Vector2 sizeDelta = stageSelectButton.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 200;
        stageSelectButton.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        SetButtonGameObject(false);
    }

    /// <summary>
    /// 拡大
    /// </summary>
    public void Expansion()
    {
        Vector2 sizeDelta = stageSelectButton.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 300;
        stageSelectButton.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        SetButtonGameObject(true);
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
