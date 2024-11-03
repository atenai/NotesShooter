using UnityEngine;

/// <summary>
/// RedTarget（派生クラス）Target（基底クラス）
/// </summary>
public class RedTarget : Target
{
    void Start()
    {
        SpawnSE();
        SpawnParticleEffect();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("RedBullet") || hit.CompareTag("DrumCollider"))
        {
            //Debug.Log("RightCubeに当たったよ");

            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //ScoreTextオブジェクトを生成する
            ScoreUIText();

            GamePlayScore.singletonInstance.AddScore(scoreNum);

            Destroy(this.gameObject);
        }
    }

}
