using UnityEngine;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(RedBullet・BlueBullet)
///
/// 弾は1回の物理ステップで約2.8m進むのに、壁の厚みは1mしかない。
/// 物理エンジンは各ステップの位置でしか重なりを見ないので、そのままでは
/// 薄い的を飛び越えてしまう（実測で20発中9発が当たらなかった）。
///
/// Rigidbodyの連続当たり判定はトリガーには効かないので設定では直せない。
/// 前の位置から今の位置までを自分でなぞって、飛び越えた相手の中へ弾を戻している。
/// 戻せば次のステップで普通に重なり、いつものOnTriggerEnterが動く。
/// </summary>
public abstract class Bullet : MonoBehaviour
{
	[Tooltip("なぞった結果を受け取る入れ物。毎回作ると弾の数だけゴミが出るので使い回す")]
	static readonly RaycastHit[] hitBuffer = new RaycastHit[16];

	[Tooltip("前の物理ステップでの位置")]
	Vector3 previousPosition;
	[Tooltip("なぞる時の太さに使う")]
	SphereCollider bulletCollider;
	Rigidbody bulletRigidbody;
	[Tooltip("なぞる対象のレイヤー。物理エンジンが当たりを見る相手と揃えておく")]
	int hitLayerMask;

	protected virtual void Awake()
	{
		bulletCollider = GetComponent<SphereCollider>();
		bulletRigidbody = GetComponent<Rigidbody>();
		previousPosition = this.transform.position;
		hitLayerMask = CreateHitLayerMask(this.gameObject.layer);
	}

	/// <summary>
	/// 自分のレイヤーと当たる事になっているレイヤーだけを集める。
	/// 全レイヤーをなぞると、物理エンジンなら無視する相手で弾が止まってしまう
	/// </summary>
	static int CreateHitLayerMask(int myLayer)
	{
		const int layerCount = 32;
		int mask = 0;

		for (int layer = 0; layer < layerCount; layer++)
		{
			if (Physics.GetIgnoreLayerCollision(myLayer, layer) == true)
			{
				continue;
			}

			mask = mask | (1 << layer);
		}

		return mask;
	}

	void FixedUpdate()
	{
		CatchPassedThrough();

		previousPosition = this.transform.position;
	}

	/// <summary>
	/// 飛び越えてしまった相手を拾って、その中まで弾を戻す
	/// </summary>
	void CatchPassedThrough()
	{
		if (bulletRigidbody == null)
		{
			return;
		}

		Vector3 move = this.transform.position - previousPosition;
		float distance = move.magnitude;

		//まだ動いていない
		const float noMove = 0.0f;
		if (distance <= noMove)
		{
			return;
		}

		Vector3 direction = move / distance;
		float radius = GetWorldRadius();

		int count = Physics.SphereCastNonAlloc(previousPosition, radius, direction, hitBuffer, distance,
			hitLayerMask, QueryTriggerInteraction.Collide);

		Collider nearestCollider = null;
		Vector3 nearestPoint = Vector3.zero;
		float nearestDistance = float.MaxValue;

		for (int i = 0; i < count; i++)
		{
			Collider other = hitBuffer[i].collider;
			if (other == null || other.transform.IsChildOf(this.transform) == true)
			{
				continue;
			}

			//最初から重なっている相手は距離0で返ってくる。
			//それは飛び越えていないので、普通の当たり判定に任せる
			if (hitBuffer[i].distance <= noMove)
			{
				continue;
			}

			if (IsHitTarget(other) == false)
			{
				continue;
			}

			if (nearestDistance <= hitBuffer[i].distance)
			{
				continue;
			}

			nearestDistance = hitBuffer[i].distance;
			nearestPoint = hitBuffer[i].point;
			nearestCollider = other;
		}

		if (nearestCollider == null)
		{
			return;
		}

		//表面より半径の分だけ内側に置く。そうしないと接するだけで重ならない事がある
		bulletRigidbody.position = nearestPoint + direction * radius;

		//止めておかないと、この後の計算でまた飛び越えてしまう。
		//当たれば消える弾なので、止まって見える事はない
		bulletRigidbody.velocity = Vector3.zero;
	}

	/// <summary>
	/// 世界での弾の半径。親の拡大縮小も掛かる
	/// </summary>
	float GetWorldRadius()
	{
		if (bulletCollider == null)
		{
			return 0.0f;
		}

		Vector3 scale = this.transform.lossyScale;
		float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

		return bulletCollider.radius * maxScale;
	}

	/// <summary>
	/// この弾が当たる相手か。
	/// OnTriggerEnterと同じ判断を使い、なぞった時と食い違わないようにする
	/// </summary>
	protected abstract bool IsHitTarget(Collider other);
}
