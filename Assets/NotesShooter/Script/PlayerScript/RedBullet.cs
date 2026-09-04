using UnityEngine;

/// <summary>
/// 赤弾クラス
/// </summary>
public class RedBullet : Bullet
{
	[SerializeField] float destroyTime = 3.0f;

	void Start()
	{
		Destroy(this.gameObject, destroyTime);
	}

	void OnTriggerEnter(Collider hit)
	{
		if (IsHitTarget(hit) == true)
		{
			Destroy(this.gameObject);
		}
	}

	protected override bool IsHitTarget(Collider other)
	{
		return other.CompareTag("RedTarget") || other.CompareTag("PurpleTarget")
			|| other.CompareTag("Wall") || other.CompareTag("Drum");
	}
}
