using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    //横回転(Y)はプレイヤーキャラクターの座標位置を使っている(Player)
    //縦回転(X)はカメラの座標位置を使っている(FPSCamera)

    public GameObject Camera;
    private Transform PlayerTransform;
    private Transform CameraTransform;
    private float cameraAngles;

    private void Awake()
    {
        //マウスカーソルを消す
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        PlayerTransform = transform.parent;
        CameraTransform = GetComponent<Transform>();
    }

    void Update()
    {
        float X_Rotation = Input.GetAxis("Mouse X");
        float Y_Rotation = Input.GetAxis("Mouse Y");
        PlayerTransform.transform.Rotate(0, X_Rotation, 0);

        cameraAngles = Camera.transform.localEulerAngles.x;
        if (324 < cameraAngles && cameraAngles < 360 || -10 < cameraAngles && cameraAngles < 79)//ここの各左の数字を変えればカメラの上下の止まる限界値が変わる
        {

            CameraTransform.transform.Rotate(-Y_Rotation, 0, 0);

        }
        else
        {
            if (300 < cameraAngles)
            {
                if (Input.GetAxis("Mouse Y") < 0)
                {

                    CameraTransform.transform.Rotate(-Y_Rotation, 0, 0);

                }
            }
            else
            {
                if (0 < Input.GetAxis("Mouse Y"))
                {

                    CameraTransform.transform.Rotate(-Y_Rotation, 0, 0);

                }

            }
        }

    }
}
