using UnityEngine;

/// <summary>
/// 青弾クラス
/// </summary>
public class BlueBullet : Bullet
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
		return other.CompareTag("BlueTarget") || other.CompareTag("PurpleTarget")
			|| other.CompareTag("Wall") || other.CompareTag("Drum");
	}
}
