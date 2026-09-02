using UnityEngine;
using UnityEngine.UI;

public class BonusStageSelectButton : StageSelectButtonBase
{
	[SerializeField] GameObject bonusStageSelectButton;
	[SerializeField] GameObject buttonGameObject;
	[SerializeField] Button button;
	[Tooltip("スタートボタンの中の文字")]
	[SerializeField] private Text buttonText;
	[SerializeField] GameObject icon;

	[Tooltip("スタートボタンに出す文字")]
	const string startText = "▶ スタート";
	[Tooltip("カードに薄く大きく出す記号")]
	const string bonusMark = "★";

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="buttonNumber">このカードが何番目のステージか</param>
	/// <param name="totalNumber">ステージの総数</param>
	/// <param name="playCount">今どこまで遊んだか</param>
	public void Initialize(int buttonNumber, int totalNumber, int playCount)
	{
		this.buttonNumber = buttonNumber;

		Reduction();
		SetVerticalBarGauge(0);
		SetLabelText("ボーナスステージ");
		SetNumberText(bonusMark);
		SetButtonText(startText);

		if (buttonNumber < playCount)
		{
			//遊び終わったステージ。スコアを伸ばしにもう一度遊べる
			Expansion();
			SetVerticalBarGauge(100);
			SetFrameLineColor(clearedColor);
			SetBackgroundColor(Color.white);
			SetStatusText("クリア", clearedColor);
		}
		else if (buttonNumber == playCount)
		{
			//今遊べるステージ。下線は演出の最後に光らせるので、ここではまだ暗いままにしておく
			Expansion();
			SetFrameLineColor(lockedColor);
			SetBackgroundColor(Color.white);
			SetStatusText("タップしてスタート", currentColor);
		}
		else
		{
			//まだ遊べないステージ
			SetFrameLineColor(lockedColor);
			SetBackgroundColor(lockedBackgroundColor);
			SetStatusText("これから", lockedTextColor);
		}

		//一番上のステージから先は無いので縦棒を消す
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
		if (buttonText == null)
		{
			return;
		}

		buttonText.text = text;
	}

	/// <summary>
	/// ボタンのサイズをセット
	/// </summary>
	/// <param name="x">横幅</param>
	/// <param name="y">縦幅</param>
	public void SetBonusStageSelectButtonSize(float x = 700, float y = 320)
	{
		bonusStageSelectButton.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
	}

	/// <summary>
	/// アイコンの座標をセット
	/// </summary>
	/// <param name="y"></param>
	public void SetIconPos(float y)
	{
		if (icon == null)
		{
			return;
		}

		Vector2 pos = icon.GetComponent<RectTransform>().anchoredPosition;
		pos.y = y;
		icon.GetComponent<RectTransform>().anchoredPosition = pos;
	}

	/// <summary>
	/// 拡大
	/// </summary>
	public void Expansion()
	{
		SetBonusStageSelectButtonSize(y: 320);
		SetButtonGameObject(true);
	}

	/// <summary>
	/// 縮小
	/// </summary>
	public void Reduction()
	{
		SetBonusStageSelectButtonSize(460, 200);
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
		StageSelectManager.AdvancePlayCount(buttonNumber);

		//シーン遷移はフェードを持っているStageSelectManagerに任せる
		StageSelectManager manager = FindObjectOfType<StageSelectManager>();
		if (manager != null)
		{
			manager.BonusStageButton();
		}
	}
}
