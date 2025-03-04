using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectButtonBase : MonoBehaviour
{
    [Tooltip("バックグラウンド画像")]
    [SerializeField] Image background;
    [Tooltip("縦棒ゲージ画像")]
    [SerializeField] Image verticalBarGauge;

    /// <summary>
    /// メイン画像のカラーを設定
    /// </summary>
    /// <param name="color"></param>
    public void SetBackgroundColor(Color color)
    {
        background.color = color;
    }

    /// <summary>
    /// 縦棒ゲージの数値を指定
    /// </summary>
    /// <param name="current"></param>
    public void SetVerticalBarGauge(float current)
    {
        const int max = 100;
        verticalBarGauge.fillAmount = current / max;
    }
}
