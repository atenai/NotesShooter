using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BonusStageSelectButton : StageSelectButtonBase
{
	[SerializeField] GameObject bonusStageSelectButton;
	[SerializeField] GameObject buttonGameObject;
	[SerializeField] Button button;
	[SerializeField] private TextMeshProUGUI buttonText;
	[SerializeField] GameObject icon;

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="buttonNumber"></param>
	/// <param name="totalNumber"></param>
	public void Initialize(int buttonNumber, int totalNumber, int playCount)
	{
		this.buttonNumber = buttonNumber;

		Reduction();
		SetVerticalBarGauge(0);
		SetFrameLineColor(Color.gray);
		SetBackgroundColor(Color.white);
		SetButtonText(buttonNumber.ToString());

		//現在の日なら
		if (buttonNumber == playCount)
		{
			Expansion();
		}

		//前の日なら
		if (buttonNumber < playCount)
		{
			SetVerticalBarGauge(100);
			SetFrameLineColor(Color.red);
		}

		//最後の日なら
		if (buttonNumber == totalNumber)
		{
			HideVerticalBar();
			SetVerticalBarGauge(0);
		}
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
	/// ボタンのサイズをセット
	/// </summary>
	/// <param name="x">横幅</param>
	/// <param name="y">縦幅</param>
	public void SetBonusStageSelectButtonSize(float x = 500, float y = 400)
	{
		bonusStageSelectButton.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
	}

	/// <summary>
	/// アイコンの座標をセット
	/// </summary>
	/// <param name="y"></param>
	public void SetIconPos(float y)
	{
		Vector2 pos = icon.GetComponent<RectTransform>().anchoredPosition;
		pos.y = y;
		icon.GetComponent<RectTransform>().anchoredPosition = pos;
	}

	/// <summary>
	/// 拡大
	/// </summary>
	public void Expansion()
	{
		SetBonusStageSelectButtonSize(y: 400);
		SetIconPos(75);
		SetButtonGameObject(true);
	}

	/// <summary>
	/// 縮小
	/// </summary>
	public void Reduction()
	{
		SetBonusStageSelectButtonSize(450, 200);
		SetIconPos(25);
		SetButtonGameObject(false);
	}

	/// <summary>
	/// 前倒しアンロック 
	/// </summary>
	public void AdvanceUnlock()
	{
		Expansion();
		SetBackgroundColor(Color.white);
	}

	void Start()
	{
		button.onClick.AddListener(OnClick);
	}

	void OnClick()
	{
		Debug.Log(buttonNumber);
		SceneManager.LoadScene("MasterStage");
	}
}
