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
    [Tooltip("直前のプレイで倒した的の数")]
    const string lastTargetCountKey = "LAST_TARGET_COUNT";
    [Tooltip("今遊んでいるステージの画面に出す名前。シーン名だけでは何ステージ目か分からないので別に覚えておく")]
    const string playingStageDisplayNameKey = "PLAYING_STAGE_DISPLAY_NAME";
    [Tooltip("直前のプレイを始める前のハイスコア。更新するとハイスコアは上書きされてしまうので、比べる相手を残しておく")]
    const string lastPreviousHighScoreKey = "LAST_PREVIOUS_HIGHSCORE";

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
    /// 直前のプレイで倒した的の数
    /// </summary>
    public static int LastTargetCount => PlayerPrefs.GetInt(lastTargetCountKey, 0);

    /// <summary>
    /// 直前のプレイを始める前のハイスコア。
    /// ハイスコアを更新した時、GetHighScoreは今回の点を返すようになるので、
    /// 「どれだけ伸びたか」を出すにはこちらを見る
    /// </summary>
    public static int LastPreviousHighScore => PlayerPrefs.GetInt(lastPreviousHighScoreKey, 0);

    /// <summary>
    /// 画面に出すステージ名。ステージセレクトから始めた時に覚える。
    /// シーンを直接再生した時など、覚えていなければシーン名から作る
    /// </summary>
    public static string GetStageDisplayName(string stageName)
    {
        string displayName = PlayerPrefs.GetString(playingStageDisplayNameKey, string.Empty);
        if (string.IsNullOrEmpty(displayName) == false)
        {
            return displayName;
        }

        return string.IsNullOrEmpty(stageName) == true ? "ステージ" : stageName;
    }

    /// <summary>
    /// これから遊ぶステージの、画面に出す名前を覚えておく
    /// </summary>
    public static void SetPlayingStageDisplayName(string displayName)
    {
        PlayerPrefs.SetString(playingStageDisplayNameKey, displayName == null ? string.Empty : displayName);
        PlayerPrefs.Save();
    }

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
    /// <param name="targetCount">そのプレイで倒した的の数。リザルトの表示だけに使う</param>
    public static bool Save(string stageName, int score, int targetCount = 0)
    {
        if (string.IsNullOrEmpty(stageName) == true)
        {
            Debug.LogWarning("ステージ名が空なのでスコアを記録しませんでした");
            return false;
        }

        //ハイスコアの判定と、比べる相手の保存は上書きする前に済ませておく
        int previousHighScore = GetHighScore(stageName);
        bool isNewRecord = previousHighScore < score;

        PlayerPrefs.SetInt(stageScoreKeyPrefix + stageName, score);
        PlayerPrefs.SetString(lastStageNameKey, stageName);
        //リザルト画面が読む「直前のスコア」
        PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.SetInt(lastPlayIsNewRecordKey, isNewRecord == true ? 1 : 0);
        PlayerPrefs.SetInt(lastTargetCountKey, targetCount);
        PlayerPrefs.SetInt(lastPreviousHighScoreKey, previousHighScore);

        if (isNewRecord == true)
        {
            PlayerPrefs.SetInt(stageHighScoreKeyPrefix + stageName, score);
        }

        PlayerPrefs.Save();

        return isNewRecord;
    }
}
