using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    //縦回転(X)はカメラの座標位置を使っている(FPSCamera)
    //横回転(Y)はプレイヤーキャラクターの座標位置を使っている(Player)


    [SerializeField] public float cameraSpeedX = 100;
    [SerializeField] public float cameraSpeedY = 100;

    private GameObject cameraGameObject;
    private Transform playerTransform;
    private Transform cameraTransform;
    private float cameraAngles;

    void Start()
    {
        cameraGameObject = this.gameObject;
        playerTransform = transform.parent;
        cameraTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        float x_Rotation = Input.GetAxis("Mouse X");
        float y_Rotation = Input.GetAxis("Mouse Y");
        playerTransform.transform.Rotate(0, x_Rotation * cameraSpeedX * Time.deltaTime, 0);

        cameraAngles = cameraGameObject.transform.localEulerAngles.x;
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
