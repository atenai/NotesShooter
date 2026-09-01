using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// リザルトのスコア表示クラス。
/// 数字をいきなり出さず、0から数え上げてゲージを伸ばし、最後にランクと新記録を出す。
/// </summary>
public class ResultScore : MonoBehaviour
{
    /// <summary>
    /// 何点以上でどのランクにするか
    /// </summary>
    [Serializable]
    public class RankThreshold
    {
        [Tooltip("表示する文字")]
        public string rankName = "C";
        [Tooltip("このランクになる、自己ベストに対する最低の割合(%)")]
        public int leastRatePercent = 0;
        [Tooltip("文字の色")]
        public Color rankColor = Color.white;
    }

    [Tooltip("スコアの数字")]
    [SerializeField] Text textScore;
    [Tooltip("遊んだステージ名")]
    [SerializeField] Text textStageName;
    [Tooltip("そのステージのハイスコア")]
    [SerializeField] Text textHighScore;
    [Tooltip("倒した的の数")]
    [SerializeField] Text textTargetCount;
    [Tooltip("ランクの文字")]
    [SerializeField] Text textRank;
    [Tooltip("ハイスコアとの差")]
    [SerializeField] Text textDiff;
    [Tooltip("ハイスコアに対する今回のスコアの割合を示すゲージ")]
    [SerializeField] Image imageScoreGauge;
    [Tooltip("ハイスコア更新時に出す表示")]
    [SerializeField] GameObject newRecordGameObject;
    [Tooltip("ハイスコア更新時の文字。どれだけ伸びたかを出す")]
    [SerializeField] Text textNewRecord;
    [Tooltip("ランクの判定表。割合の高い順に並べる")]
    [SerializeField] RankThreshold[] rankThresholds;

    [Tooltip("スコアを数え上げるのにかける秒数")]
    [SerializeField] float countUpTime = 0.9f;
    [Tooltip("数え終わってからランクと新記録を出すまでの間")]
    [SerializeField] float revealDelay = 0.15f;
    [Tooltip("広告が閉じるのを待つ上限の秒数")]
    [SerializeField] float adsWaitLimit = 30.0f;

    [Tooltip("ランクが出る時に一瞬大きく見せる倍率")]
    const float rankPopScale = 1.6f;
    [Tooltip("ランクの大きさが戻る速さ")]
    const float rankPopSpeed = 6.0f;

    void Start()
    {
        StartCoroutine(DisplayResult());
        StartCoroutine(GetScore());
    }

    /// <summary>
    /// 直前のプレイの結果を順番に出していく
    /// </summary>
    IEnumerator DisplayResult()
    {
        string stageName = ScoreRecord.LastStageName;
        int score = ScoreRecord.LastScore;
        int highScore = ScoreRecord.GetHighScore(stageName);
        bool isNewRecord = ScoreRecord.LastPlayIsNewRecord;

        //更新するとハイスコアは今回の点で上書きされるので、比べる相手は更新前の点を使う
        int previousHighScore = ScoreRecord.LastPreviousHighScore;

        if (textStageName != null)
        {
            textStageName.text = ScoreRecord.GetStageDisplayName(stageName);
        }

        if (textHighScore != null)
        {
            //更新した時、ハイスコアは今回の点で上書き済み。
            //同じ数字が2つ並ぶだけになるので、ここには更新前の記録を出す
            textHighScore.text = previousHighScore <= 0 ? "-" : previousHighScore.ToString();
        }

        if (textTargetCount != null)
        {
            textTargetCount.text = ScoreRecord.LastTargetCount.ToString();
        }

        if (textDiff != null)
        {
            textDiff.text = string.Empty;
        }

        //数え上げが終わるまでは伏せておく
        if (newRecordGameObject != null)
        {
            newRecordGameObject.SetActive(false);
        }

        if (textRank != null)
        {
            textRank.text = string.Empty;
        }

        //広告が画面を覆っている間に演出を流しても見えないので、閉じるまで待つ。
        //閉じた通知が来ない環境もあるので、待つのは上限までにして必ず先へ進める
        float waited = 0.0f;
        while (AdsInterstitial.IsShowing == true && waited < adsWaitLimit)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return StartCoroutine(CountUpScore(score, previousHighScore));

        yield return new WaitForSeconds(revealDelay);

        DisplayRank(score, previousHighScore);
        DisplayDiff(score, previousHighScore, isNewRecord);

        if (newRecordGameObject != null)
        {
            newRecordGameObject.SetActive(isNewRecord);
        }

        if (textRank != null)
        {
            yield return StartCoroutine(PopRank());
        }
    }

    /// <summary>
    /// 0から今回のスコアまで数え上げ、あわせてゲージも伸ばす
    /// </summary>
    IEnumerator CountUpScore(int score, int highScore)
    {
        if (textScore == null)
        {
            yield break;
        }

        float elapsed = 0.0f;

        while (elapsed < countUpTime)
        {
            elapsed += Time.deltaTime;

            //最初は速く、最後にゆっくり止まるようにする
            float rate = Mathf.Clamp01(elapsed / countUpTime);
            rate = 1.0f - (1.0f - rate) * (1.0f - rate);

            int current = Mathf.RoundToInt(score * rate);
            textScore.text = current.ToString();
            ApplyGauge(current, highScore);

            yield return null;
        }

        textScore.text = score.ToString();
        ApplyGauge(score, highScore);
    }

    /// <summary>
    /// 自己ベストに対する割合でゲージを伸ばす。
    /// Imageのfillは角丸スプライトが要り、細い棒だと縁が破綻するので、
    /// 板そのものの右端を動かして伸ばしている
    /// </summary>
    void ApplyGauge(int score, int highScore)
    {
        if (imageScoreGauge == null)
        {
            return;
        }

        float rate;
        if (highScore <= 0)
        {
            //まだ記録が無い時は、点が入っていれば満タンにする
            rate = score <= 0 ? 0.0f : 1.0f;
        }
        else
        {
            rate = Mathf.Clamp01((float)score / highScore);
        }

        RectTransform rectTransform = imageScoreGauge.rectTransform;
        rectTransform.anchorMax = new Vector2(rate, rectTransform.anchorMax.y);
        rectTransform.offsetMin = new Vector2(0.0f, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(0.0f, rectTransform.offsetMax.y);
    }

    /// <summary>
    /// ランクを出す。
    /// ステージごとに取れる点が大きく違うので、決め打ちの点数で区切ると
    /// あるステージは常に高評価、別のステージは常に低評価になってしまう。
    /// 自分の best に対してどこまで届いたかで決めれば、どのステージでも意味を持つ
    /// </summary>
    void DisplayRank(int score, int previousHighScore)
    {
        if (textRank == null || rankThresholds == null || rankThresholds.Length == 0)
        {
            return;
        }

        //まだ記録が無い初回は、それ自体が自己ベストなので一番上にする
        int ratePercent = previousHighScore <= 0 ? 100 : Mathf.RoundToInt(100.0f * score / previousHighScore);

        foreach (RankThreshold threshold in rankThresholds)
        {
            if (threshold == null || ratePercent < threshold.leastRatePercent)
            {
                continue;
            }

            textRank.text = threshold.rankName;
            textRank.color = threshold.rankColor;
            return;
        }

        //どれにも当てはまらなければ一番下のランクにする
        RankThreshold lowest = rankThresholds[rankThresholds.Length - 1];
        textRank.text = lowest.rankName;
        textRank.color = lowest.rankColor;
    }

    /// <summary>
    /// ハイスコアとの差を出す
    /// </summary>
    void DisplayDiff(int score, int previousHighScore, bool isNewRecord)
    {
        if (isNewRecord == true)
        {
            //更新した時は差を新記録の側に出すので、こちらは空にしておく
            if (textDiff != null)
            {
                textDiff.text = string.Empty;
            }

            if (textNewRecord != null)
            {
                int growth = score - previousHighScore;
                //初めて遊んだステージは比べる相手が無いので、伸び幅は出さない
                textNewRecord.text = previousHighScore <= 0
                    ? "NEW RECORD"
                    : "NEW RECORD   +" + growth.ToString();
            }

            return;
        }

        if (textDiff == null)
        {
            return;
        }

        int diff = previousHighScore - score;
        if (diff <= 0)
        {
            textDiff.text = "自己ベストに ならびました";
            return;
        }

        textDiff.text = "ベストまで あと " + diff.ToString();
    }

    /// <summary>
    /// ランクの文字を一瞬大きく出してから元の大きさへ戻す
    /// </summary>
    IEnumerator PopRank()
    {
        RectTransform rectTransform = textRank.rectTransform;
        Vector3 baseScale = Vector3.one;
        float scale = rankPopScale;

        while (0.001f < scale - 1.0f)
        {
            scale = Mathf.Lerp(scale, 1.0f, 1.0f - Mathf.Exp(-rankPopSpeed * Time.deltaTime));
            rectTransform.localScale = baseScale * scale;
            yield return null;
        }

        rectTransform.localScale = baseScale;
    }

    IEnumerator GetScore()
    {
        //ローカルサーバー
        const string localServer = "http://localhost/NotesShooter/GetRankingData.php";
        //AWSサーバー
        //パブリック IPv4 アドレス
        string awsServerPublicIPv4Address = "35.78.65.237";
        string awsServer = "http://" + awsServerPublicIPv4Address + "/GetRankingData.php";
        using (UnityWebRequest www = UnityWebRequest.Get(awsServer))
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
