using UnityEngine;

/// <summary>
/// 青弾クラス
/// </summary>
public class BlueBullet : MonoBehaviour
{
	[SerializeField] float destroyTime = 3.0f;

	void Start()
	{
		Destroy(this.gameObject, destroyTime);
	}

	void OnTriggerEnter(Collider hit)
	{
		if (hit.CompareTag("BlueTarget") || hit.CompareTag("PurpleTarget") || hit.CompareTag("Wall") || hit.CompareTag("Drum"))
		{
			Destroy(this.gameObject);
		}
	}
}
