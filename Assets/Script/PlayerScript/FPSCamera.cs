using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    //横回転(Y)はプレイヤーキャラクターの座標位置を使っている(Player)
    //縦回転(X)はカメラの座標位置を使っている(FPSCamera)


    public GameObject Camera;
    private Transform PlayerTransform;
    private Transform CameraTransform;
    private float ii;

    private void Awake()
    {
        //マウスカーソルを消す
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerTransform = transform.parent;
        CameraTransform = GetComponent<Transform>();


    }

    // Update is called once per frame
    void Update()
    {
        float X_Rotation = Input.GetAxis("Mouse X");
        float Y_Rotation = Input.GetAxis("Mouse Y");
        PlayerTransform.transform.Rotate(0, X_Rotation, 0);

        ii = Camera.transform.localEulerAngles.x;
        if (ii > 324 && ii < 360 || ii > -10 && 79 > ii)//ここの各左の数字を変えればカメラの上下の止まる限界値が変わる
        {
            CameraTransform.transform.Rotate(-Y_Rotation, 0, 0);
            float kk = Y_Rotation;
        }
        else
        {

            if (ii > 300)
            {

                if (Input.GetAxis("Mouse Y") < 0)
                {

                    CameraTransform.transform.Rotate(-Y_Rotation, 0, 0);

                }
            }
            else
            {
                if (Input.GetAxis("Mouse Y") > 0)
                {

                    CameraTransform.transform.Rotate(-Y_Rotation, 0, 0);

                }

            }
        }

    }
}
