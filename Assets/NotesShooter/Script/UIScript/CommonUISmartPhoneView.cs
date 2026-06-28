using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonUISmartPhoneView : MonoBehaviour
{
    [Tooltip("ポーズボタン")]
    [SerializeField] Button pauseButton;
    public Button PauseButton => pauseButton;

    [Tooltip("ジョイスティック")]
    [SerializeField] FloatingJoystick floatingJoystick;
    public FloatingJoystick FloatingJoystick => floatingJoystick;

    /// <summary>
    /// スマートフォン用UIを非表示にする
    /// </summary>
    public void HiddenSmartPhoneUI()
    {
        pauseButton.gameObject.SetActive(false);
    }
}
