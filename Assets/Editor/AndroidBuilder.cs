using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// バッチモードからAndroid向けAPKをビルドする。
/// エディターを起動したままの実行だとビルドが完了する前に打ち切られてしまうため、
/// -batchmode で Unity を起動して呼び出す。
///
/// 実行例:
///   Unity.exe -quit -batchmode -nographics -projectPath &lt;プロジェクト&gt;
///             -executeMethod AndroidBuilder.BuildAndroid
///             -buildOutput &lt;出力APKパス&gt; -logFile &lt;ログパス&gt;
/// </summary>
public static class AndroidBuilder
{
	/// <summary>ログから結果を拾うための固定マーカー</summary>
	const string Start_Marker = "NS_BUILD_START";
	const string Done_Marker = "NS_BUILD_DONE";

	public static void BuildAndroid()
	{
		string outputPath = GetCommandLineArg("-buildOutput");
		if (string.IsNullOrEmpty(outputPath) == true)
		{
			outputPath = GetDefaultOutputPath();
		}

		string outputDirectory = Path.GetDirectoryName(outputPath);
		if (string.IsNullOrEmpty(outputDirectory) == false && Directory.Exists(outputDirectory) == false)
		{
			Directory.CreateDirectory(outputDirectory);
		}

		List<string> scenes = new List<string>();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			if (scene.enabled == true)
			{
				scenes.Add(scene.path);
			}
		}

		Debug.Log(Start_Marker + " scenes=" + scenes.Count + " out=" + outputPath
			+ " cleanBuild=" + HasCommandLineFlag("-cleanBuild"));

		if (scenes.Count == 0)
		{
			Debug.LogError(Done_Marker + " result=NoScenes");
			EditorApplication.Exit(1);
			return;
		}

		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = scenes.ToArray();
		options.locationPathName = outputPath;
		options.target = BuildTarget.Android;
		options.targetGroup = BuildTargetGroup.Android;
		options.options = BuildOptions.None;

		// 差分ビルドのキャッシュが古い状態を再利用して、削除済みスクリプトの参照が残ったままの
		// シーンデータが出来てしまうことがある（実機で「level? is corrupted」となり起動時に落ちる）。
		// -cleanBuild を付けた場合はキャッシュを捨ててビルドし直す。
		bool isCleanBuild = HasCommandLineFlag("-cleanBuild");
		if (isCleanBuild == true)
		{
			options.options = options.options | BuildOptions.CleanBuildCache;
		}

		// EditorUserBuildSettings.buildAppBundle が有効なままだと、出力先の拡張子が .apk でも
		// 中身はAAB(Android App Bundle)になり、端末に直接インストールできない成果物になってしまう。
		// このメソッドはAPKを出す前提なので明示的に無効化し、エディターの設定はビルド後に戻す。
		bool previousBuildAppBundle = EditorUserBuildSettings.buildAppBundle;
		EditorUserBuildSettings.buildAppBundle = false;

		BuildReport report;
		try
		{
			report = BuildPipeline.BuildPlayer(options);
		}
		finally
		{
			EditorUserBuildSettings.buildAppBundle = previousBuildAppBundle;
		}

		BuildSummary summary = report.summary;

		Debug.Log(Done_Marker
			+ " result=" + summary.result
			+ " errors=" + summary.totalErrors
			+ " warnings=" + summary.totalWarnings
			+ " sizeBytes=" + summary.totalSize
			+ " duration=" + summary.totalTime
			+ " path=" + summary.outputPath);

		if (summary.result != BuildResult.Succeeded)
		{
			EditorApplication.Exit(1);
			return;
		}

		EditorApplication.Exit(0);
	}

	/// <summary>
	/// 既定の出力先。リポジトリを汚さないよう、プロジェクトの兄弟フォルダに出す
	/// （Tools/deploygate-upload.ps1 が既定で見るフォルダと同じ）
	/// </summary>
	static string GetDefaultOutputPath()
	{
		string projectRoot = Directory.GetParent(Application.dataPath).FullName;
		string siblingDirectory = Path.Combine(Directory.GetParent(projectRoot).FullName, "NotesShooter_Build");
		return Path.Combine(siblingDirectory, "NotesShooterBuild.apk");
	}

	/// <summary>
	/// 値を伴わないコマンドライン引数（フラグ）が指定されているか
	/// </summary>
	static bool HasCommandLineFlag(string name)
	{
		string[] args = System.Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] == name)
			{
				return true;
			}
		}
		return false;
	}

	static string GetCommandLineArg(string name)
	{
		string[] args = System.Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length - 1; i++)
		{
			if (args[i] == name)
			{
				return args[i + 1];
			}
		}
		return null;
	}
}
