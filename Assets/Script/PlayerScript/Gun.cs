using UnityEngine;

//基底クラス//
//派生クラスのリスト(RightGun・LeftGun)
public class Gun : MonoBehaviour
{

    public int BulletNumReset = 20;
    public int ReloadTime = 0;//リロードタイム
    public int ReloadTimeDefine = 100;
    protected const int reloadTimeReset = 0;

    //弾発射のSE
    public GameObject BulletSEPrefab;
    public float BulletSE_Endtime;

    public void BulletSE()
    {
        //SEオブジェクトを生成する
        GameObject BulletSE = Instantiate(BulletSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(BulletSE, BulletSE_Endtime);//SEをSE_Endtime後削除
    }

    //薬莢プレファブ生成
    public GameObject GunCartridgePrefab;
    public float GunCartridgeDestroyTime;

    //リロードのSE
    public GameObject ReloadSEPrefab;
    public float ReloadSE_Endtime;

    public void ReloadSE()
    {
        //SEオブジェクトを生成する
        GameObject ReloadSE = Instantiate(ReloadSEPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(ReloadSE, ReloadSE_Endtime);//SEをSE_Endtime後削除
    }
}
