using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{

    public static int ScoreNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        ScoreNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(ScoreNum);
    }

    public void AddScore(int add_score)
    {
        ScoreNum += add_score;//キューブを壊した際にスコアを+する処理
    }
}
