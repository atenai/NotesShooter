using UnityEngine;

/// <summary>
/// LeftGun（派生クラス）Gun（基底クラス）
/// </summary>
public class LeftGun : Gun
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    public static LeftGun singletonInstance = null;

    public GameObject LeftBullet;

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
        bulletNum = BulletNumReset;//残弾数
        isReloadTime = false;//リロードのオン/オフ
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
        if (Input.GetMouseButtonDown(0) && (bulletNum != 0) && isReloadTime == false)//マウス左クリックが押されたときかつ(tamaが0じゃないとき)
        {
            bulletNum = bulletNum - 1;//残弾数を-1する

            //SEオブジェクトを生成する
            BulletSE();

            //薬莢
            Vector3 v3_Cartridge = new Vector3(this.gameObject.transform.position.x - 0.5f, this.gameObject.transform.position.y, this.gameObject.transform.position.z + 0.5f);

            //薬莢オブジェクトを生成する	
            GameObject newCartridge = Instantiate(GunCartridgePrefab, v3_Cartridge, Quaternion.identity);
            Destroy(newCartridge, GunCartridgeDestroyTime);//3秒後に消す 

            newCartridge.GetComponent<Rigidbody>().AddForce(transform.forward * 250.0f);//速すぎるとすり抜けてしまう
            newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);//速すぎるとすり抜けてしまう
            newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * -200.0f);//速すぎるとすり抜けてしまう

            GameObject newBullet = Instantiate(LeftBullet, transform.position, transform.rotation);

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
        if ((bulletNum == 0 || (bulletNum != BulletNumReset && Input.GetKey(KeyCode.Q))) && isReloadTime == false)
        {
            isReloadTime = true;//リロードのオン
            isReloadSE = true;
        }

        //リロードがオンになったら
        if (isReloadTime == true)
        {
            if (isReloadSE == true)
            {
                //SEオブジェクトを生成する
                ReloadSE();
                isReloadSE = false;
            }

            reloadTime = reloadTime + Time.deltaTime;//リロードタイムをプラス
            if (ReloadTimeDefine <= reloadTime)//リロードタイムがReloadTimeDefineより大きくなったら
            {
                bulletNum = BulletNumReset;//弾リセット
                reloadTime = reloadTimeReset;//リロードタイムをリセット
                isReloadTime = false;//リロードのオフ
            }
        }
    }
}
