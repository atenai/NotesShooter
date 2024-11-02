using UnityEngine;

/// <summary>
/// Doramu（派生クラス）Target（基底クラス）
/// </summary>
public class DoramuTarget : Target
{
    [Tooltip("爆発のあたり判定オブジェクトを生成")]
    [SerializeField] GameObject doramuColliderPrefab;
    float doramuColliderDestroyTime = 1.0f;

    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("BlueBullet") || hit.CompareTag("RedBullet"))
        {
            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //爆発のあたり判定オブジェクトを生成する	
            GameObject doramuCollider = Instantiate(doramuColliderPrefab, this.gameObject.transform.position, Quaternion.identity);
            Destroy(doramuCollider, doramuColliderDestroyTime);//爆発オブジェクトをDestroyTime後削除

            Destroy(this.gameObject);//このオブジェクトを削除
        }
    }
}
