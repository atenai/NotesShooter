using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 右手の銃
/// </summary>
public class RightGun : MonoBehaviour
{
    [SerializeField] RedBullet bullet;
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
        SetGun(new RedGun(bullet, bulletSEPrefab, gunCartridgePrefab, reloadSEPrefab));
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
        //マウス右クリックが押されたとき
        if (Input.GetMouseButtonDown(1))
        {
            gun.ShotSystem(this.gameObject, shootPoint, cartridgePoint);
        }
#endif//終了

        gun.AutoReloadTrigger();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
        //Eキーが押されたとき
        if (Input.GetKey(KeyCode.E))
        {
            gun.ManualReloadTrigger();
        }
#endif//終了

        gun.ReloadSystem(this.gameObject);
    }
}
