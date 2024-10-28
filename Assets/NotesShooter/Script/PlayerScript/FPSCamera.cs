using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    //最小値は50、最大値は150としてスライダーを表示
    [Range(50, 150)] public float cameraSpeedX = 100;
    //最小値は50、最大値は150としてスライダーを表示
    [Range(50, 150)] public float cameraSpeedY = 100;

    [Tooltip("横回転(Y)はプレイヤーキャラクターの座標位置を使っている(Player_RotY)")]
    private Transform playerTransform;
    [Tooltip("縦回転(X)はカメラの座標位置を使っている(FPSCamera_RotX)")]
    private Transform cameraTransform;

    void Start()
    {
        playerTransform = transform.parent;
        cameraTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        float x_Rotation = Input.GetAxis("Mouse X");
        float y_Rotation = Input.GetAxis("Mouse Y");
        MouseCamera(new Vector2(x_Rotation, y_Rotation));
    }

    void MouseCamera(Vector2 angles)
    {
        float x_Rotation = angles.x;
        float y_Rotation = angles.y;

        //マウスXの入力量 × カメラのスピード × 時間 = の値をX回転の量にする
        playerTransform.transform.Rotate(0, x_Rotation * cameraSpeedX * Time.deltaTime, 0);

        float cameraAngles = cameraTransform.transform.localEulerAngles.x;
        const float lookingUpLimit = 360.0f;//変えてはいけない数値
        float lookingUp = 324.0f;//減らしていくほど上を見れる範囲が広がる
        const float lookingDownLimit = -10.0f;//変えてはいけない数値
        float lookingDown = 79.0f;//増やしていくほど下を見れる範囲が広がる

        if (lookingUp < cameraAngles && cameraAngles < lookingUpLimit || lookingDownLimit < cameraAngles && cameraAngles < lookingDown)//ここの数値を変えればカメラの上下の止まる限界値が変わる
        {
            //マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
            cameraTransform.transform.Rotate(-y_Rotation * cameraSpeedY * Time.deltaTime, 0, 0);
        }
        else
        {
            if (300 < cameraAngles)
            {
                if (Input.GetAxis("Mouse Y") < 0)
                {
                    //マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
                    cameraTransform.transform.Rotate(-y_Rotation * cameraSpeedY * Time.deltaTime, 0, 0);
                }
            }
            else
            {
                if (0 < Input.GetAxis("Mouse Y"))
                {
                    //マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
                    cameraTransform.transform.Rotate(-y_Rotation * cameraSpeedY * Time.deltaTime, 0, 0);
                }

            }
        }
    }
}
