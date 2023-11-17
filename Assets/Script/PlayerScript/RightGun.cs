using UnityEngine;

//RightGun（派生クラス）Gun（基底クラス）
public class RightGun : Gun
{
    public GameObject RightBullet;

    public static int rightBulletNum;//残弾数
    public static bool isRightReloadTime = false;//リロードのオン/オフ

    new void Start()
    {
        base.Start();
        rightBulletNum = BulletNumReset;//残弾数
        isRightReloadTime = false;//リロードのオン/オフ
    }

    void Update()
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

        //リロードシステム
        if ((rightBulletNum == 0 || (rightBulletNum != BulletNumReset && Input.GetKey(KeyCode.E))) && isRightReloadTime == false)
        {
            isRightReloadTime = true;//リロードのオン
            isReloadSE = true;
        }

        if (isRightReloadTime == true)//リロードがオンになったら
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
        //リロードシステム
    }
}
