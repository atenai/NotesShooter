using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ボタンを押した時に少し縮ませて、押した手応えを出すコンポーネント。
/// 指を離すとバネのように少し行き過ぎてから戻るので、跳ね返る感じが出る。
///
/// 色を変えるだけでは、ガラスのシェーダーを使ったボタンで変化が分かりにくい。
/// 大きさが変われば背景に関係なく押した事が伝わるので、こちらで補っている。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIButtonPressEffect : MonoBehaviour,
	IPointerDownHandler, IPointerUpHandler, ICancelHandler
{
	[Tooltip("押している間の大きさ")]
	[SerializeField] float pressedScale = 0.94f;
	[Tooltip("バネの強さ。大きいほど素早く戻る")]
	[SerializeField] float stiffness = 520.0f;
	[Tooltip("バネの減衰。小さいほど長く揺れる")]
	[SerializeField] float damping = 22.0f;

	[Tooltip("これより目標に近く、かつ動きが止まっていたら計算をやめる")]
	const float restThreshold = 0.0008f;

	[Tooltip("今の大きさ")]
	float currentScale = 1.0f;
	[Tooltip("大きさの変化する速さ")]
	float scaleVelocity = 0.0f;
	[Tooltip("今押されているか")]
	bool isPressed = false;

	RectTransform rectTransform;
	Selectable selectable;
	Vector3 baseScale = Vector3.one;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		selectable = GetComponent<Selectable>();

		//元々の大きさを覚えておき、いつもそこを基準に伸び縮みさせる
		baseScale = rectTransform.localScale;
	}

	void OnEnable()
	{
		isPressed = false;
		currentScale = 1.0f;
		scaleVelocity = 0.0f;
		ApplyScale();
	}

	void OnDisable()
	{
		//押したまま消えると縮んだ大きさのまま残るので戻しておく
		isPressed = false;
		currentScale = 1.0f;
		scaleVelocity = 0.0f;
		ApplyScale();
	}

	void Update()
	{
		float target = isPressed == true ? pressedScale : 1.0f;

		//目標に着いて動きも止まっていれば、これ以上計算しない
		if (Mathf.Abs(target - currentScale) < restThreshold && Mathf.Abs(scaleVelocity) < restThreshold)
		{
			if (currentScale != target)
			{
				currentScale = target;
				ApplyScale();
			}
			return;
		}

		//ポーズ中でもボタンは動いてほしいので、時間の倍率に影響されない値を使う
		float deltaTime = Time.unscaledDeltaTime;

		scaleVelocity += (target - currentScale) * stiffness * deltaTime;
		scaleVelocity *= Mathf.Exp(-damping * deltaTime);
		currentScale += scaleVelocity * deltaTime;

		ApplyScale();
	}

	void ApplyScale()
	{
		if (rectTransform == null)
		{
			return;
		}

		rectTransform.localScale = baseScale * currentScale;
	}

	/// <summary>
	/// 押せない状態のボタンは反応させない
	/// </summary>
	bool IsPressable()
	{
		return selectable == null || selectable.IsInteractable() == true;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (IsPressable() == false)
		{
			return;
		}

		isPressed = true;
	}

	/// <summary>
	/// 指を離した時に戻す。
	/// 押したまま指がボタンの外へ出ても離すまでは縮んだままにしている。
	/// uGUIの色の変化がそういう動きなので、大きさだけ先に戻ると食い違って見える
	/// </summary>
	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
	}

	public void OnCancel(BaseEventData eventData)
	{
		isPressed = false;
	}
}
