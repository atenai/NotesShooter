using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    const float MoveNum = 12.0f;

    /// <summary>
    /// プレイヤーの前進速度（他スクリプトから的の到達タイミング計算などに利用）
    /// </summary>
    public const float ForwardSpeed = MoveNum;

    void FixedUpdate()
    {
        this.transform.Translate(0.0f, 0.0f, MoveNum * Time.deltaTime);
    }
}
