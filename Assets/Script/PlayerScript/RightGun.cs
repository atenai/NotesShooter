using UnityEngine;

//RightGun（派生クラス）Gun（基底クラス）
public class RightGun : Gun
{
    public GameObject RightBullet;

    public static int RightBulletNum;//残弾数
    public static bool b_RightReloadTime = false;//リロードのオン/オフ

    delegate int ReloadTimeReset();

    void Start()
    {
        RightBulletNum = BulletNumReset;//残弾数
        ReloadTimeReset reloadTimeReset = () => 0;
        ReloadTime = reloadTimeReset();//リロードタイム
        b_RightReloadTime = false;//リロードのオン/オフ
    }

    void Update()
    {
        ReloadTimeReset reloadTimeReset = () => 0;

        if (Input.GetMouseButtonDown(1) && (RightBulletNum != 0) && b_RightReloadTime == false)//マウス右クリックが押されたときかつ(RigthTamaが0じゃないとき)
        {
            RightBulletNum = RightBulletNum - 1;//残弾数を-1する

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

        //リロードシステム
        if (RightBulletNum == 0 || (RightBulletNum != BulletNumReset && Input.GetKey(KeyCode.E)))
        {
            b_RightReloadTime = true;//リロードのオン
        }

        if (b_RightReloadTime == true)//リロードがオンになったら
        {
            if (ReloadTime == reloadTimeReset())
            {
                //SEオブジェクトを生成する
                ReloadSE();
            }
            //リロード中画像
            ReloadTime++;//リロードタイムをプラス
            if (ReloadTime == ReloadTimeDefine)//リロードタイムが100になったら
            {
                RightBulletNum = BulletNumReset;//弾リセット
                ReloadTime = reloadTimeReset();//リロードタイムをリセット
                b_RightReloadTime = false;//リロードのオフ
            }
        }
        //リロードシステム
    }
}
