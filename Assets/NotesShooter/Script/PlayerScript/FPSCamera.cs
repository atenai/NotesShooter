using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Androidでのカメラの操作方法
/// </summary>
public enum CameraControlType
{
	//左側のジョイスティックを倒した量で回し続ける（倒しっぱなしで回り続ける）
	Joystick,
	//左側を指でなぞった分だけ回す（マウスと同じで、指を止めればカメラも止まる）
	Swipe,
}

public class FPSCamera : MonoBehaviour
{
	//シングルトンで作成（ゲーム中に１つのみにする）
	private static FPSCamera singletonInstance = null;
	public static FPSCamera SingletonInstance => singletonInstance;

	//プラットフォームごとに#ifで参照する設定が変わる為、参照されない側で「未使用」の警告が出る。設定値はインスペクターで保持したいので警告だけ止める
#pragma warning disable 0414
	[Header("PC（マウス）用の設定")]
	[Tooltip("X軸のカメラの回転スピード")]
	[Range(50, 150)][SerializeField] private float cameraSpeedX = 100;
	[Tooltip("Y軸のカメラの回転スピード")]
	[Range(50, 150)][SerializeField] private float cameraSpeedY = 100;

	[Header("Androidの操作方法")]
	[Tooltip("Joystick：倒した量で回り続ける／Swipe：なぞった分だけ回る（マウスと同じで指を止めればカメラも止まる。行き過ぎにくく狙いやすい）")]
	[SerializeField] private CameraControlType cameraControlType = CameraControlType.Joystick;

	[Header("Android：ジョイスティック操作の設定")]
	[Tooltip("X軸（横回転）の最大回転スピード（度/秒）")]
	[Range(100.0f, 800.0f)][SerializeField] private float touchCameraSpeedX = 220.0f;
	[Tooltip("Y軸（縦回転）の最大回転スピード（度/秒）")]
	[Range(100.0f, 800.0f)][SerializeField] private float touchCameraSpeedY = 150.0f;
	[Tooltip("スティックの効き方のカーブ。1.0で線形、大きくするほど「倒し始めは繊細・倒し切ると速い」になる。行き過ぎる時はここを上げる")]
	[Range(1.0f, 3.0f)][SerializeField] private float touchResponseCurve = 2.0f;
	[Tooltip("スティックのデッドゾーン。この量までの傾きは無視する（指のブレ対策）")]
	[Range(0.0f, 0.3f)][SerializeField] private float touchDeadZone = 0.05f;
	[Tooltip("動き出しをなめらかにする秒数。指のガタつきが消えて手触りが良くなる。戻す時は補間しないので、指を止めた後にカメラが流れる事は無い（0で無効）")]
	[Range(0.0f, 0.2f)][SerializeField] private float touchInputSmoothTime = 0.05f;
	[Tooltip("スティックを大きく倒し続けた時に最大何倍まで加速するか。既定の1.0は加速なし（マウスと同じで倒した量にそのまま比例する動き）")]
	[Range(1.0f, 3.0f)][SerializeField] private float touchTurnBoost = 1.0f;
	[Tooltip("最大倍率まで加速するのにかかる秒数")]
	[Range(0.05f, 1.0f)][SerializeField] private float touchTurnBoostTime = 0.4f;

	[Header("Android：スワイプ操作の設定")]
	[Tooltip("画面の高さ分だけ横になぞった時に回る角度。端末の解像度で感度が変わらないように画面サイズ基準にしている")]
	[Range(60.0f, 600.0f)][SerializeField] private float swipeCameraSpeedX = 240.0f;
	[Tooltip("画面の高さ分だけ縦になぞった時に回る角度")]
	[Range(60.0f, 600.0f)][SerializeField] private float swipeCameraSpeedY = 160.0f;
	[Tooltip("画面の左から何割をカメラ操作に使うか（右側は射撃ボタンなので触らない）")]
	[Range(0.3f, 1.0f)][SerializeField] private float swipeAreaRate = 0.5f;
#pragma warning restore 0414

	[Header("カメラの手触り")]
	[Tooltip("横に振った時にカメラを傾ける最大角度。振っている方向へ体重が乗るような手触りになる（0で無効）")]
	[Range(0.0f, 6.0f)][SerializeField] private float cameraRollAngle = 2.0f;
	[Tooltip("1発撃った時に跳ね上がる角度。跳ね上がった分は自動で元に戻る（0で無効）")]
	[Range(0.0f, 3.0f)][SerializeField] private float recoilKick = 0.6f;
	[Tooltip("跳ね上がった分が戻る速さ（度/秒）")]
	[Range(1.0f, 30.0f)][SerializeField] private float recoilRecoverSpeed = 6.0f;

	[Header("エイムアシスト（的を狙いやすくする補助）")]
	[Tooltip("エイムアシストを使うか")]
	[SerializeField] private bool useAimAssist = true;
	[Tooltip("照準の中心から何度以内の的をアシスト対象にするか")]
	[Range(1.0f, 20.0f)][SerializeField] private float aimAssistAngle = 11.0f;
	[Tooltip("アシスト対象を探す距離")]
	[SerializeField] private float aimAssistRange = 100.0f;
	[Tooltip("的の真ん中を狙っている時にカメラの速さを何倍にするか。小さいほど的の上で止まりやすく、行き過ぎにくくなる（1.0で減速なし）")]
	[Range(0.1f, 1.0f)][SerializeField] private float aimAssistSlowDown = 0.28f;
	[Tooltip("的の方向へ引き寄せる速さ（度/秒）。的が動いても照準が離れにくくなる（0で吸い付きなし）")]
	[Range(0.0f, 150.0f)][SerializeField] private float aimAssistTrackSpeed = 45.0f;
	[Tooltip("壁などに隠れている的をアシスト対象から外すか")]
	[SerializeField] private bool useAimAssistLineOfSight = true;
	[Tooltip("照準が少し外れていても銃だけ的へ向けて当たるようにするか")]
	[SerializeField] private bool useBulletMagnetism = true;
	[Tooltip("銃を的へ向ける許容角度。これ以上外していると当たらない。大きくし過ぎると自動で狙ってくれる感じになる")]
	[Range(0.0f, 8.0f)][SerializeField] private float bulletMagnetismAngle = 3.0f;

	[Tooltip("レイの長さ")]
	[SerializeField] private float range = 100.0f;
	[Tooltip("レティクルの中心点（レイキャスト）にターゲットがヒットしてるか？")]
	private bool isRayCasthit = false;
	public bool IsRayCasthit => isRayCasthit;
	[Tooltip("横回転(Y)はプレイヤーキャラクターの座標位置を使っている(Player_RotY)")]
	private Transform playerTransform;
	[Tooltip("縦回転(X)はカメラの座標位置を使っている(FPSCamera_RotX)")]
	private Transform cameraTransform;
	[Tooltip("縦回転の現在角度を保持する")]
	private float cameraPitch; // カメラの上下回転角度を保持し、Clampで制限するための変数
	[Tooltip("レイキャストの中心点（レティクル）")]
	[SerializeField] private GameObject lookPoint;
	public GameObject LookPoint => lookPoint;

	[Tooltip("今カメラを傾けている角度（Z回転）")]
	private float cameraRoll = 0.0f;
	[Tooltip("傾きをなめらかに変化させる為の作業用")]
	private float cameraRollVelocity = 0.0f;
	[Tooltip("リコイルで跳ね上げた分のうち、まだ戻していない角度")]
	private float pendingRecoilRecovery = 0.0f;
	[Tooltip("なめらかにした後のスティックの傾き")]
	private Vector2 smoothedStickInput = Vector2.zero;
	[Tooltip("スティックを倒し続けている時間。加速（touchTurnBoost）の計算に使う")]
	private float turnBoostTimer = 0.0f;
	[Tooltip("ポーズボタン等のUIを押した指のID。スワイプ操作でカメラを動かさない為に覚えておく")]
	private readonly List<int> ignoredFingerIds = new List<int>();
	[Tooltip("エイムアシストの対象を探す時の作業用。毎フレーム確保しないように使い回す")]
	private readonly Collider[] aimAssistColliders = new Collider[128];
	[Tooltip("今アシスト対象にしている的。対象がチラチラ切り替わらないように前のフレームの物を覚えておく")]
	private Collider lockedAssistCollider = null;
	[Tooltip("今アシスト対象にしている的が居るか")]
	private bool hasAssistTarget = false;
	public bool HasAssistTarget => hasAssistTarget;
	[Tooltip("今アシスト対象にしている的の位置")]
	private Vector3 assistTargetPosition = Vector3.zero;

	//マウスの入力量に掛ける係数。Input.GetAxisは既に「1フレーム分の移動量」なので、Time.deltaTimeでは無く固定値を掛ける（フレームレートで感度が変わらないようにする為）
	private const float MouseDeltaScale = 0.02f;
	//この量以上スティックを倒している時だけ加速させる。細かい照準合わせでは加速させない為
	private const float TurnBoostThreshold = 0.7f;
	//「目一杯操作している」とみなす1秒あたりの回転量（度）。エイムアシストを効かせる強さの計算に使う
	private const float FullInputRotationSpeed = 150.0f;
	//この速さで振った時にカメラの傾きが最大になる（度/秒）
	private const float RollReferenceRotationSpeed = 180.0f;
	//カメラの傾きが変化するのにかかる秒数
	private const float RollSmoothTime = 0.12f;
	//一度ロックした的を維持する角度の倍率。アシスト範囲より少し広い所まで追い続ける
	private const float LockKeepAngleRate = 1.3f;
	//別の的へ乗り換える条件。今の的より「この倍率より近い」的が現れた時だけ乗り換える
	private const float LockSwitchAngleRate = 0.6f;
	// 上を向ける最大角度
	private const float LookingUpAngle = 36.0f;
	// 下を向ける最大角度
	private const float LookingDownAngle = 79.0f;

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

#if !UNITY_EDITOR && UNITY_ANDROID//実機のAndroidだった場合の処理
		//操作方法に合わせてジョイスティックを有効/無効にする
		ApplyCameraControlType();
#endif//終了
	}

	// FixedUpdate（毎秒50回固定）だと描画のフレームレートと噛み合わずカクつき・入力の遅れが出るのでUpdate（描画と同じ回数）で回す
	// UIのEventSystemは実行順が-1000で必ず先に動く為、Updateの時点でジョイスティックやタッチの入力は最新の値になっている
	void Update()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		//マウスの入力は「1フレーム分の移動量」なので、そのまま回転量（度）に変換する
		Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		Vector2 rotationAmount = new Vector2(mouseInput.x * cameraSpeedX, mouseInput.y * cameraSpeedY) * MouseDeltaScale;
#elif UNITY_ANDROID//端末がAndroidだった場合の処理
		Vector2 rotationAmount;
		if (cameraControlType == CameraControlType.Swipe)
		{
			//なぞった距離をそのまま回転量にする（マウスと同じ考え方なので、指を止めればカメラも止まる）
			//画面の高さを基準にして、端末の解像度が変わっても感度が変わらないようにする
			Vector2 swipeDelta = GetSwipeDelta();
			float screenBase = Mathf.Max(1.0f, Screen.height);
			rotationAmount = new Vector2(swipeDelta.x / screenBase * swipeCameraSpeedX, swipeDelta.y / screenBase * swipeCameraSpeedY);
		}
		else
		{
			//ジョイスティックの入力は「傾き（＝回転の速さ）」なので、Time.deltaTimeを掛けて1フレーム分の回転量（度）にする
			Vector2 stickInput = SmoothStickInput(ApplyStickResponse(GetJoystickInput()));
			float turnBoost = UpdateTurnBoost(stickInput.magnitude);
			rotationAmount = new Vector2(stickInput.x * touchCameraSpeedX, stickInput.y * touchCameraSpeedY) * turnBoost * Time.deltaTime;
		}
#endif//終了

		//カウントダウンが終わるまではカメラを動かせないようにする
		if (IsCameraControlEnabled() == false)
		{
			rotationAmount = Vector2.zero;
		}

		RotateCamera(rotationAmount);
	}

	/// <summary>
	/// カメラを操作してよいか。カウントダウン中は動かせない
	/// </summary>
	bool IsCameraControlEnabled()
	{
		//GameManagerが居ないシーン（テスト用シーン等）では常に操作できるようにしておく
		return GameManager.SingletonInstance == null || GameManager.SingletonInstance.IsGameStarted == true;
	}

	/// <summary>
	/// Android用。操作方法に合わせてジョイスティックの有効/無効を切り替える
	/// </summary>
	void ApplyCameraControlType()
	{
		FloatingJoystick joystick = GetJoystick();
		if (joystick == null)
		{
			return;
		}

		bool useJoystick = (cameraControlType == CameraControlType.Joystick);
		//スワイプ操作の時はジョイスティックを止める。透明な操作エリアのImageも当たり判定を切らないと、指の入力がUIに吸われてしまう
		joystick.enabled = useJoystick;
		Image joystickArea = joystick.GetComponent<Image>();
		if (joystickArea != null)
		{
			joystickArea.raycastTarget = useJoystick;
		}
	}

	/// <summary>
	/// Android用。ジョイスティックを取得する
	/// </summary>
	FloatingJoystick GetJoystick()
	{
		if (UIPresenter.SingletonInstance == null || UIPresenter.SingletonInstance.CommonUISmartPhoneView == null)
		{
			return null;
		}

		return UIPresenter.SingletonInstance.CommonUISmartPhoneView.FloatingJoystick;
	}

	/// <summary>
	/// Android用。ジョイスティックの傾きを取得する
	/// </summary>
	Vector2 GetJoystickInput()
	{
		FloatingJoystick joystick = GetJoystick();
		if (joystick == null)
		{
			return Vector2.zero;
		}

		return new Vector2(joystick.Horizontal, joystick.Vertical);
	}

	/// <summary>
	/// Android用。ジョイスティックの傾きにデッドゾーンと効き方のカーブを掛ける
	/// </summary>
	Vector2 ApplyStickResponse(Vector2 stickInput)
	{
		//斜めに倒した時に長さが1を超えるので、方向と傾き量に分けてから傾き量だけを0〜1に収める
		float magnitude = Mathf.Clamp01(stickInput.magnitude);
		if (magnitude <= touchDeadZone)
		{
			return Vector2.zero;
		}

		//デッドゾーン分を差し引いてから0〜1に正規化し直す（デッドゾーンの境目でいきなり動き出さないようにする為）
		float normalizedMagnitude = (magnitude - touchDeadZone) / (1.0f - touchDeadZone);
		//カーブを掛けて「倒し始めは繊細・倒し切ると速い」にする
		float curvedMagnitude = Mathf.Pow(normalizedMagnitude, touchResponseCurve);

		return stickInput.normalized * curvedMagnitude;
	}

	/// <summary>
	/// Android用。スティックの傾きの変化をなめらかにする
	/// </summary>
	Vector2 SmoothStickInput(Vector2 rawStickInput)
	{
		if (touchInputSmoothTime <= 0.0f)
		{
			smoothedStickInput = rawStickInput;
			return smoothedStickInput;
		}

		//倒す方向（入力が増える方向）だけなめらかにする。戻す時までなめらかにすると、指を離した後にカメラが流れて行き過ぎてしまう為
		float changeRate = 1.0f;
		if (smoothedStickInput.sqrMagnitude < rawStickInput.sqrMagnitude)
		{
			//フレームレートが変わっても同じ効き方になる指数移動平均
			changeRate = 1.0f - Mathf.Exp(-Time.deltaTime / touchInputSmoothTime);
		}
		smoothedStickInput = Vector2.Lerp(smoothedStickInput, rawStickInput, changeRate);

		return smoothedStickInput;
	}

	/// <summary>
	/// Android用。スティックを大きく倒し続けている間だけ回転を加速させる倍率を返す
	/// </summary>
	float UpdateTurnBoost(float stickMagnitude)
	{
		//細かい照準合わせ（＝少しだけ倒している状態）では加速させたくないので、大きく倒している時だけタイマーを進める
		if (TurnBoostThreshold <= stickMagnitude)
		{
			turnBoostTimer += Time.deltaTime;
		}
		else
		{
			turnBoostTimer = 0.0f;
		}

		if (touchTurnBoost <= 1.0f || touchTurnBoostTime <= 0.0f)
		{
			return 1.0f;
		}

		return Mathf.Lerp(1.0f, touchTurnBoost, Mathf.Clamp01(turnBoostTimer / touchTurnBoostTime));
	}

	/// <summary>
	/// Android用。この1フレームで画面をなぞった距離（ピクセル）を返す
	/// </summary>
	Vector2 GetSwipeDelta()
	{
		if (Input.touchCount == 0)
		{
			//指が全部離れたら覚えていたIDを捨てる（Endedを取りこぼした時の保険）
			ignoredFingerIds.Clear();
			return Vector2.zero;
		}

		Vector2 swipeDelta = Vector2.zero;

		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);

			if (touch.phase == TouchPhase.Began)
			{
				//ポーズボタン等のUIを押した指はカメラ操作に使わない
				if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId) == true)
				{
					ignoredFingerIds.Add(touch.fingerId);
				}
				continue;
			}

			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				ignoredFingerIds.Remove(touch.fingerId);
				continue;
			}

			if (touch.phase != TouchPhase.Moved)
			{
				continue;
			}
			if (ignoredFingerIds.Contains(touch.fingerId) == true)
			{
				continue;
			}
			//画面の左側から始めた指だけカメラ操作に使う（右側は射撃ボタンの為）
			if (Screen.width * swipeAreaRate < touch.rawPosition.x)
			{
				continue;
			}

			swipeDelta += touch.deltaPosition;
		}

		return swipeDelta;
	}

	/// <summary>
	/// 引数で受け取った1フレーム分の回転量（度）でカメラを回す
	/// </summary>
	void RotateCamera(Vector2 rotationAmount)
	{
		//プレイヤーがどれくらい強く操作しているか（0〜1）。エイムアシストを効かせる強さに使う
		float inputStrength = Mathf.Clamp01(rotationAmount.magnitude / (FullInputRotationSpeed * Time.deltaTime));

		//アシストする的を決める
		UpdateAimAssistTarget();

		//的の近くを狙っている時はカメラを遅くして、行き過ぎ（オーバーシュート）を防ぐ
		if (hasAssistTarget == true)
		{
			rotationAmount *= GetAimAssistSlowDownRate();
		}

		//横回転（Y軸）はプレイヤーキャラクターを回し、縦回転（X軸）はカメラを回す
		float appliedYaw = rotationAmount.x;
		playerTransform.Rotate(0.0f, appliedYaw, 0.0f);
		AddCameraPitch(-rotationAmount.y);

		//撃った時に跳ね上げた分を少しずつ戻す
		UpdateRecoilRecovery();

		//的の方向へ少しだけ引き寄せる（的が動いても照準が離れにくくなる）
		if (hasAssistTarget == true && IsCameraControlEnabled() == true)
		{
			appliedYaw += ApplyAimAssistTracking(inputStrength);
		}

		//実際に回った量に合わせてカメラを傾ける
		UpdateCameraRoll(appliedYaw);

		//回し終わった後の向きでレティクルの位置を決める
		UpdateLookPoint();
	}

	/// <summary>
	/// 縦回転を加算する。上下の最大角度を超えないようにClampして、実際に回した角度を返す
	/// </summary>
	float AddCameraPitch(float deltaPitch)
	{
		float newPitch = Mathf.Clamp(cameraPitch + deltaPitch, -LookingUpAngle, LookingDownAngle);
		float appliedPitch = newPitch - cameraPitch;
		cameraPitch = newPitch;
		ApplyCameraLocalRotation();

		return appliedPitch;
	}

	/// <summary>
	/// 今の縦回転と傾きをカメラに反映する
	/// </summary>
	void ApplyCameraLocalRotation()
	{
		Vector3 localEuler = cameraTransform.localEulerAngles;
		//Z回転（傾き）は見た目だけの物で、前方向（＝弾を撃つ向き）は変わらない
		cameraTransform.localEulerAngles = new Vector3(cameraPitch, localEuler.y, cameraRoll);
	}

	/// <summary>
	/// 横に振った量に応じてカメラを傾ける
	/// </summary>
	void UpdateCameraRoll(float appliedYaw)
	{
		float targetRoll = 0.0f;
		if (0.0f < cameraRollAngle && 0.0f < Time.deltaTime)
		{
			float yawSpeed = appliedYaw / Time.deltaTime;
			//右へ振ったら右へ倒れ込むように、振った向きと逆符号のZ回転にする
			targetRoll = -Mathf.Clamp(yawSpeed / RollReferenceRotationSpeed, -1.0f, 1.0f) * cameraRollAngle;
		}

		cameraRoll = Mathf.SmoothDamp(cameraRoll, targetRoll, ref cameraRollVelocity, RollSmoothTime);
		ApplyCameraLocalRotation();
	}

	/// <summary>
	/// 銃を撃った時に呼ぶ。カメラを少しだけ跳ね上げる（跳ね上げた分は自動で戻る）
	/// </summary>
	public void AddRecoil()
	{
		if (recoilKick <= 0.0f)
		{
			return;
		}

		//cameraPitchは下向きが＋なので、マイナス方向が跳ね上げになる
		float appliedPitch = AddCameraPitch(-recoilKick);
		//上を向ける限界でClampされた場合は、実際に跳ね上がった分だけ戻すようにする
		pendingRecoilRecovery += -appliedPitch;
	}

	/// <summary>
	/// リコイルで跳ね上げた分を少しずつ元に戻す
	/// </summary>
	void UpdateRecoilRecovery()
	{
		if (pendingRecoilRecovery <= 0.0f)
		{
			return;
		}

		float recoverStep = Mathf.Min(pendingRecoilRecovery, recoilRecoverSpeed * Time.deltaTime);
		pendingRecoilRecovery -= recoverStep;
		AddCameraPitch(recoverStep);
	}

	/// <summary>
	/// レイの中心点（レティクル）の位置を更新する。銃はこの位置を向く
	/// </summary>
	void UpdateLookPoint()
	{
		isRayCasthit = false;

		Ray ray = new Ray(this.transform.position, this.transform.forward);
		Debug.DrawRay(ray.origin, ray.direction * range, Color.gray, 1.0f);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, range) == true) // もしRayを投射して何らかのコライダーに衝突したら
		{
			if (IsShootableTarget(hit.collider.gameObject) == true)//※間違ってオブジェクトの設定にレイヤーとタグを間違えるなよおれｗ
			{
				//レイの中心点（レティクル）にターゲットがヒットしている位置を入れる
				lookPoint.transform.position = hit.point;
				isRayCasthit = true;
				return;
			}
		}

		//照準の中心は外れていても、すぐ近くに的が居る場合は銃だけそちらへ向ける（少し外しても当たるようにする補助）
		if (useBulletMagnetism == true && hasAssistTarget == true && GetAngleToPosition(assistTargetPosition) <= bulletMagnetismAngle)
		{
			lookPoint.transform.position = assistTargetPosition;
			//この場合も弾は当たるので、レティクルの色は「当たる」表示にする
			isRayCasthit = true;
			return;
		}

		//レイの中心点（レティクル）にターゲットがヒットしていない位置を入れる
		lookPoint.transform.position = ray.origin + ray.direction * range;
	}

	/// <summary>
	/// 撃てる的かどうか
	/// </summary>
	bool IsShootableTarget(GameObject checkObject)
	{
		return checkObject.CompareTag("BlueTarget") || checkObject.CompareTag("PurpleTarget") || checkObject.CompareTag("RedTarget");
	}

	/// <summary>
	/// カメラの正面から、指定した位置までの角度を返す
	/// </summary>
	float GetAngleToPosition(Vector3 position)
	{
		return Vector3.Angle(cameraTransform.forward, position - cameraTransform.position);
	}

	/// <summary>
	/// エイムアシストの対象を決める。一度決めた的は少し粘って追い続ける（対象がチラチラ切り替わらないようにする為）
	/// </summary>
	void UpdateAimAssistTarget()
	{
		hasAssistTarget = false;

		if (useAimAssist == false)
		{
			lockedAssistCollider = null;
			return;
		}

		//今ロックしている的が、まだ狙える所に居るか調べる
		float lockedAngle = float.MaxValue;
		Vector3 lockedCenter = Vector3.zero;
		if (lockedAssistCollider != null && lockedAssistCollider.gameObject.activeInHierarchy == true)
		{
			lockedCenter = lockedAssistCollider.bounds.center;
			lockedAngle = GetAngleToPosition(lockedCenter);
			//一度ロックした的はアシスト範囲より少し広い所まで維持する
			if (aimAssistAngle * LockKeepAngleRate < lockedAngle || IsVisibleTarget(lockedAssistCollider, lockedCenter) == false)
			{
				lockedAssistCollider = null;
			}
		}
		else
		{
			lockedAssistCollider = null;
		}

		//アシスト範囲の中で照準に一番近い的を探す
		Collider bestCollider = null;
		float bestAngle = float.MaxValue;
		Vector3 bestCenter = Vector3.zero;

		int colliderCount = Physics.OverlapSphereNonAlloc(cameraTransform.position, aimAssistRange, aimAssistColliders);
		for (int i = 0; i < colliderCount; i++)
		{
			Collider targetCollider = aimAssistColliders[i];
			if (targetCollider == null)
			{
				continue;
			}
			if (IsShootableTarget(targetCollider.gameObject) == false)
			{
				continue;
			}

			//コライダーの中心を的の位置として扱う
			Vector3 center = targetCollider.bounds.center;
			float angle = GetAngleToPosition(center);
			//照準から離れすぎている的と、既に見つけた的より照準から遠い的は無視する
			if (aimAssistAngle < angle || bestAngle <= angle)
			{
				continue;
			}
			//壁の向こうに居る的は狙えないので外す
			if (IsVisibleTarget(targetCollider, center) == false)
			{
				continue;
			}

			bestAngle = angle;
			bestCollider = targetCollider;
			bestCenter = center;
		}

		if (lockedAssistCollider != null)
		{
			//今の的より明らかに照準へ近い的が現れた時だけ乗り換える
			if (bestCollider != null && bestCollider != lockedAssistCollider && bestAngle < lockedAngle * LockSwitchAngleRate)
			{
				lockedAssistCollider = bestCollider;
				assistTargetPosition = bestCenter;
			}
			else
			{
				assistTargetPosition = lockedCenter;
			}
			hasAssistTarget = true;
			return;
		}

		if (bestCollider == null)
		{
			return;
		}

		lockedAssistCollider = bestCollider;
		assistTargetPosition = bestCenter;
		hasAssistTarget = true;
	}

	/// <summary>
	/// 的が壁などに隠れていないか調べる
	/// </summary>
	bool IsVisibleTarget(Collider targetCollider, Vector3 targetCenter)
	{
		if (useAimAssistLineOfSight == false)
		{
			return true;
		}

		Vector3 origin = cameraTransform.position;
		Vector3 toTarget = targetCenter - origin;
		float distance = toTarget.magnitude;
		if (distance <= 0.01f)
		{
			return true;
		}

		RaycastHit hit;
		if (Physics.Raycast(origin, toTarget / distance, out hit, distance) == false)
		{
			return true;
		}

		//手前に別の的が居るのは構わない（そちらを狙う事になるだけ）
		return hit.collider == targetCollider || IsShootableTarget(hit.collider.gameObject);
	}

	/// <summary>
	/// 的の近くを狙っている時にカメラの速さを落とす倍率を返す。照準の中心に近いほど遅くなる
	/// </summary>
	float GetAimAssistSlowDownRate()
	{
		//照準の中心に近いほど1に近づく値
		float closeness = 1.0f - Mathf.Clamp01(GetAngleToPosition(assistTargetPosition) / aimAssistAngle);

		return Mathf.Lerp(1.0f, aimAssistSlowDown, closeness);
	}

	/// <summary>
	/// 的の方向へカメラを少しだけ引き寄せる。実際に横へ回した角度を返す
	/// </summary>
	float ApplyAimAssistTracking(float inputStrength)
	{
		if (aimAssistTrackSpeed <= 0.0f)
		{
			return 0.0f;
		}

		Vector3 toTarget = assistTargetPosition - cameraTransform.position;
		if (toTarget.sqrMagnitude <= 0.0001f)
		{
			return 0.0f;
		}

		//カメラを回した後の向きで、まだアシストの範囲内にいるか調べ直す
		float angle = Vector3.Angle(cameraTransform.forward, toTarget);
		if (aimAssistAngle < angle)
		{
			return 0.0f;
		}

		//照準の中心に近いほど強く効かせる
		float closeness = 1.0f - Mathf.Clamp01(angle / aimAssistAngle);
		//自分で大きく動かしている時は弱める（勝手にカメラが動かされる感じを減らす為）
		float inputWeight = Mathf.Lerp(1.0f, 0.35f, inputStrength);
		//1フレームで引き寄せてよい角度
		float maxStep = aimAssistTrackSpeed * closeness * inputWeight * Time.deltaTime;
		if (maxStep <= 0.0f)
		{
			return 0.0f;
		}

		//横（ヨー）の差分を求める
		Vector3 forward = cameraTransform.forward;
		float targetYaw = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
		float currentYaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
		float yawDelta = Mathf.DeltaAngle(currentYaw, targetYaw);

		//縦（ピッチ）の差分を求める。cameraPitchは下向きが＋なのでマイナスを掛けて向きを合わせる
		float targetPitch = -Mathf.Asin(Mathf.Clamp(toTarget.normalized.y, -1.0f, 1.0f)) * Mathf.Rad2Deg;
		float pitchDelta = targetPitch - cameraPitch;

		//残りの差分を超えて回さないようにClampしているので、行き過ぎずに的へ寄っていく
		float yawStep = Mathf.Clamp(yawDelta, -maxStep, maxStep);
		playerTransform.Rotate(0.0f, yawStep, 0.0f);
		AddCameraPitch(Mathf.Clamp(pitchDelta, -maxStep, maxStep));

		return yawStep;
	}
}
