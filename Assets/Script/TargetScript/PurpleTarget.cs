using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PurpleTarget（派生クラス）Target（基底クラス）
public class PurpleTarget : Target
{
    Score score;

    //紫キューブの体力
    public int life = 5;
    public TextMesh LifeText;

    //ヒットのSE
    public GameObject lifeDamageSEPrefab;
    public float lifeDamageSE_Endtime;

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.Find("Canvas").GetComponent<Score>();//ゲームオブジェクトのGameControllerの中にあるコンポーネントのScoreを見つける

        //SEオブジェクトを生成する
        AppearanceParticleSE();

        //パーティクルオブジェクトを生成する	
        AppearanceParticleEffect();
    }

    // Update is called once per frame
    void Update()
    {
        //lifeを文字列にして3D_UIで表示
        LifeText.text = life.ToString();
    }

    //必ずリジットボディーをアタッチする事！
    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {
        //接触対象はRightBulletまたはLeftBulletタグですか？
        if (hit.CompareTag("RightBullet") || hit.CompareTag("LeftBullet"))
        {
            //Debug.Log("BulletがPurpleCubeに当たったよ");

            life = life - 1;

            //SEオブジェクトを生成する
            GameObject lifeDamageSE = Instantiate(lifeDamageSEPrefab, this.gameObject.transform.position, Quaternion.identity);
            Destroy(lifeDamageSE, lifeDamageSE_Endtime);//SEをSE_Endtime後削除

            if (life <= 0)
            {
                //爆発エフェクトオブジェクトを生成する	
                HitEffect();

                //SEオブジェクトを生成する
                HitSE();

                //ScoreUITextオブジェクトを生成する
                ScoreUIText();

                score.AddScore(ScoreNum);//スコアを+する

                Destroy(this.gameObject);//このオブジェクトを削除
            }
        }

        //接触対象はRightBulletまたはLeftBulletタグですか？
        if (hit.CompareTag("DrumCollider"))
        {
            //Debug.Log("DrumColliderがPurpleCubeに当たったよ");


            //爆発エフェクトオブジェクトを生成する	
            HitEffect();

            //SEオブジェクトを生成する
            HitSE();

            //ScoreUITextオブジェクトを生成する
            ScoreUIText();

            score.AddScore(ScoreNum);//スコアを+する

            Destroy(this.gameObject);//このオブジェクトを削除

        }
    }

    public override void ScoreUIText()
    {
        //ScoreUITextオブジェクトの生成位置を取得する
        float CubePositionX = this.gameObject.transform.position.x;
        float CubePositionY = this.gameObject.transform.position.y;
        float CubePositionZ = this.gameObject.transform.position.z;
        Vector3 p = new Vector3(CubePositionX + 10.0f, CubePositionY + 4.0f, CubePositionZ);

        //ScoreNumを文字列にして3D_UIで表示
        ScoreUIPrefab.GetComponent<TextMesh>().text = ScoreNum.ToString();

        //ScoreUITextオブジェクトを生成する
        GameObject ScoreUIText = Instantiate(ScoreUIPrefab, p, Quaternion.identity);

        Destroy(ScoreUIText, ScoreUI_Endtime);//ScoreTextをSxoreText_Endtime後削除
    }
}
