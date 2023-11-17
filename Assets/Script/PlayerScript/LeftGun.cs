using UnityEngine;

//LeftGun（派生クラス）Gun（基底クラス）
public class LeftGun : Gun
{
    public GameObject LeftBullet;

    public static int leftBulletNum;//残弾数
    public static bool isLeftReloadTime = false;//リロードのオン/オフ

    new void Start()
    {
        base.Start();
        leftBulletNum = BulletNumReset;//残弾数
        isLeftReloadTime = false;//リロードのオン/オフ
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (leftBulletNum != 0) && isLeftReloadTime == false)//マウス左クリックが押されたときかつ(tamaが0じゃないとき)
        {
            leftBulletNum = leftBulletNum - 1;//残弾数を-1する

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

        //リロードシステム
        if ((leftBulletNum == 0 || (leftBulletNum != BulletNumReset && Input.GetKey(KeyCode.Q))) && isLeftReloadTime == false)
        {
            isLeftReloadTime = true;//リロードのオン
            isReloadSE = true;
        }

        if (isLeftReloadTime == true)//リロードがオンになったら
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
                leftBulletNum = BulletNumReset;//弾リセット
                reloadTime = reloadTimeReset;//リロードタイムをリセット
                isLeftReloadTime = false;//リロードのオフ
            }
        }
        //リロードシステム
    }
}
