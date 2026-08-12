using UnityEngine;

/// <summary>
/// 音楽のDSP時刻（MusicManager.CurrentMusicTime）を基準に、的をspawnPointからhitPointへ移動させる。
/// targetCubeを指定した場合、出現時刻までは対象を非アクティブのまま保持し、出現時刻でSetActive(true)する
/// （的のStart()で行われるSE/パーティクル再生が、従来のトリガー起動時と同じタイミングで一度だけ鳴るようにするため）。
/// targetCube未指定の場合は自分自身を直接動かす（従来通りのシンプルな挙動）。
/// </summary>
public class RhythmTargetMoverDSP : MonoBehaviour
{
    [Header("対象")]
    [Tooltip("実際に動かす的オブジェクト。未設定の場合はこのGameObject自身を動かす")]
    [SerializeField] private GameObject targetCube;

    [Header("タイミング")]
    [Tooltip("的がヒット地点に到達する音楽時間（秒）。0以下の場合はhitPoint（未設定ならスポーン位置）のワールドZ座標から自動計算する（Z / PlayerMove.ForwardSpeed）")]
    [SerializeField] private float hitTime = 0f;
    [Tooltip("的がスポーン地点からヒット地点に移動する時間（秒）")]
    [SerializeField] private float approachTime = 3.0f;
    [Tooltip("ヒット時刻を過ぎても撃たれなかった場合、消滅させるまでの猶予秒数")]
    [SerializeField] private float missDespawnDelay = 1.0f;

    [Header("軌道")]
    [Tooltip("スポーン地点")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("ヒット地点")]
    [SerializeField] private Transform hitPoint;
    [Tooltip("0より大きい場合、spawnPoint-hitPoint間を2次ベジェで弧を描くように補間する（放物線移動の近似用）")]
    [SerializeField] private float arcHeight = 0f;

    GameObject cube;
    bool isExternalTarget;
    bool destroyed;

    void Awake()
    {
        isExternalTarget = targetCube != null;
        cube = isExternalTarget ? targetCube : this.gameObject;

        if (hitTime <= 0f)
        {
            float z = hitPoint != null ? hitPoint.position.z : (spawnPoint != null ? spawnPoint.position.z : cube.transform.position.z);
            hitTime = z / PlayerMove.ForwardSpeed;
        }

        if (isExternalTarget)
        {
            cube.SetActive(false);
        }
    }

    void Update()
    {
        if (destroyed || cube == null)
        {
            destroyed = true;
            return;
        }

        double currentMusicTime = MusicManager.SingletonInstance.CurrentMusicTime;
        float appearTime = hitTime - approachTime;

        if (isExternalTarget)
        {
            if (currentMusicTime < appearTime)
            {
                //まだ出現時刻に達していない
                return;
            }

            if (!cube.activeSelf)
            {
                //出現時刻に到達した瞬間だけアクティブ化する（Start()のSE/パーティクルが1回だけ鳴る）
                cube.SetActive(true);
            }

            if (currentMusicTime > hitTime + missDespawnDelay)
            {
                //撃たれずにヒット時刻を過ぎたら消滅させる
                destroyed = true;
                Destroy(cube);
                return;
            }
        }

        float t = Mathf.Clamp01((float)((currentMusicTime - appearTime) / approachTime));

        cube.transform.position = arcHeight > 0f
            ? QuadraticBezier(spawnPoint.position, hitPoint.position, arcHeight, t)
            : Vector3.Lerp(spawnPoint.position, hitPoint.position, t);
    }

    static Vector3 QuadraticBezier(Vector3 from, Vector3 to, float arcHeight, float t)
    {
        Vector3 control = Vector3.Lerp(from, to, 0.5f) + Vector3.up * arcHeight;
        Vector3 a = Vector3.Lerp(from, control, t);
        Vector3 b = Vector3.Lerp(control, to, t);
        return Vector3.Lerp(a, b, t);
    }
}
