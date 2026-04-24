using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// リザルトのスコア表示クラス
/// </summary>
public class ResultScore : MonoBehaviour
{
    [SerializeField] Text scoreText;

    void Start()
    {
        scoreText.text = PlayerPrefs.GetInt("SCORE", 0).ToString();
        StartCoroutine(GetScore());
    }

    IEnumerator GetScore()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/NotesShooter/GetRankingData.php"))
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

            Debug.Log("スコア: " + www.downloadHandler.text);
            RankingLoad(www.downloadHandler.text);
        }
    }

    /// <summary>
    /// サーバーからランキングデータを取得する
    /// </summary>
    /// <param name="rankingText">rankingText にランキングデータを代入する</param>
    void RankingLoad(string rankingText)
    {
        // splitメソッドを使い、"," で区切られている値を rankingArr 配列に代入する
        // rankingArrの中身：["500","400","300","200","100"]
        string[] rankingTextArr = rankingText.Split(',');

        int[] rankingIntArr = new int[rankingTextArr.Length];

        // rankingIntArr 配列に int 変換したスコアを代入する (安全なパース)
        for (int i = 0; i < rankingTextArr.Length; i++)
        {
            // null 合体演算子
            // rankingTextArr[i] を取得。これが null かもしれない。
            // ?.Trim() は null 条件演算子：rankingTextArr[i] が非nullのときだけ Trim() を呼び、null なら結果は null。
            // ?? string.Empty は null 合体演算子：左辺が null の場合に代わりに空文字列 ("") を返す。
            // 結果：s には「前後の空白を取り除いた文字列」か、「null の場合は空文字列」が入る（NullReference を防ぐ）。
            // (例)
            // string temp = rankingTextArr[i];
            // string s;
            // if (temp != null) s = temp.Trim();
            // else s = string.Empty;
            string s = rankingTextArr[i]?.Trim() ?? string.Empty;

            if (int.TryParse(s, out int val))
            {
                rankingIntArr[i] = val;
            }
            else
            {
                // 数字以外の文字が混入している場合は除去して再試行
                string cleaned = Regex.Replace(s, "[^0-9\\-+]", string.Empty);
                if (int.TryParse(cleaned, out val))
                {
                    rankingIntArr[i] = val;
                    Debug.LogWarning($"ランキングデータをクリーン変換: '{s}' -> '{cleaned}' (index={i})");
                }
                else
                {
                    Debug.LogWarning($"ランキングデータのパース失敗: '{s}' を0に置換します。index={i}");
                    rankingIntArr[i] = 0;
                }
            }
        }

        // rankingIntArrを昇順ソート
        Array.Sort(rankingIntArr);
        // rankingIntArrの順序を反転させる (降順のランキングを作る場合は Array.Reverse(); は不要)
        Array.Reverse(rankingIntArr);

        // ランキングを出力する（今回は Debug.Log(); で仮出力）
        for (int i = 0; i < rankingTextArr.Length; i++)
        {
            Debug.Log($"{i + 1}位：{rankingIntArr[i]}");
        }
    }
}
