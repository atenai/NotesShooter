using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// ゴールクラス
/// </summary>
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

    void Start()
    {
        //テスト
        //StartCoroutine(RegisterScore(3000));
    }

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            //「SCORE」というキーで、Int値の「score.ScoreNum」を保存
            PlayerPrefs.SetInt("SCORE", GamePlayScore.SingletonInstance.ScoreNum);
            PlayerPrefs.Save();

            StartCoroutine(RegisterScore(GamePlayScore.SingletonInstance.ScoreNum));
        }
    }

    /// <summary>
    /// スコアを登録する
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    IEnumerator RegisterScore(int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post($"http://localhost/NotesShooter/RegisterRankingData.php", form))
        {
            www.redirectLimit = 0;
            www.timeout = 10;

            yield return www.SendWebRequest();

            //Debug.Log("Request URL : " + url);
            //Debug.Log("Result      : " + www.result);
            //Debug.Log("ResponseCode: " + www.responseCode);
            //Debug.Log("Error       : " + www.error);

            //string location = www.GetResponseHeader("Location");
            //Debug.Log("Location    : " + location);

            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("リクエスト中");
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log("リクエスト成功");
                    break;

                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log(@"サーバとの通信に失敗。リクエストが接続できなかった、セキュリティで保護されたチャネルを確立できなかったなど。");
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(@"サーバがエラー応答を返した。サーバとの通信には成功したが、接続プロトコルで定義されているエラーを受け取った。");
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log(@"データの処理中にエラーが発生。リクエストはサーバとの通信に成功したが、受信したデータの処理中にエラーが発生。データが破損しているか、正しい形式ではないなど。");
                    break;
            }

            Debug.Log("受信: " + www.downloadHandler.text);

            // サーバーからの応答をトリムして、余分な空白を削除
            string resp = www.downloadHandler.text.Trim();
            bool registered = false;
            if (bool.TryParse(resp, out registered))
            {
                if (registered == true)
                {
                    Debug.Log("スコア登録成功");
                }
                else
                {
                    Debug.Log("スコア登録失敗");
                }
                isGoal = true;
            }
            else
            {
                Debug.LogWarning("予期せぬ応答: " + resp);
                isGoal = true;
            }
        }
    }
}
