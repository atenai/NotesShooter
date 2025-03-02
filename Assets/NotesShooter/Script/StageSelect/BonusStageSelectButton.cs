using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusStageSelectButton : MonoBehaviour
{
    [SerializeField] GameObject verticalBar;
    [SerializeField] GameObject buttonGameObject;
    [SerializeField] Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

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

    public void SetButtonText(string text)
    {
        buttonText.text = text;
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
