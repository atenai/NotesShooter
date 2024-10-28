using UnityEngine;

public class Goal : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    public static Goal singletonInstance = null;

    bool isGoal = false;
    public bool IsGoal => isGoal;

    void Awake()
    {
        //staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
        if (singletonInstance == null)
        {
            singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
        }
        else
        {
            Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            isGoal = true;

            //「SCORE」というキーで、Int値の「score.ScoreNum」を保存
            PlayerPrefs.SetInt("SCORE", Score.singletonInstance.ScoreNum);
            PlayerPrefs.Save();
        }
    }
}
