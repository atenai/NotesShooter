using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    Text score_text;
    //トータルスコア
    private int ResultScoreNum;

    // Start is called before the first frame update
    void Start()
    {
        ResultScoreNum = 0;//スコア初期化
        //Textコンポーネント取得
        score_text = this.GetComponent<Text>();
        //テキストの文字入力
        score_text.text = ResultScoreNum.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ResultScoreNum = Score.ScoreNum;
        //テキストの文字入力
        score_text.text = ResultScoreNum.ToString();
    }
}
