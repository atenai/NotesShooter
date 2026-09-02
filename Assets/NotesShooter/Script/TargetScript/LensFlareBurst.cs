using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 出した瞬間だけ強く光って、すぐ消えるレンズフレア。
/// 光りっぱなしだと眩しいだけなので、自分で明るさを絞って消える。
/// </summary>
[RequireComponent(typeof(LensFlareComponentSRP))]
public class LensFlareBurst : MonoBehaviour
{
	[Tooltip("一番明るい時の強さ")]
	[SerializeField] float peakIntensity = 1.0f;
	[Tooltip("消えるまでにかける秒数")]
	[SerializeField] float decayTime = 0.35f;

	[Tooltip("最大の明るさに達するまでの秒数。0だと出た瞬間に眩しい")]
	const float attackTime = 0.05f;

	LensFlareComponentSRP lensFlare;
	float elapsed = 0.0f;

	void Awake()
	{
		lensFlare = GetComponent<LensFlareComponentSRP>();
		lensFlare.intensity = 0.0f;
	}

	void Update()
	{
		elapsed += Time.deltaTime;

		if (elapsed < attackTime)
		{
			//ぱっと明るくなるところ
			lensFlare.intensity = peakIntensity * (elapsed / attackTime);
			return;
		}

		float rate = decayTime <= 0.0f ? 1.0f : (elapsed - attackTime) / decayTime;
		if (1.0f <= rate)
		{
			Destroy(gameObject);
			return;
		}

		//最後にすっと消えるよう、二乗で落とす
		float remain = 1.0f - rate;
		lensFlare.intensity = peakIntensity * remain * remain;
	}
}
