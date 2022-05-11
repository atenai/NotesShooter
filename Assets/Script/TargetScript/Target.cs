using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基底クラス//
//派生クラスのリスト(RightTarget・LeftTarget・PurpleTarget・Doramu・WallTarget)
public class Target : MonoBehaviour
{

    //爆発エフェクト
    public GameObject HitEffectPrefab;
    public float HitEffectDestroyTime;

    public void HitEffect()
    {
        //爆発エフェクトオブジェクトを生成する	
        GameObject HitEffect = Instantiate(HitEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(HitEffect, HitEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
    }

    //爆発SE
    public GameObject HitSEPrefab;
    public float HitSE_Endtime;

    public void HitSE()
    {
        //SEオブジェクトを生成する
        GameObject HitSE = Instantiate(HitSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(HitSE, HitSE_Endtime);//SEをSE_Endtime後削除
    }

    //スコアUIのオブジェクトを生成
    public int ScoreNum;
    public GameObject ScoreUIPrefab;
    public float ScoreUI_Endtime;

    public virtual void ScoreUIText()
    {
        //ScoreUITextオブジェクトの生成位置を取得する
        float CubePositionX = this.gameObject.transform.position.x;
        float CubePositionY = this.gameObject.transform.position.y;
        float CubePositionZ = this.gameObject.transform.position.z;
        Vector3 p = new Vector3(CubePositionX + 2.0f, CubePositionY + 2.0f, CubePositionZ);

        //ScoreNumを文字列にして3D_UIで表示
        ScoreUIPrefab.GetComponent<TextMesh>().text = ScoreNum.ToString();

        //ScoreUITextオブジェクトを生成する
        GameObject ScoreUIText = Instantiate(ScoreUIPrefab, p, Quaternion.identity);

        Destroy(ScoreUIText, ScoreUI_Endtime);//ScoreTextをSxoreText_Endtime後削除
    }

    //パーティクルエフェクト
    public GameObject AppearanceParticleEffectPrefab;
    public float AppearanceParticleEffectDestroyTime;

    public void AppearanceParticleEffect()
    {
        //パーティクルオブジェクトを生成する	
        GameObject AppearanceParticleEffect = Instantiate(AppearanceParticleEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(AppearanceParticleEffect, AppearanceParticleEffectDestroyTime);//エフェクトをDestroyTime後削除
    }

    //パーティクルSE
    public GameObject AppearanceParticleSEPrefab;
    public float AppearanceParticleSE_Endtime;

    public void AppearanceParticleSE()
    {
        //SEオブジェクトを生成する
        GameObject AppearanceParticleSE = Instantiate(AppearanceParticleSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(AppearanceParticleSE, AppearanceParticleSE_Endtime);//SEをSE_Endtime後削除
    }

}
