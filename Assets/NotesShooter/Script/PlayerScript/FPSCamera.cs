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
	[Tooltip("縦回転の現在角度を保持する")]
	private float cameraPitch; // カメラの上下回転角度を保持し、Clampで制限するための変数

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

		// localEulerAngles.x は 0〜360 の範囲で返るため、-180〜180 に変換して扱いやすくする
		float startX = cameraTransform.localEulerAngles.x;
		if (180.0f < startX)
		{
			startX -= 360.0f;
		}
		cameraPitch = startX; // 初期縦回転角度を cameraPitch に保持
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
		float cameraRotationX = angles.x;
		float cameraRotationY = angles.y;

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
		playerTransform.transform.Rotate(0, cameraRotationX * localCameraSpeedX * Time.deltaTime, 0);

		const float lookingUpAngle = 36.0f;// 上を向ける最大角度
		const float lookingDownAngle = 79.0f;// 下を向ける最大角度

		// 入力に応じた縦回転量を計算し、現在の cameraPitch に加算する
		float deltaPitch = -cameraRotationY * localCameraSpeedY * Time.deltaTime;
		// ここで上下の最大角度を超えないように Clamp する
		cameraPitch = Mathf.Clamp(cameraPitch + deltaPitch, -lookingUpAngle, lookingDownAngle);

		Vector3 localEuler = cameraTransform.localEulerAngles;
		cameraTransform.localEulerAngles = new Vector3(cameraPitch, localEuler.y, localEuler.z);
	}
}
