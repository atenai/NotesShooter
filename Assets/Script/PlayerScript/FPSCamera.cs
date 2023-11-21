using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] public float cameraSpeedX = 100;
    [SerializeField] public float cameraSpeedY = 100;

    [Tooltip("横回転(Y)はプレイヤーキャラクターの座標位置を使っている(Player_RotY)")]
    private Transform playerTransform;
    [Tooltip("縦回転(X)はカメラの座標位置を使っている(FPSCamera_RotX)")]
    private Transform cameraTransform;
    private float cameraAngles;
    float x_Rotation;
    float y_Rotation;

    void Start()
    {
        playerTransform = transform.parent;
        cameraTransform = this.GetComponent<Transform>();

        //↓ゲームプレイ時にカメラの回転軸の数値がおかしくなるバグがあるため、一通り初期化
        Vector3 localAngle = new Vector3(0.0f, 0.0f, 0.0f);
        cameraTransform.transform.localEulerAngles = localAngle; // 回転角度を設定
        playerTransform.transform.localEulerAngles = localAngle;
        x_Rotation = 0.01f;//+にして置けば反転する心配がないはず
        y_Rotation = 0.01f;//+にして置けば反転する心配がないはず
        //↑ゲームプレイ時にカメラの回転軸の数値がおかしくなるバグがあるため、一通り初期化
    }

    void Update()
    {
        x_Rotation = Input.GetAxis("Mouse X");
        y_Rotation = Input.GetAxis("Mouse Y");
        playerTransform.transform.Rotate(0, x_Rotation * cameraSpeedX * Time.deltaTime, 0);

        cameraAngles = cameraTransform.transform.localEulerAngles.x;
        if (324 < cameraAngles && cameraAngles < 360 || -10 < cameraAngles && cameraAngles < 79)//ここの各左の数字を変えればカメラの上下の止まる限界値が変わる
        {

            cameraTransform.transform.Rotate(-y_Rotation * cameraSpeedY * Time.deltaTime, 0, 0);

        }
        else
        {
            if (300 < cameraAngles)
            {
                if (Input.GetAxis("Mouse Y") < 0)
                {

                    cameraTransform.transform.Rotate(-y_Rotation * cameraSpeedY * Time.deltaTime, 0, 0);

                }
            }
            else
            {
                if (0 < Input.GetAxis("Mouse Y"))
                {

                    cameraTransform.transform.Rotate(-y_Rotation * cameraSpeedY * Time.deltaTime, 0, 0);

                }

            }
        }

    }
}
