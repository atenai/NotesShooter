using UnityEngine;

/// <summary>
/// Doramu（派生クラス）Target（基底クラス）
/// </summary>
public class DoramuTarget : Target
{
    [Header("爆発のあたり判定オブジェクトを生成")]
    public GameObject DoramuColliderPrefab;
    float DoramuColliderDestroyTime = 1.0f;

    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {
        //接触対象はRightBulletまたはLeftBulletタグですか？
        if (hit.CompareTag("LeftBullet") || hit.CompareTag("RightBullet"))
        {
            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //爆発のあたり判定オブジェクトを生成する	
            GameObject DoramuCollider = Instantiate(DoramuColliderPrefab, this.gameObject.transform.position, Quaternion.identity);
            Destroy(DoramuCollider, DoramuColliderDestroyTime);//爆発オブジェクトをDestroyTime後削除

            Destroy(this.gameObject);//このオブジェクトを削除
        }
    }
}
