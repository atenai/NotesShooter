using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 左手の銃
/// </summary>
public class LeftGun : MonoBehaviour
{
    [SerializeField] BlueBullet bullet;
    [SerializeField] GameObject bulletSEPrefab;
    [SerializeField] GameObject gunCartridgePrefab;
    [SerializeField] GameObject reloadSEPrefab;
    [SerializeField] GameObject shootPoint;
    public GameObject ShootPoint => shootPoint;
    [SerializeField] GameObject cartridgePoint;
    public GameObject CartridgePoint => cartridgePoint;

    IGun gun;
    public IGun Gun => gun;

    void Awake()
    {
        SetGun(new BlueGun(bullet, bulletSEPrefab, gunCartridgePrefab, reloadSEPrefab));
    }

    void SetGun(IGun newGun)
    {
        gun = newGun;
    }

    void Start()
    {

    }

    void Update()
    {
        //銃がレイの中心点（レティクル）を向くようにする
        this.transform.LookAt(FPSCamera.SingletonInstance.LookPoint.transform.position);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
        //マウス左クリックが押されたとき
        if (Input.GetMouseButtonDown(0))
        {
            gun.ShotSystem(this.gameObject, shootPoint, cartridgePoint);
        }
#endif//終了

        gun.AutoReloadTrigger();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
        //Qキーが押されたらとき
        if (Input.GetKey(KeyCode.Q))
        {
            gun.ManualReloadTrigger();
        }
#endif//終了

        gun.ReloadSystem(this.gameObject);
    }
}
