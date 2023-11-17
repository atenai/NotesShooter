using UnityEngine;

public class Score : MonoBehaviour
{
    public static int ScoreNum = 0;

    void Start()
    {
        ScoreNum = 0;
    }

    public void AddScore(int add_score)
    {
        ScoreNum += add_score;//スコアを+する処理
    }
}
