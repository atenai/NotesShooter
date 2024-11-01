using UnityEngine;

/// <summary>
/// RightTarget（派生クラス）Target（基底クラス）
/// </summary>
public class RightTarget : Target
{
    void Start()
    {
        //SEオブジェクトを生成する
        AppearanceParticleSE();

        //パーティクルオブジェクトを生成する	
        AppearanceParticleEffect();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("RightBullet") || hit.CompareTag("DrumCollider"))
        {
            //Debug.Log("RightCubeに当たったよ");

            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //ScoreTextオブジェクトを生成する
            ScoreUIText();

            Score.singletonInstance.AddScore(ScoreNum);//スコアを+する

            Destroy(this.gameObject);//このオブジェクトを削除
        }
    }

}
