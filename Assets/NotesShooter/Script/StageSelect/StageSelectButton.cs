using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : StageSelectButtonBase
{
	[SerializeField] GameObject stageSelectButton;
	[SerializeField] GameObject buttonGameObject;
	[SerializeField] Button button;
	[Tooltip("スタートボタンの中の文字")]
	[SerializeField] private Text buttonText;
	[Tooltip("完了マーク")]
	[SerializeField] private GameObject completeMarkGameObject;

	[Tooltip("スタートボタンに出す文字")]
	const string startText = "▶ スタート";

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
		SetCompleteMark(false);
		SetLabelText("ステージ " + buttonNumber.ToString());
		SetNumberText(buttonNumber.ToString());
		SetButtonText(startText);

		if (buttonNumber < playCount)
		{
			//遊び終わったステージ
			SetFrameLineColor(clearedColor);
			SetBackgroundColor(Color.white);
			SetCompleteMark(true);
			SetStatusText("クリア", clearedColor);

			//1つ前のステージのゲージは演出で伸ばすので、それより前だけ最初から満たしておく
			if (buttonNumber < playCount - 1)
			{
				SetVerticalBarGauge(100);
			}
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
	public void SetStageSelectButtonSize(float x = 700, float y = 320)
	{
		stageSelectButton.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
	}

	/// <summary>
	/// 拡大
	/// </summary>
	public void Expansion()
	{
		SetStageSelectButtonSize(y: 320);
		SetButtonGameObject(true);
	}

	/// <summary>
	/// 縮小
	/// </summary>
	public void Reduction()
	{
		SetStageSelectButtonSize(460, 200);
		SetButtonGameObject(false);
	}

	void Start()
	{
		button.onClick.AddListener(OnClick);
	}

	void OnClick()
	{
		StageSelectManager.AdvancePlayCount();

		//シーン遷移はフェードを持っているStageSelectManagerに任せる
		StageSelectManager manager = FindObjectOfType<StageSelectManager>();
		if (manager != null)
		{
			manager.StageButton();
		}
	}
}
