using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusStageSelectButton : MonoBehaviour
{
    [SerializeField] GameObject bonusStageSelectButton;
    [SerializeField] GameObject verticalBar;
    [SerializeField] GameObject buttonGameObject;
    [SerializeField] Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] GameObject mark;

    int buttonNumber;

    public void Initialize(int buttonNumber, int totalNumber)
    {
        this.buttonNumber = buttonNumber;
        SetButtonText(buttonNumber.ToString());

        if (buttonNumber == totalNumber)
        {
            HideVerticalBar();
        }
    }

    public void HideVerticalBar()
    {
        verticalBar.gameObject.SetActive(false);
    }

    public void SetButtonGameObject(bool isActive)
    {
        buttonGameObject.SetActive(isActive);
    }

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
