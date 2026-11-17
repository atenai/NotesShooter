using UnityEngine;

/// <summary>
/// 音楽のDSP時刻（MusicManager.CurrentMusicTime）に合わせて、ロケットを点火・上昇させる演出用コンポーネント。
/// 噴射口に置いたTransformをRhythmTargetMoverDSPのspawnPointに指定すると、
/// 上昇していくロケットの噴射口から的が出てくる見た目になる。
/// 位置は毎フレーム音楽時間から直接求めるため、途中でアクティブになっても、ポーズを挟んでも同期がずれない。
/// </summary>
public class RocketLaunchDSP : MonoBehaviour
{
    [Header("タイミング（音楽時間・秒）")]
    [Tooltip("噴射を開始する音楽時間。この時点ではまだ上昇せず、機体が小刻みに震えるだけ")]
    [SerializeField] private float ignitionTime = 0f;
    [Tooltip("上昇を開始する音楽時間")]
    [SerializeField] private float liftoffTime = 0f;
    [Tooltip("ロケットを消す音楽時間。0以下なら消さない")]
    [SerializeField] private float despawnTime = 0f;

    [Header("上昇")]
    [Tooltip("上昇の加速度（unit/秒^2）。離床からの上昇量 = 0.5 * 加速度 * 経過秒^2")]
    [SerializeField] private float acceleration = 0.78f;
    [Tooltip("上昇速度の上限（unit/秒）。0以下なら無制限に加速し続ける")]
    [SerializeField] private float maxSpeed = 0f;

    [Header("点火から離床までの震え")]
    [Tooltip("震えの振幅（unit）")]
    [SerializeField] private float shakeAmplitude = 0.05f;
    [Tooltip("震えの速さ")]
    [SerializeField] private float shakeFrequency = 26f;

    [Header("参照")]
    [Tooltip("点火で有効化し、消滅時に無効化する噴射エフェクト")]
    [SerializeField] private GameObject thrustEffect;

    Vector3 basePosition;
    bool thrustOn;

    /// <summary>離床時の高さを基準にした、現在の上昇量（他スクリプトからの参照用）</summary>
    public float CurrentRiseHeight { get; private set; }

    void Awake()
    {
        basePosition = transform.position;

        if (thrustEffect != null)
        {
            //点火時刻まではエフェクトを止めておく
            thrustEffect.SetActive(false);
        }
    }

    void Update()
    {
        if (MusicManager.SingletonInstance == null)
        {
            return;
        }

        double musicTime = MusicManager.SingletonInstance.CurrentMusicTime;

        if (despawnTime > 0f && musicTime >= despawnTime)
        {
            SetThrust(false);
            gameObject.SetActive(false);
            return;
        }

        SetThrust(musicTime >= ignitionTime);

        if (musicTime < ignitionTime)
        {
            CurrentRiseHeight = 0f;
            transform.position = basePosition;
            return;
        }

        if (musicTime < liftoffTime)
        {
            //点火してから離床するまでは、その場で小刻みに震わせる
            CurrentRiseHeight = 0f;
            float phase = (float)musicTime * shakeFrequency;
            transform.position = basePosition + new Vector3(
                Mathf.Sin(phase) * shakeAmplitude,
                0f,
                Mathf.Cos(phase * 1.37f) * shakeAmplitude);
            return;
        }

        CurrentRiseHeight = RiseHeight((float)(musicTime - liftoffTime));
        transform.position = basePosition + Vector3.up * CurrentRiseHeight;
    }

    /// <summary>離床からの経過秒数に対する上昇量。等加速度で上がり、maxSpeedに達した後は等速で上がる</summary>
    float RiseHeight(float elapsed)
    {
        if (elapsed <= 0f)
        {
            return 0f;
        }

        if (maxSpeed > 0f && acceleration > 0f)
        {
            float accelEndTime = maxSpeed / acceleration;
            if (elapsed > accelEndTime)
            {
                //加速をやめた後は等速。加速中に進んだ分（0.5 * maxSpeed * accelEndTime）に足していく
                return (0.5f * maxSpeed * accelEndTime) + (maxSpeed * (elapsed - accelEndTime));
            }
        }

        return 0.5f * acceleration * elapsed * elapsed;
    }

    void SetThrust(bool on)
    {
        if (thrustEffect == null || thrustOn == on)
        {
            return;
        }

        thrustOn = on;
        thrustEffect.SetActive(on);
    }
}
