using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージセレクトのカード1枚に共通する見た目の操作をまとめたクラス。
/// どの状態にするかはStageSelectButtonとBonusStageSelectButtonが決める。
/// </summary>
public class StageSelectButtonBase : MonoBehaviour
{
	[Tooltip("次のステージへ伸びる縦棒")]
	[SerializeField] GameObject verticalBar;
	[Tooltip("縦棒の中で進み具合を示すゲージ")]
	[SerializeField] Image verticalBarGauge;
	[Tooltip("カードの下に引く線。状態によって色を変える")]
	[SerializeField] Image frameLine;
	[Tooltip("カードのガラス板")]
	[SerializeField] Image background;
	[Tooltip("「ステージ 1」などの見出し")]
	[SerializeField] Text textLabel;
	[Tooltip("「クリア」などの状態")]
	[SerializeField] Text textStatus;
	[Tooltip("カードに薄く大きく出す番号や記号")]
	[SerializeField] Text textNumber;

	[Tooltip("まだ遊べないステージの線の色")]
	protected static readonly Color lockedColor = new Color(1.0f, 1.0f, 1.0f, 0.3f);
	[Tooltip("今遊べるステージの線の色")]
	protected static readonly Color currentColor = new Color(0.404f, 0.827f, 1.0f, 1.0f);
	[Tooltip("クリア済みのステージの線の色")]
	protected static readonly Color clearedColor = new Color(0.541f, 0.898f, 0.729f, 1.0f);
	[Tooltip("まだ遊べないカードの薄さ")]
	protected static readonly Color lockedBackgroundColor = new Color(1.0f, 1.0f, 1.0f, 0.45f);
	[Tooltip("まだ遊べないステージの文字の色")]
	protected static readonly Color lockedTextColor = new Color(0.804f, 0.906f, 0.965f, 0.7f);

	protected int buttonNumber;
	public int ButtonNumber => buttonNumber;

	/// <summary>
	/// カードのガラス板の濃さを設定
	/// </summary>
	public void SetBackgroundColor(Color color)
	{
		if (background == null)
		{
			return;
		}

		background.color = color;
	}

	/// <summary>
	/// 縦棒を消す
	/// </summary>
	public void HideVerticalBar()
	{
		if (verticalBar == null)
		{
			return;
		}

		verticalBar.gameObject.SetActive(false);
	}

	/// <summary>
	/// 縦棒ゲージの数値を指定
	/// </summary>
	/// <param name="current">0から100</param>
	public void SetVerticalBarGauge(float current)
	{
		if (verticalBarGauge == null)
		{
			return;
		}

		const int max = 100;
		verticalBarGauge.fillAmount = current / max;
	}

	/// <summary>
	/// 下線の色を設定
	/// </summary>
	public void SetFrameLineColor(Color color)
	{
		if (frameLine == null)
		{
			return;
		}

		frameLine.color = color;
	}

	/// <summary>
	/// 下線を「今遊べる」色にする。演出の最後に外から呼ばれる
	/// </summary>
	public void SetFrameLineToCurrent()
	{
		SetFrameLineColor(currentColor);
	}

	/// <summary>
	/// カードの見出しを設定
	/// </summary>
	public void SetLabelText(string text)
	{
		if (textLabel == null)
		{
			return;
		}

		textLabel.text = text;
	}

	/// <summary>
	/// カードの状態表示を設定
	/// </summary>
	public void SetStatusText(string text, Color color)
	{
		if (textStatus == null)
		{
			return;
		}

		textStatus.text = text;
		textStatus.color = color;
	}

	/// <summary>
	/// カードに薄く大きく出す番号や記号を設定
	/// </summary>
	public void SetNumberText(string text)
	{
		if (textNumber == null)
		{
			return;
		}

		textNumber.text = text;
	}
}
