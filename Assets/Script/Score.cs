using UnityEngine;

public class Score : MonoBehaviour
{
    int scoreNum = 0;
    public int ScoreNum => scoreNum;

    public void AddScore(int add_score)
    {
        scoreNum += add_score;//スコアを+する処理
    }
}
