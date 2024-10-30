using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    static FPSCamera singletonInstance = null;
    public static FPSCamera SingletonInstance => singletonInstance;

    [Tooltip("X軸のカメラの回転スピード")]
    [Range(50, 150)] public float cameraSpeedX = 100;
    [Tooltip("Y軸のカメラの回転スピード")]
    [Range(50, 150)] public float cameraSpeedY = 100;
    [Tooltip("ローカルで計算する為のX軸のカメラの回転スピード")]
    float localCameraSpeedX;
    [Tooltip("ローカルで計算する為のY軸のカメラの回転スピード")]
    float localCameraSpeedY;
    [Tooltip("カメラのスピードを遅くする")]
    [Range(1.0f, 4.0f)] float slowDownCameraSpeed = 2.0f;
    [Tooltip("レイの長さ")]
    [SerializeField] float range = 100.0f;
    [Tooltip("レティクルの中心点（レイキャスト）にターゲットがヒットしてるか？")]
    bool isTargethit = false;
    public bool IsTargethit => isTargethit;
    [Tooltip("横回転(Y)はプレイヤーキャラクターの座標位置を使っている(Player_RotY)")]
    private Transform playerTransform;
    [Tooltip("縦回転(X)はカメラの座標位置を使っている(FPSCamera_RotX)")]
    private Transform cameraTransform;

    void Awake()
    {
        //staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
        if (singletonInstance == null)
        {
            singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
        }
        else
        {
            Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
        }
    }

    void Start()
    {
        playerTransform = transform.parent;
        cameraTransform = this.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        float x_Rotation = Input.GetAxis("Mouse X");
        float y_Rotation = Input.GetAxis("Mouse Y");
        MouseCamera(new Vector2(x_Rotation, y_Rotation));
    }

    void MouseCamera(Vector2 angles)
    {
        float x_Rotation = angles.x;
        float y_Rotation = angles.y;

        localCameraSpeedX = cameraSpeedX;
        localCameraSpeedY = cameraSpeedY;
        isTargethit = false;

        //ターゲットにあたった際にカメラを遅くする処理
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.gray, 1.0f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range) == true) // もしRayを投射して何らかのコライダーに衝突したら
        {
            if (hit.collider.gameObject.CompareTag("LeftTarget") || hit.collider.gameObject.CompareTag("PurpleTarget") || hit.collider.gameObject.CompareTag("RightTarget"))//※間違ってオブジェクトの設定にレイヤーとタグを間違えるなよおれｗ
            {
                //カメラの速さを遅くする
                localCameraSpeedX = cameraSpeedX / slowDownCameraSpeed;
                localCameraSpeedY = cameraSpeedY / slowDownCameraSpeed;
                isTargethit = true;
            }
        }

        //マウスXの入力量 × カメラのスピード × 時間 = の値をX回転の量にする
        playerTransform.transform.Rotate(0, x_Rotation * localCameraSpeedX * Time.deltaTime, 0);

        float cameraAngles = cameraTransform.transform.localEulerAngles.x;
        const float lookingUpLimit = 360.0f;//変えてはいけない数値
        float lookingUp = 324.0f;//減らしていくほど上を見れる範囲が広がる
        const float lookingDownLimit = -10.0f;//変えてはいけない数値
        float lookingDown = 79.0f;//増やしていくほど下を見れる範囲が広がる

        if (lookingUp < cameraAngles && cameraAngles < lookingUpLimit || lookingDownLimit < cameraAngles && cameraAngles < lookingDown)//ここの数値を変えればカメラの上下の止まる限界値が変わる
        {
            //マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
            cameraTransform.transform.Rotate(-y_Rotation * localCameraSpeedY * Time.deltaTime, 0, 0);
        }
        else
        {
            if (300 < cameraAngles)
            {
                if (Input.GetAxis("Mouse Y") < 0)
                {
                    //マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
                    cameraTransform.transform.Rotate(-y_Rotation * localCameraSpeedY * Time.deltaTime, 0, 0);
                }
            }
            else
            {
                if (0 < Input.GetAxis("Mouse Y"))
                {
                    //マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
                    cameraTransform.transform.Rotate(-y_Rotation * localCameraSpeedY * Time.deltaTime, 0, 0);
                }

            }
        }
    }
}
