using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] GameObject verticalBar;
    [SerializeField] GameObject buttonGameObject;
    [SerializeField] Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    // 完了マークを表示/非表示するためのUI（アイコン画像など）をアタッチ
    [SerializeField] private GameObject completeMarkGameObject;

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

    // クリア済みマークの表示/非表示
    public void SetCompleteMark(bool isCompleted)
    {
        completeMarkGameObject.SetActive(isCompleted);
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
