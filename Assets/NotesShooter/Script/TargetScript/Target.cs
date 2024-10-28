using UnityEngine;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(RightTarget・LeftTarget・PurpleTarget・Doramu・WallTarget)
/// </summary>
public class Target : MonoBehaviour
{
    [Header("爆発エフェクト")]
    public GameObject HitEffectPrefab;

    protected void HitEffect()
    {
        float hitEffectDestroyTime = HitEffectPrefab.GetComponent<ParticleSystem>().duration;
        //Debug.Log("hitEffectDestroyTime : " + hitEffectDestroyTime);
        //爆発エフェクトオブジェクトを生成する	
        GameObject hitEffect = Instantiate(HitEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(hitEffect, hitEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
    }

    [Header("爆発SE")]
    public GameObject HitSEPrefab;
    protected float HitSE_Endtime = 1.0f;

    protected void HitSE()
    {
        //SEオブジェクトを生成する
        GameObject HitSE = Instantiate(HitSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(HitSE, HitSE_Endtime);//SEをSE_Endtime後削除
    }

    [Header("スコアUIのオブジェクトを生成")]
    public int ScoreNum;
    public GameObject ScoreUIPrefab;
    protected float ScoreUI_Endtime = 1.0f;

    protected virtual void ScoreUIText()
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

    [Header("パーティクルエフェクト")]
    public GameObject AppearanceParticleEffectPrefab;
    protected float AppearanceParticleEffectDestroyTime = 3.0f;

    protected void AppearanceParticleEffect()
    {
        //パーティクルオブジェクトを生成する	
        GameObject AppearanceParticleEffect = Instantiate(AppearanceParticleEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(AppearanceParticleEffect, AppearanceParticleEffectDestroyTime);//エフェクトをDestroyTime後削除
    }

    [Header("パーティクルSE")]
    public GameObject AppearanceParticleSEPrefab;
    protected float AppearanceParticleSE_Endtime = 1.0f;

    protected void AppearanceParticleSE()
    {
        //SEオブジェクトを生成する
        GameObject AppearanceParticleSE = Instantiate(AppearanceParticleSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(AppearanceParticleSE, AppearanceParticleSE_Endtime);//SEをSE_Endtime後削除
    }
}
