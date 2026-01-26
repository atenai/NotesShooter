using UnityEngine;

/// <summary>
/// 赤弾クラス
/// </summary>
public class RedBullet : MonoBehaviour
{
	[SerializeField] float destroyTime = 3.0f;

	void Start()
	{
		Destroy(this.gameObject, destroyTime);
	}

	void OnTriggerEnter(Collider hit)
	{
		if (hit.CompareTag("RedTarget") || hit.CompareTag("PurpleTarget") || hit.CompareTag("Wall") || hit.CompareTag("Drum"))
		{
			Destroy(this.gameObject);
		}
	}
}
