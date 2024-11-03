using UnityEngine;

public class GamePlayScore : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    public static GamePlayScore singletonInstance = null;

    int scoreNum = 0;
    public int ScoreNum => scoreNum;

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

    public void AddScore(int add_score)
    {
        scoreNum += add_score;//スコアを+する処理
    }
}
