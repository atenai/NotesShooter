using UnityEngine;

/// <summary>
/// BlueTarget（派生クラス）Target（基底クラス）
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

            PlayerUI.SingletonInstance.BlueGunUI.IsHitReticule = true;

            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //ScoreTextオブジェクトを生成する
            ScoreUIText();

            GamePlayScore.SingletonInstance.AddScore(scoreNum);

            Destroy(this.gameObject);
        }
    }
}
