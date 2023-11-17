using UnityEngine;

/// <summary>
/// RightTarget（派生クラス）Target（基底クラス）
/// </summary>
public class RightTarget : Target
{
    Score score;

    void Start()
    {
        score = GameObject.Find("Canvas").GetComponent<Score>();//ゲームオブジェクトのGameControllerの中にあるコンポーネントのScoreを見つける

        //SEオブジェクトを生成する
        AppearanceParticleSE();

        //パーティクルオブジェクトを生成する	
        AppearanceParticleEffect();
    }

    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {

        //接触対象はRightBulletまたはLeftBulletタグですか？
        if (hit.CompareTag("RightBullet") || hit.CompareTag("DrumCollider"))
        {

            //Debug.Log("RightCubeに当たったよ");

            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //ScoreTextオブジェクトを生成する
            ScoreUIText();

            score.AddScore(ScoreNum);//スコアを+する

            Destroy(this.gameObject);//このオブジェクトを削除
        }
    }

}
