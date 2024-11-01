using UnityEngine;
using TMPro;

/// <summary>
/// PurpleTarget（派生クラス）Target（基底クラス）
/// </summary>
public class PurpleTarget : Target
{
    [Header("紫キューブの体力")]
    [SerializeField] TextMeshPro lifeText;
    int life = 5;

    [Header("ヒットのSE")]
    [SerializeField] GameObject lifeDamageSEPrefab;
    float lifeDamageSeEndtime = 1.0f;

    void Start()
    {
        SpawnSE();
        SpawnParticleEffect();
    }

    void Update()
    {
        lifeText.text = life.ToString();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("LeftBullet") || collider.CompareTag("RightBullet"))
        {
            //Debug.Log("BulletがPurpleCubeに当たったよ");

            life = life - 1;

            //SEオブジェクトを生成する
            GameObject lifeDamageSE = Instantiate(lifeDamageSEPrefab, this.gameObject.transform.position, Quaternion.identity);
            Destroy(lifeDamageSE, lifeDamageSeEndtime);//SEをSE_Endtime後削除

            if (life <= 0)
            {
                TargetDestroy();
            }
        }

        //接触対象はRightBulletまたはLeftBulletタグですか？
        if (collider.CompareTag("DrumCollider"))
        {
            //Debug.Log("DrumColliderがPurpleCubeに当たったよ");

            TargetDestroy();
        }
    }

    void TargetDestroy()
    {

        //爆発エフェクトオブジェクトを生成する	
        HitEffect();

        //SEオブジェクトを生成する
        HitSE();

        //ScoreUITextオブジェクトを生成する
        ScoreUIText();

        Score.singletonInstance.AddScore(scoreNum);

        Destroy(this.gameObject);
    }
}
