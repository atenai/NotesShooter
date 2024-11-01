using UnityEngine;
using TMPro;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(RightTarget・LeftTarget・PurpleTarget・Doramu・WallTarget)
/// </summary>
public class Target : MonoBehaviour
{
    [Header("爆発エフェクト")]
    public GameObject hitEffectPrefab;

    protected void HitEffect()
    {
        float hitEffectDestroyTime = hitEffectPrefab.GetComponent<ParticleSystem>().duration;
        GameObject hitEffect = Instantiate(hitEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(hitEffect, hitEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
    }

    [Tooltip("ヒットSE")]
    [SerializeField] protected GameObject hitSEPrefab;
    protected float hitSeEndtime = 1.0f;

    protected void HitSE()
    {
        GameObject hitSe = Instantiate(hitSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(hitSe, hitSeEndtime);
    }

    [Tooltip("スコア数")]
    [SerializeField] protected int scoreNum;
    [Tooltip("スコアUIテキストプレファブ")]
    [SerializeField] protected GameObject scoreUITextPrefab;
    [Tooltip("スコアUIテキストをどれくらいでデストロイするか？の時間")]
    protected float scoreUIendtime = 1.0f;
    [Tooltip("スコアUIテキストの生成位置Xをどれくらいずらすか？")]
    [SerializeField] protected float scoreUITextPosX = 2.0f;
    [Tooltip("スコアUIテキストの生成位置Yをどれくらいずらすか？")]
    [SerializeField] protected float scoreUITextPosY = 2.0f;

    /// <summary>
    /// スコアUIテキストのオブジェクトを生成
    /// </summary>
    protected virtual void ScoreUIText()
    {
        Vector3 pos = new Vector3(this.gameObject.transform.position.x + scoreUITextPosX, this.gameObject.transform.position.y + scoreUITextPosY, this.gameObject.transform.position.z);
        scoreUITextPrefab.GetComponent<TextMeshPro>().text = scoreNum.ToString();
        GameObject scoreUIText = Instantiate(scoreUITextPrefab, pos, Quaternion.identity);
        Destroy(scoreUIText, scoreUIendtime);
    }

    [Header("スポーン")]

    [Tooltip("スポーンパーティクルエフェクト")]
    [SerializeField] GameObject spawnParticleEffectPrefab;
    protected float spawnParticleEffectDestroyTime = 3.0f;

    /// <summary>
    /// スポーンパーティクルオブジェクトを生成する	
    /// </summary>
    protected void SpawnParticleEffect()
    {
        GameObject spawnParticleEffect = Instantiate(spawnParticleEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(spawnParticleEffect, spawnParticleEffectDestroyTime);//エフェクトをDestroyTime後削除
    }

    [Tooltip("スポーンSE")]
    [SerializeField] GameObject spawnSEPrefab;
    protected float spawnSeEndtime = 1.0f;

    /// <summary>
    /// スポーンSEオブジェクトを生成する
    /// </summary>
    protected void SpawnSE()
    {
        GameObject spawnParticleSE = Instantiate(spawnSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(spawnParticleSE, spawnSeEndtime);
    }
}
