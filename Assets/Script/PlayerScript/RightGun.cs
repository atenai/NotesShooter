using UnityEngine;

/// <summary>
/// RightGun（派生クラス）Gun（基底クラス）
/// </summary>
public class RightGun : Gun
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    public static RightGun singletonInstance = null;

    public GameObject RightBullet;
    public int rightBulletNum;//残弾数
    public bool isRightReloadTime = false;//リロードのオン/オフ

    void Awake()
    {
        //staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
        if (singletonInstance == null)
        {
            singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
        }
        else
        {
            Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
        }
    }

    new void Start()
    {
        base.Start();
        rightBulletNum = BulletNumReset;//残弾数
        isRightReloadTime = false;//リロードのオン/オフ
    }

    void Update()
    {
        ShotSystem();

        ReloadSystem();
    }

    /// <summary>
    /// ショットシステム
    /// </summary>
    void ShotSystem()
    {
        if (Input.GetMouseButtonDown(1) && (rightBulletNum != 0) && isRightReloadTime == false)//マウス右クリックが押されたときかつ(RigthTamaが0じゃないとき)
        {
            rightBulletNum = rightBulletNum - 1;//残弾数を-1する

            //SEオブジェクトを生成する
            BulletSE();

            //薬莢
            Vector3 v3_Cartridge = new Vector3(this.gameObject.transform.position.x + 0.5f, this.gameObject.transform.position.y, this.gameObject.transform.position.z + 0.5f);

            //薬莢オブジェクトを生成する	
            GameObject newCartridge = Instantiate(GunCartridgePrefab, v3_Cartridge, Quaternion.identity);
            Destroy(newCartridge, GunCartridgeDestroyTime);//3秒後に消す 

            newCartridge.GetComponent<Rigidbody>().AddForce(transform.forward * 250.0f);//速すぎるとすり抜けてしまう
            newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);//速すぎるとすり抜けてしまう
            newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 200.0f);//速すぎるとすり抜けてしまう

            GameObject newBullet = Instantiate(RightBullet, transform.position, transform.rotation);

            //前方向に飛ばす 
            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 7000.0f);//速すぎるとすり抜けてしまう

            Destroy(newBullet, 3.0f);//3秒後に消す 
        }
    }

    /// <summary>
    /// リロードシステム
    /// </summary>
    void ReloadSystem()
    {
        //リロードのトリガー
        if ((rightBulletNum == 0 || (rightBulletNum != BulletNumReset && Input.GetKey(KeyCode.E))) && isRightReloadTime == false)
        {
            isRightReloadTime = true;//リロードのオン
            isReloadSE = true;
        }

        //リロードがオンになったら
        if (isRightReloadTime == true)
        {
            if (isReloadSE == true)
            {
                //SEオブジェクトを生成する
                ReloadSE();
                isReloadSE = false;
            }

            reloadTime = reloadTime + Time.deltaTime;//リロードタイムをプラス
            //Debug.Log("reloadTIme : " + reloadTime);
            if (ReloadTimeDefine <= reloadTime)//リロードタイムがReloadTimeDefineより大きくなったら
            {
                rightBulletNum = BulletNumReset;//弾リセット
                reloadTime = reloadTimeReset;//リロードタイムをリセット
                isRightReloadTime = false;//リロードのオフ
            }
        }
    }
}
