using UnityEngine;

/// <summary>
/// ステージごとのスコアをPlayerPrefsに記録するクラス。
/// キーは「SCORE_ステージ名」「HIGHSCORE_ステージ名」のようにステージ名を付けて分けている。
/// </summary>
public static class ScoreRecord
{
    [Tooltip("直前のプレイのスコア。以前から使っているキーなのでそのまま残している")]
    const string scoreKey = "SCORE";
    [Tooltip("ステージ別の最新スコア。実際のキーは「SCORE_ステージ名」になる")]
    const string stageScoreKeyPrefix = "SCORE_";
    [Tooltip("ステージ別のハイスコア。実際のキーは「HIGHSCORE_ステージ名」になる")]
    const string stageHighScoreKeyPrefix = "HIGHSCORE_";
    [Tooltip("直前に遊んだステージ名。リザルト画面がどのステージの結果かを知る為に使う")]
    const string lastStageNameKey = "LAST_STAGE_NAME";
    [Tooltip("直前のプレイでハイスコアを更新したか")]
    const string lastPlayIsNewRecordKey = "LAST_PLAY_IS_NEW_RECORD";

    /// <summary>
    /// 直前に遊んだステージ名
    /// </summary>
    public static string LastStageName => PlayerPrefs.GetString(lastStageNameKey, string.Empty);

    /// <summary>
    /// 直前のプレイのスコア
    /// </summary>
    public static int LastScore => PlayerPrefs.GetInt(scoreKey, 0);

    /// <summary>
    /// 直前のプレイでハイスコアを更新したか
    /// </summary>
    public static bool LastPlayIsNewRecord => PlayerPrefs.GetInt(lastPlayIsNewRecordKey, 0) != 0;

    /// <summary>
    /// 指定したステージのハイスコアを取得する
    /// </summary>
    public static int GetHighScore(string stageName)
    {
        if (string.IsNullOrEmpty(stageName) == true)
        {
            return 0;
        }

        return PlayerPrefs.GetInt(stageHighScoreKeyPrefix + stageName, 0);
    }

    /// <summary>
    /// 指定したステージの最新スコアを取得する
    /// </summary>
    public static int GetScore(string stageName)
    {
        if (string.IsNullOrEmpty(stageName) == true)
        {
            return 0;
        }

        return PlayerPrefs.GetInt(stageScoreKeyPrefix + stageName, 0);
    }

    /// <summary>
    /// ステージのスコアを記録する。ハイスコアを更新した場合はtrueを返す
    /// </summary>
    public static bool Save(string stageName, int score)
    {
        if (string.IsNullOrEmpty(stageName) == true)
        {
            Debug.LogWarning("ステージ名が空なのでスコアを記録しませんでした");
            return false;
        }

        //ハイスコアの判定は上書きする前に済ませておく
        bool isNewRecord = GetHighScore(stageName) < score;

        PlayerPrefs.SetInt(stageScoreKeyPrefix + stageName, score);
        PlayerPrefs.SetString(lastStageNameKey, stageName);
        //リザルト画面が読む「直前のスコア」
        PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.SetInt(lastPlayIsNewRecordKey, isNewRecord == true ? 1 : 0);

        if (isNewRecord == true)
        {
            PlayerPrefs.SetInt(stageHighScoreKeyPrefix + stageName, score);
        }

        PlayerPrefs.Save();

        return isNewRecord;
    }
}
