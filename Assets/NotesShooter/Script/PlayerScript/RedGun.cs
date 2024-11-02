using UnityEngine;

/// <summary>
/// RedGun（派生クラス）Gun（基底クラス）
/// </summary>
public class RedGun : Gun
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    public static RedGun singletonInstance = null;

    [SerializeField] RedBullet redBullet;

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
        if (Input.GetMouseButtonDown(1) && (currentBullet != 0) && isReloadTime == false)//マウス右クリックが押されたときかつ(RigthTamaが0じゃないとき)
        {
            currentBullet = currentBullet - 1;//残弾数を-1する

            //SEオブジェクトを生成する
            BulletSE();

            //薬莢
            Vector3 v3_Cartridge = new Vector3(this.gameObject.transform.position.x + 0.5f, this.gameObject.transform.position.y, this.gameObject.transform.position.z + 0.5f);

            //薬莢オブジェクトを生成する	
            GameObject newCartridge = Instantiate(gunCartridgePrefab, v3_Cartridge, Quaternion.identity);
            Destroy(newCartridge, gunCartridgeDestroyTime);

            newCartridge.GetComponent<Rigidbody>().AddForce(transform.forward * 250.0f);//速すぎるとすり抜けてしまう
            newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);//速すぎるとすり抜けてしまう
            newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 200.0f);//速すぎるとすり抜けてしまう

            GameObject newBullet = Instantiate(redBullet.gameObject, transform.position, transform.rotation);

            //前方向に飛ばす 
            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 7000.0f);//速すぎるとすり抜けてしまう

            Destroy(newBullet, 3.0f);
        }
    }

    /// <summary>
    /// リロードシステム
    /// </summary>
    void ReloadSystem()
    {
        //リロードのトリガー
        if ((currentBullet == 0 || (currentBullet != resetBulletNumber && Input.GetKey(KeyCode.E))) && isReloadTime == false)
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
            if (reloadTimeDefine <= reloadTime)//リロードタイムがReloadTimeDefineより大きくなったら
            {
                currentBullet = resetBulletNumber;//残弾数をリセット
                reloadTime = reloadTimeReset;//リロードタイムをリセット
                isReloadTime = false;//リロードのオフ
            }
        }
    }
}
