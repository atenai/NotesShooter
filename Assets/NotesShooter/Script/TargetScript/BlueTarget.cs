using UnityEngine;

/// <summary>
/// LeftTarget（派生クラス）Target（基底クラス）
/// </summary>
public class BlueTarget : Target
{
    void Start()
    {
        SpawnSE();
        SpawnParticleEffect();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("BlueBullet") || hit.CompareTag("DrumCollider"))
        {
            //Debug.Log("LeftCubeに当たったよ");

            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //ScoreTextオブジェクトを生成する
            ScoreUIText();

            Score.singletonInstance.AddScore(scoreNum);

            Destroy(this.gameObject);
        }
    }
}
