using UnityEngine;

public class RhythmTargetMoverDSP : MonoBehaviour
{
    [Tooltip("的がヒット地点に到達する音楽時間（秒）")]
    [SerializeField] private float hitTime = 3.0f;
    [Tooltip("的がスポーン地点からヒット地点に移動する時間（秒）")]
    [SerializeField] private float approachTime = 2.0f;
    [Tooltip("スポーン地点")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("ヒット地点")]
    [SerializeField] private Transform hitPoint;

    private void Update()
    {
        double currentMusicTime = AudioSettings.dspTime - MusicManager.SingletonInstance.MusicStartDspTime;

        float appearTime = hitTime - approachTime;

        float t = (float)((currentMusicTime - appearTime) / approachTime);
        t = Mathf.Clamp01(t);

        this.transform.position = Vector3.Lerp(spawnPoint.position, hitPoint.position, t);
    }
}