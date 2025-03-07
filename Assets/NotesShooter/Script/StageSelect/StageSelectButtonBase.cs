using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectButtonBase : MonoBehaviour
{
	[Tooltip("縦棒画像")]
	[SerializeField] GameObject verticalBar;
	[Tooltip("縦棒ゲージ画像")]
	[SerializeField] Image verticalBarGauge;
	[Tooltip("枠線画像")]
	[SerializeField] Image frameLine;
	[Tooltip("バックグラウンド画像")]
	[SerializeField] Image background;

	protected int buttonNumber;
	public int ButtonNumber => buttonNumber;

	/// <summary>
	/// メイン画像のカラーを設定
	/// </summary>
	/// <param name="color"></param>
	public void SetBackgroundColor(Color color)
	{
		background.color = color;
	}

	/// <summary>
	/// 縦棒を消す
	/// </summary>
	public void HideVerticalBar()
	{
		verticalBar.gameObject.SetActive(false);
	}

	/// <summary>
	/// 縦棒ゲージの数値を指定
	/// </summary>
	/// <param name="current"></param>
	public void SetVerticalBarGauge(float current)
	{
		const int max = 100;
		verticalBarGauge.fillAmount = current / max;
	}

	/// <summary>
	/// 枠線画像のカラーを設定
	/// </summary>
	/// <param name="color"></param>
	public void SetFrameLineColor(Color color)
	{
		frameLine.color = color;
	}
}
