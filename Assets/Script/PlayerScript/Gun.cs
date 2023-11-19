using UnityEngine;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(RightGun・LeftGun)
/// </summary>
public class Gun : MonoBehaviour
{
    /// <summary>
    /// リロード
    /// </summary>
    protected int BulletNumReset = 20;
    [Tooltip("リロードタイム")]
    protected float reloadTime = 0.0f;
    protected const float ReloadTimeDefine = 1.0f;
    protected const int reloadTimeReset = 0;

    /// <summary>
    /// 弾発射のSE
    /// </summary>
    public GameObject BulletSEPrefab;
    private float BulletSE_Endtime = 1.0f;

    protected void BulletSE()
    {
        //SEオブジェクトを生成する
        GameObject BulletSE = Instantiate(BulletSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(BulletSE, BulletSE_Endtime);//SEをSE_Endtime後削除
    }

    /// <summary>
    /// 薬莢プレファブ生成
    /// </summary>
    public GameObject GunCartridgePrefab;
    protected float GunCartridgeDestroyTime = 3.0f;

    /// <summary>
    /// リロードのSE
    /// </summary>
    public GameObject ReloadSEPrefab;
    private float ReloadSE_Endtime = 1.0f;
    protected bool isReloadSE = false;

    protected void ReloadSE()
    {
        //SEオブジェクトを生成する
        GameObject ReloadSE = Instantiate(ReloadSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(ReloadSE, ReloadSE_Endtime);//SEをSE_Endtime後削除
    }

    protected void Start()
    {
        Debug.Log("GunのStart()");
        reloadTime = reloadTimeReset;
    }
}
