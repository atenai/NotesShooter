using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    Text score_text;
    //トータルスコア
    int ResultScoreNum;

    void Start()
    {
        ResultScoreNum = 0;//スコア初期化
        //Textコンポーネント取得
        score_text = this.GetComponent<Text>();
        //テキストの文字入力
        score_text.text = ResultScoreNum.ToString();
    }

    void Update()
    {
        ResultScoreNum = Score.ScoreNum;
        //テキストの文字入力
        score_text.text = ResultScoreNum.ToString();
    }
}
