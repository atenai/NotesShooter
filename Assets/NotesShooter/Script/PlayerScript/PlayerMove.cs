using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    const float MoveNum = 12.0f;

    /// <summary>
    /// プレイヤーの前進速度（他スクリプトから的の到達タイミング計算などに利用）
    /// </summary>
    public const float ForwardSpeed = MoveNum;

    [Tooltip("音楽の時間から進んだ位置を出す為の、スタート地点")]
    Vector3 startPosition;

    [Tooltip("1回の物理更新で進める最大距離。処理落ちから追いつく時に、薄いゴールの判定をすり抜けない為の上限")]
    const float MaxCatchUpDistance = 1.0f;

    void Awake()
    {
        startPosition = this.transform.position;
    }

    void FixedUpdate()
    {
        //カウントダウンが終わるまでは前に進まない
        if (GameManager.SingletonInstance != null && GameManager.SingletonInstance.IsGameStarted == false)
        {
            return;
        }

        if (MusicManager.SingletonInstance == null)
        {
            //音楽を置いていないテスト用のシーンでは、今まで通り経過時間で進める
            this.transform.Translate(0.0f, 0.0f, MoveNum * Time.deltaTime);
            return;
        }

        //的は音楽の時間（dspTime）を見て動いている。プレイヤーも同じ時計で位置を決めないと、
        //経過時間を足していく方式では処理落ちで落とした分がそのまま的との「ずれ」になって溜まっていく
        float goalZ = startPosition.z + MoveNum * (float)MusicManager.SingletonInstance.CurrentMusicTime;

        //大きく処理落ちした後に一気に飛ばすと、ゴールの判定（Z方向の厚み1）をすり抜けてしまう。
        //1回に進める距離を抑えて、何回かの物理更新に分けて追いつかせる
        Vector3 position = this.transform.position;
        float step = Mathf.Clamp(goalZ - position.z, -MaxCatchUpDistance, MaxCatchUpDistance);

        this.transform.position = new Vector3(position.x, position.y, position.z + step);
    }
}
