using UnityEngine;

public class FPSCamera : MonoBehaviour
{
	//シングルトンで作成（ゲーム中に１つのみにする）
	private static FPSCamera singletonInstance = null;
	public static FPSCamera SingletonInstance => singletonInstance;

	[Tooltip("X軸のカメラの回転スピード")]
	[Range(50, 150)][SerializeField] private float cameraSpeedX = 100;
	[Tooltip("Y軸のカメラの回転スピード")]
	[Range(50, 150)][SerializeField] private float cameraSpeedY = 100;
	[Tooltip("ローカルで計算する為のX軸のカメラの回転スピード")]
	private float localCameraSpeedX;
	[Tooltip("ローカルで計算する為のY軸のカメラの回転スピード")]
	private float localCameraSpeedY;
	[Tooltip("カメラのスピードを遅くする")]
	[Range(1.0f, 4.0f)] private float slowDownCameraSpeed = 2.0f;
	[Tooltip("レイの長さ")]
	[SerializeField] private float range = 100.0f;
	[Tooltip("レティクルの中心点（レイキャスト）にターゲットがヒットしてるか？")]
	private bool isTargethit = false;
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
#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		float xRotation = Input.GetAxis("Mouse X");
		float yRotation = Input.GetAxis("Mouse Y");
#elif UNITY_ANDROID//端末がAndroidだった場合の処理
		float xRotation = PlayerUI.SingletonInstance.FloatingJoystick.Horizontal;
		float yRotation = PlayerUI.SingletonInstance.FloatingJoystick.Vertical;
#endif//終了
		MouseCamera(new Vector2(xRotation, yRotation));
	}

	void MouseCamera(Vector2 angles)
	{
		float xRotation = angles.x;
		float yRotation = angles.y;

		localCameraSpeedX = cameraSpeedX;
		localCameraSpeedY = cameraSpeedY;
		isTargethit = false;

		//ターゲットにあたった際にカメラを遅くする処理
		Ray ray = new Ray(this.transform.position, this.transform.forward);
		Debug.DrawRay(ray.origin, ray.direction * range, Color.gray, 1.0f);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, range) == true) // もしRayを投射して何らかのコライダーに衝突したら
		{
			if (hit.collider.gameObject.CompareTag("BlueTarget") || hit.collider.gameObject.CompareTag("PurpleTarget") || hit.collider.gameObject.CompareTag("RedTarget"))//※間違ってオブジェクトの設定にレイヤーとタグを間違えるなよおれｗ
			{
				//カメラの速さを遅くする
				localCameraSpeedX = cameraSpeedX / slowDownCameraSpeed;
				localCameraSpeedY = cameraSpeedY / slowDownCameraSpeed;
				isTargethit = true;
			}
		}

		//マウスXの入力量 × カメラのスピード × 時間 = の値をX回転の量にする
		playerTransform.transform.Rotate(0, xRotation * localCameraSpeedX * Time.deltaTime, 0);

		float cameraAngles = cameraTransform.transform.localEulerAngles.x;
		const float lookingUpLimit = 360.0f;//変えてはいけない数値
		float lookingUp = 324.0f;//減らしていくほど上を見れる範囲が広がる
		const float lookingDownLimit = -10.0f;//変えてはいけない数値
		float lookingDown = 79.0f;//増やしていくほど下を見れる範囲が広がる

		if (lookingUp < cameraAngles && cameraAngles < lookingUpLimit || lookingDownLimit < cameraAngles && cameraAngles < lookingDown)//ここの数値を変えればカメラの上下の止まる限界値が変わる
		{
			//マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
			cameraTransform.transform.Rotate(-yRotation * localCameraSpeedY * Time.deltaTime, 0, 0);
		}
		else
		{
			if (300 < cameraAngles)
			{
				if (yRotation < 0)
				{
					//マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
					cameraTransform.transform.Rotate(-yRotation * localCameraSpeedY * Time.deltaTime, 0, 0);
				}
			}
			else
			{
				if (0 < yRotation)
				{
					//マウスYの入力量 × カメラのスピード × 時間 = の値をY回転の量にする
					cameraTransform.transform.Rotate(-yRotation * localCameraSpeedY * Time.deltaTime, 0, 0);
				}

			}
		}
	}
}
