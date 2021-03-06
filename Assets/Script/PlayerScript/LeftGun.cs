using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//LeftGun（派生クラス）Gun（基底クラス）
public class LeftGun : Gun
{
    //シリアライズフィールド尚化
    public GameObject LeftBullet;

    public static int LeftBulletNum;//残弾数
    public static bool b_LeftReloadTime = false;//リロードのオン/オフ

    delegate int ReloadTimeReset();

    // Start is called before the first frame update
    void Start()
    {
        LeftBulletNum = BulletNumReset;//残弾数
        ReloadTimeReset reloadTimeReset = () => 0;
        ReloadTime = reloadTimeReset();
        b_LeftReloadTime = false;//リロードのオン/オフ
    }

    // Update is called once per frame
    void Update()
    {
        ReloadTimeReset reloadTimeReset = () => 0;

        if (Input.GetMouseButtonDown(0) && (LeftBulletNum != 0) && b_LeftReloadTime == false)//マウス左クリックが押されたときかつ(tamaが0じゃないとき)
        {
            LeftBulletNum = LeftBulletNum - 1;//残弾数を-1する

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
        if (LeftBulletNum == 0 || (LeftBulletNum != BulletNumReset && Input.GetKey(KeyCode.Q)))
        {
            b_LeftReloadTime = true;//リロードのオン
        }

        if (b_LeftReloadTime == true)//リロードがオンになったら
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
                LeftBulletNum = BulletNumReset;//弾リセット
                ReloadTime = reloadTimeReset();//リロードタイムをリセット
                b_LeftReloadTime = false;//リロードのオフ
            }
        }
        //リロードシステム
    }
}
