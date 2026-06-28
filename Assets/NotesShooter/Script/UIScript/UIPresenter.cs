using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPresenter : MonoBehaviour
{
    [SerializeField] CommonUIView commonUIView;
    [SerializeField] CommonUIPCView commonUIPCView;
    [SerializeField] CommonUISmartPhoneView commonUISmartPhoneView;

    [SerializeField] RightGunUICommonView rightGunUICommonView;
    [SerializeField] RightGunUIPCView rightGunUIPCView;
    [SerializeField] RightGunUISmartPhoneView rightGunUISmartPhoneView;

    [SerializeField] LeftGunUICommonView leftGunUICommonView;
    [SerializeField] LeftGunUIPCView leftGunUIPCView;
    [SerializeField] LeftGunUISmartPhoneView leftGunUISmartPhoneView;

    [SerializeField] RightGun rightGun;
    [SerializeField] LeftGun leftGun;

    void Start()
    {
        commonUISmartPhoneView.PauseButton.onClick.AddListener(GameManager.SingletonInstance.Pause);
        HideUI();

        rightGunUISmartPhoneView.RightShotButton.onClick.AddListener(() => rightGun.Gun.ShotSystem(rightGun.gameObject, rightGun.ShootPoint, rightGun.CartridgePoint));
        rightGunUISmartPhoneView.RightReloadButton.onClick.AddListener(() => rightGun.Gun.ManualReloadTrigger());

        leftGunUISmartPhoneView.LeftShotButton.onClick.AddListener(() => leftGun.Gun.ShotSystem(leftGun.gameObject, leftGun.ShootPoint, leftGun.CartridgePoint));
        leftGunUISmartPhoneView.LeftReloadButton.onClick.AddListener(() => leftGun.Gun.ManualReloadTrigger());
    }

    void HideUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
        commonUISmartPhoneView.HiddenSmartPhoneUI();
        rightGunUISmartPhoneView.HiddenSmartPhoneUI();
        leftGunUISmartPhoneView.HiddenSmartPhoneUI();
#elif UNITY_ANDROID//端末がAndroidだった場合の処理
		commonUIPCView.HiddenPCUI();
        rightGunUIPCView.HiddenPCUI();
        leftGunUIPCView.HiddenPCUI();
#endif//終了
    }

    void Update()
    {
        commonUIView.ChangeRedColorReticleAndCrossHair(FPSCamera.SingletonInstance.IsRayCasthit);
        commonUIView.Pause(GameManager.SingletonInstance.IsPause);

        rightGunUIPCView.UpdateReloadImage(rightGun.Gun.IsReloadTrigger);
        rightGunUIPCView.UpdateBulletText(rightGun.Gun.CurrentBullet);
        rightGunUISmartPhoneView.UpdateReloadImage(rightGun.Gun.IsReloadTrigger);
        rightGunUISmartPhoneView.UpdateBulletText(rightGun.Gun.CurrentBullet);

        leftGunUIPCView.UpdateReloadImage(leftGun.Gun.IsReloadTrigger);
        leftGunUIPCView.UpdateBulletText(leftGun.Gun.CurrentBullet);
        leftGunUISmartPhoneView.UpdateReloadImage(leftGun.Gun.IsReloadTrigger);
        leftGunUISmartPhoneView.UpdateBulletText(leftGun.Gun.CurrentBullet);
    }
}
