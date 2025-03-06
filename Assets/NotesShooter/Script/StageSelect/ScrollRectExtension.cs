using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ScrollRectの拡張機能を提供します
/// </summary>
public static class ScrollRectExtension
{
	/// <summary>
	/// ScrollRectの上端にGameObjectをあわせる
	/// </summary>
	/// <param name="scrollRect"></param>
	/// <param name="go"></param>
	public static float ScrollToBeBottom(this ScrollRect scrollRect, GameObject go, float speed, UnityAction onComplete = null)
	{
		const float align = 0.0f;
		return ScrollToCore(scrollRect, go, align, speed, onComplete);
	}

	/// <summary>
	/// ScrollRectの縦中央にGameObjectをあわせる
	/// </summary>
	/// <param name="scrollRect"></param>
	/// <param name="go"></param>
	public static float ScrollToCentering(this ScrollRect scrollRect, GameObject go, float speed, UnityAction onComplete = null)
	{
		const float align = 0.5f;
		return ScrollToCore(scrollRect, go, align, speed, onComplete);
	}

	/// <summary>
	/// ScrollRectの下端にGameObjectをあわせる
	/// </summary>
	/// <param name="scrollRect"></param>
	/// <param name="go"></param>
	public static float ScrollToBeTop(this ScrollRect scrollRect, GameObject go, float speed, UnityAction onComplete = null)
	{
		const float align = 1.0f;
		return ScrollToCore(scrollRect, go, align, speed, onComplete);
	}

	/// <summary>
	/// ScrollRectのスクロール位置をGameObjectにあわせる
	/// </summary>
	/// <param name="scrollRect"></param>
	/// <param name="go"></param>
	/// <param name="align">0:下、0.5:中央、1:上</param>
	/// <returns></returns>
	static private float ScrollToCore(ScrollRect scrollRect, GameObject go, float align, float speed, UnityAction onComplete = null)
	{
		var targetRect = go.transform.GetComponent<RectTransform>();
		var contentHeight = scrollRect.content.rect.height;
		var viewportHeight = scrollRect.viewport.rect.height;
		// スクロール不要
		if (contentHeight < viewportHeight) return 0f;

		// ローカル座標が contentHeight の上辺を0として負の値で格納されてる
		// これは現在のレイアウト特有なのかもしれないので、要確認
		var targetPos = contentHeight + GetPosY(targetRect) + targetRect.rect.height * align;
		var gap = viewportHeight * align; // 上端〜下端あわせのための調整量
		var normalizedPos = (targetPos - gap) / (contentHeight - viewportHeight);

		normalizedPos = Mathf.Clamp01(normalizedPos);
		//scrollRect.verticalNormalizedPosition = normalizedPos;
		DOTween.To(() => scrollRect.verticalNormalizedPosition, (x) => scrollRect.verticalNormalizedPosition = x, normalizedPos, speed).SetEase(Ease.Linear).OnComplete(() =>
		{
			onComplete?.Invoke();
		});
		return normalizedPos;
	}

	static private float GetPosY(RectTransform transform)
	{
		return transform.localPosition.y + transform.rect.y; //pivotによるズレをrect.yで補正
	}
}
