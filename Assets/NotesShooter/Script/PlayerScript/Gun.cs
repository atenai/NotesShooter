using UnityEngine;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(RightGun・LeftGun)
/// </summary>
public class Gun : MonoBehaviour
{
    [Tooltip("残弾数")]
    protected int currentBullet;
    public int CurrentBullet => currentBullet;
    [Tooltip("リロード後の弾のリセットした際の数")]
    protected int resetBulletNumber = 20;
    [Tooltip("リロードタイム")]
    protected float reloadTime = 0.0f;
    protected const float reloadTimeDefine = 1.0f;
    protected const int reloadTimeReset = 0;
    [Tooltip("リロードのオン/オフ")]
    protected bool isReloadTime = false;
    public bool IsReloadTime => isReloadTime;


    [SerializeField] GameObject bulletSEPrefab;
    private float bulletSeEndtime = 1.0f;
    /// <summary>
    /// 弾発射のSE
    /// </summary>
    protected void BulletSE()
    {
        GameObject BulletSE = Instantiate(bulletSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(BulletSE, bulletSeEndtime);
    }

    [Tooltip("薬莢プレファブ")]
    [SerializeField] protected GameObject gunCartridgePrefab;
    protected float gunCartridgeDestroyTime = 3.0f;


    [SerializeField] GameObject reloadSEPrefab;
    private float reloadSeEndtime = 1.0f;
    protected bool isReloadSE = false;
    /// <summary>
    /// リロードのSE
    /// </summary>
    protected void ReloadSE()
    {
        GameObject ReloadSE = Instantiate(reloadSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(ReloadSE, reloadSeEndtime);//SEをSE_Endtime後削除
    }

    protected void Start()
    {
        reloadTime = reloadTimeReset;//リロードタイムをリセット
        currentBullet = resetBulletNumber;//残弾数をリセット
        isReloadTime = false;//リロードのオン/オフ
    }
}
