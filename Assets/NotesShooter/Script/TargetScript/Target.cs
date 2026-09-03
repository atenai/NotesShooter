using UnityEngine;
using TMPro;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(RedTarget・BlueTarget・PurpleTarget・Doramu・WallTarget)
/// </summary>
public class Target : MonoBehaviour
{
    [Tooltip("ヒットエフェクト")]
    [SerializeField] protected GameObject hitEffectPrefab;
    [Tooltip("的を壊した時に出すレンズフレア")]
    [SerializeField] protected GameObject hitLensFlarePrefab;

    /// <summary>
    /// ヒットエフェクトオブジェクトを生成
    /// </summary>
    protected void HitEffect()
    {
        float hitEffectDestroyTime = hitEffectPrefab.GetComponent<ParticleSystem>().duration;
        GameObject hitEffect = Instantiate(hitEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(hitEffect, hitEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

        LensFlareEffect(hitLensFlarePrefab);

        //当たった手応えを端末の振動でも返す
        HapticFeedback.Play(HapticFeedback.DestroyMilliseconds);
    }

    [Tooltip("レンズフレアが自分で消えなかった時に、念の為に消すまでの秒数")]
    protected float lensFlareDestroyTime = 2.0f;

    /// <summary>
    /// レンズフレアオブジェクトを生成する。
    /// 割り当てが無い的もあるので、その時は何もしない
    /// </summary>
    protected void LensFlareEffect(GameObject lensFlarePrefab)
    {
        if (lensFlarePrefab == null)
        {
            return;
        }

        GameObject lensFlare = Instantiate(lensFlarePrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(lensFlare, lensFlareDestroyTime);
    }

    [Tooltip("ヒットSE")]
    [SerializeField] protected GameObject hitSEPrefab;
    protected float hitSeEndtime = 1.0f;

    /// <summary>
    /// ヒットSEオブジェクトを生成
    /// </summary> 
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
    /// スコアUIテキストオブジェクトを生成
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
    [Tooltip("的が出てきた時に出すレンズフレア")]
    [SerializeField] protected GameObject spawnLensFlarePrefab;
    protected float spawnParticleEffectDestroyTime = 3.0f;

    /// <summary>
    /// スポーンパーティクルオブジェクトを生成する	
    /// </summary>
    protected void SpawnParticleEffect()
    {
        GameObject spawnParticleEffect = Instantiate(spawnParticleEffectPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(spawnParticleEffect, spawnParticleEffectDestroyTime);//エフェクトをDestroyTime後削除

        //的の多くはシーンに置きっぱなしで、シーン読み込みの瞬間に一斉にStartが走る。
        //そのまま出すと開始前に何十個ものフレアが同時に光ってしまうので、
        //プレイが始まってから出てきた的だけを光らせる
        if (IsGameStarted() == true)
        {
            LensFlareEffect(spawnLensFlarePrefab);
        }
    }

    /// <summary>
    /// プレイが始まっているか。GameManagerが居ないテスト用のシーンでは始まっている扱いにする
    /// </summary>
    bool IsGameStarted()
    {
        return GameManager.SingletonInstance == null || GameManager.SingletonInstance.IsGameStarted == true;
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
