using UnityEngine;

public class Goal : MonoBehaviour
{
    bool isGoal = false;
    public bool IsGoal => isGoal;

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            isGoal = true;

            Score score = GameObject.Find("Canvas").GetComponent<Score>();

            //「SCORE」というキーで、Int値の「score.ScoreNum」を保存
            PlayerPrefs.SetInt("SCORE", score.ScoreNum);
            PlayerPrefs.Save();
        }
    }
}
