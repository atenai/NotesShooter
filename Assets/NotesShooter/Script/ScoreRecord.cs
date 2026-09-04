using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージごとのスコアと進行状況をEasy Save 3で記録するクラス。
/// キーは「SCORE_ステージ名」「HIGHSCORE_ステージ名」のようにステージ名を付けて分けている。
///
/// 以前はPlayerPrefsに保存していた。既に遊んでいる人の記録が消えないよう、
/// 最初に読み書きする時に一度だけPlayerPrefsから引き継ぐ。
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
    [Tooltip("何ステージ目まで進んだか")]
    const string playCountKey = "PLAY_COUNT";
    [Tooltip("解除の演出を見せたステージ番号")]
    const string directedStageNumberKey = "DIRECTED_STAGE_NUMBER";

    [Tooltip("PlayerPrefsからの引き継ぎが済んだかの目印")]
    const string migratedKey = "MIGRATED_FROM_PLAYERPREFS";

    [Tooltip("引き継ぎの確認を、アプリを動かしている間に一度だけにする為の覚え")]
    static bool isMigrationChecked = false;

    /// <summary>
    /// 直前に遊んだステージ名
    /// </summary>
    public static string LastStageName
    {
        get
        {
            EnsureMigrated();
            return LoadString(lastStageNameKey, string.Empty);
        }
    }

    /// <summary>
    /// 直前のプレイのスコア
    /// </summary>
    public static int LastScore
    {
        get
        {
            EnsureMigrated();
            return ES3.Load<int>(scoreKey, 0);
        }
    }

    /// <summary>
    /// 直前のプレイでハイスコアを更新したか
    /// </summary>
    public static bool LastPlayIsNewRecord
    {
        get
        {
            EnsureMigrated();
            return ES3.Load<int>(lastPlayIsNewRecordKey, 0) != 0;
        }
    }

    /// <summary>
    /// 直前のプレイで倒した的の数
    /// </summary>
    public static int LastTargetCount
    {
        get
        {
            EnsureMigrated();
            return ES3.Load<int>(lastTargetCountKey, 0);
        }
    }

    /// <summary>
    /// 直前のプレイを始める前のハイスコア。
    /// ハイスコアを更新した時、GetHighScoreは今回の点を返すようになるので、
    /// 「どれだけ伸びたか」を出すにはこちらを見る
    /// </summary>
    public static int LastPreviousHighScore
    {
        get
        {
            EnsureMigrated();
            return ES3.Load<int>(lastPreviousHighScoreKey, 0);
        }
    }

    /// <summary>
    /// 何ステージ目まで進んだか。まだ一度も遊んでいなければ1
    /// </summary>
    public static int PlayCount
    {
        get
        {
            EnsureMigrated();
            return ES3.Load<int>(playCountKey, 1);
        }
    }

    /// <summary>
    /// 進んだステージ数を記録する
    /// </summary>
    public static void SavePlayCount(int playCount)
    {
        EnsureMigrated();
        ES3.Save<int>(playCountKey, playCount);
    }

    /// <summary>
    /// 解除の演出を見せたステージ番号。まだ一度も見せていなければ0。
    /// PlayerPrefsの頃には無かったキーなので引き継ぎは要らない
    /// </summary>
    public static int DirectedStageNumber
    {
        get
        {
            EnsureMigrated();
            return ES3.Load<int>(directedStageNumberKey, 0);
        }
    }

    /// <summary>
    /// 解除の演出を見せたステージ番号を記録する。
    /// 番号が戻ると一度見た演出をまた見せてしまうので、大きい方だけを残す
    /// </summary>
    public static void SaveDirectedStageNumber(int stageNumber)
    {
        EnsureMigrated();

        if (stageNumber <= DirectedStageNumber)
        {
            return;
        }

        ES3.Save<int>(directedStageNumberKey, stageNumber);
    }

    /// <summary>
    /// 画面に出すステージ名。ステージセレクトから始めた時に覚える。
    /// シーンを直接再生した時など、覚えていなければシーン名から作る
    /// </summary>
    public static string GetStageDisplayName(string stageName)
    {
        EnsureMigrated();

        string displayName = LoadString(playingStageDisplayNameKey, string.Empty);
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
        EnsureMigrated();
        ES3.Save<string>(playingStageDisplayNameKey, displayName == null ? string.Empty : displayName);
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

        EnsureMigrated();
        return ES3.Load<int>(stageHighScoreKeyPrefix + stageName, 0);
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

        EnsureMigrated();
        return ES3.Load<int>(stageScoreKeyPrefix + stageName, 0);
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

        EnsureMigrated();

        //ハイスコアの判定と、比べる相手の保存は上書きする前に済ませておく
        int previousHighScore = GetHighScore(stageName);
        bool isNewRecord = previousHighScore < score;

        //1つずつ保存するとその都度ファイルへ書きに行くので、まとめて一度で書く
        using (ES3Writer writer = ES3Writer.Create(new ES3Settings()))
        {
            if (writer == null)
            {
                Debug.LogError("セーブファイルを開けなかったのでスコアを記録できませんでした");
                return false;
            }

            writer.Write<int>(stageScoreKeyPrefix + stageName, score);
            writer.Write<string>(lastStageNameKey, stageName);
            //リザルト画面が読む「直前のスコア」
            writer.Write<int>(scoreKey, score);
            writer.Write<int>(lastPlayIsNewRecordKey, isNewRecord == true ? 1 : 0);
            writer.Write<int>(lastTargetCountKey, targetCount);
            writer.Write<int>(lastPreviousHighScoreKey, previousHighScore);

            if (isNewRecord == true)
            {
                writer.Write<int>(stageHighScoreKeyPrefix + stageName, score);
            }

            //書いていないキーは消えずに残る
            writer.Save();
        }

        return isNewRecord;
    }

    /// <summary>
    /// 記録を全て消す。デバッグ用。
    ///
    /// Easy Save 3のファイルを消すだけでは足りない。引き継ぎ済みの目印も
    /// 一緒に消えてしまい、次に起動した時にPlayerPrefsからの引き継ぎが
    /// 走って、消したはずの記録が戻ってくる。
    /// 古い方も消した上で、目印だけ書き直しておく
    /// </summary>
    public static void DeleteAll()
    {
        ES3.DeleteFile();

        DeletePlayerPrefs(scoreKey);
        DeletePlayerPrefs(lastPlayIsNewRecordKey);
        DeletePlayerPrefs(lastTargetCountKey);
        DeletePlayerPrefs(lastPreviousHighScoreKey);
        DeletePlayerPrefs(playCountKey);
        DeletePlayerPrefs(lastStageNameKey);
        DeletePlayerPrefs(playingStageDisplayNameKey);

        //ステージ別の記録は、ビルドに含まれているシーン名から総当たりで消す
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            DeletePlayerPrefs(stageScoreKeyPrefix + sceneName);
            DeletePlayerPrefs(stageHighScoreKeyPrefix + sceneName);
        }

        PlayerPrefs.Save();

        //消した直後に引き継ぎが走らないようにする
        isMigrationChecked = true;
        ES3.Save<bool>(migratedKey, true);
    }

    static void DeletePlayerPrefs(string key)
    {
        if (PlayerPrefs.HasKey(key) == false)
        {
            return;
        }

        PlayerPrefs.DeleteKey(key);
    }

    /// <summary>
    /// PlayerPrefsに残っている記録をEasy Save 3へ引き継ぐ。
    /// アプリを動かしている間に一度だけ実行する。
    /// PlayerPrefs側は消さずに残しておく。引き継ぎに何かあっても元に戻せるようにする為
    /// </summary>
    static void EnsureMigrated()
    {
        if (isMigrationChecked == true)
        {
            return;
        }
        isMigrationChecked = true;

        if (ES3.KeyExists(migratedKey) == true)
        {
            return;
        }

        MigrateInt(scoreKey);
        MigrateInt(lastPlayIsNewRecordKey);
        MigrateInt(lastTargetCountKey);
        MigrateInt(lastPreviousHighScoreKey);
        MigrateInt(playCountKey);
        MigrateString(lastStageNameKey);
        MigrateString(playingStageDisplayNameKey);

        //ステージ別の記録は、ビルドに含まれているシーン名から総当たりで拾う
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            MigrateInt(stageScoreKeyPrefix + sceneName);
            MigrateInt(stageHighScoreKeyPrefix + sceneName);
        }

        ES3.Save<bool>(migratedKey, true);
    }

    /// <summary>
    /// 文字列を読む。
    /// ES3.Load&lt;string&gt;(key, 既定値) と書くと、第2引数が文字列なので
    /// 「ファイルパス指定のオーバーロード」の方に解決されてしまい、
    /// 存在しないファイルを開こうとして例外になる。
    /// 曖昧さの無い呼び方に分けておく
    /// </summary>
    static string LoadString(string key, string defaultValue)
    {
        if (ES3.KeyExists(key) == false)
        {
            return defaultValue;
        }

        return ES3.Load<string>(key);
    }

    static void MigrateInt(string key)
    {
        if (PlayerPrefs.HasKey(key) == false)
        {
            return;
        }

        ES3.Save<int>(key, PlayerPrefs.GetInt(key));
    }

    static void MigrateString(string key)
    {
        if (PlayerPrefs.HasKey(key) == false)
        {
            return;
        }

        ES3.Save<string>(key, PlayerPrefs.GetString(key));
    }
}
