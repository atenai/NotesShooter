using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    [SerializeField] Text scoreText;

    int resultScoreNum = 0;

    void Start()
    {
        resultScoreNum = Score.ScoreNum;
        scoreText.text = resultScoreNum.ToString();
    }
}
