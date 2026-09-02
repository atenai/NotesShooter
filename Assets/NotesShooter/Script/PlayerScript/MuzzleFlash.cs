using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 撃った瞬間に銃口で一瞬だけ光る。
/// 明かりとレンズフレアの両方を短い時間で絞って、自分で消える。
/// </summary>
public class MuzzleFlash : MonoBehaviour
{
	[Tooltip("一番明るい時の明かりの強さ")]
	[SerializeField] float lightPeakIntensity = 8.0f;
	[Tooltip("一番明るい時のレンズフレアの強さ")]
	[SerializeField] float flarePeakIntensity = 0.35f;
	[Tooltip("消えるまでの秒数。長いと発砲の切れ味が無くなる")]
	[SerializeField] float lifeTime = 0.07f;

	Light muzzleLight;
	LensFlareComponentSRP lensFlare;
	float elapsed = 0.0f;

	void Awake()
	{
		muzzleLight = GetComponent<Light>();
		lensFlare = GetComponent<LensFlareComponentSRP>();

		//出た瞬間が一番明るい
		Apply(1.0f);
	}

	void Update()
	{
		elapsed += Time.deltaTime;

		if (lifeTime <= elapsed)
		{
			Destroy(gameObject);
			return;
		}

		//二乗で落として、最初だけ強く光って素早く消えるようにする
		float remain = 1.0f - (elapsed / lifeTime);
		Apply(remain * remain);
	}

	void Apply(float rate)
	{
		if (muzzleLight != null)
		{
			muzzleLight.intensity = lightPeakIntensity * rate;
		}

		if (lensFlare != null)
		{
			lensFlare.intensity = flarePeakIntensity * rate;
		}
	}
}
