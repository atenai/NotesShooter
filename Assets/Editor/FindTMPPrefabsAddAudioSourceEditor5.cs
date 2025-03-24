using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using System.IO;

// 特定フォルダー内のTextMeshProコンポーネント（TextMeshProまたはTextMeshProUGUI）が付いているPrefabを検索・表示し、
// チェックを付けたPrefabだけAudioSourceコンポーネントを自動追加するエディター拡張
// さらに検索結果をCSVファイルに書き出す
public class FindTMPPrefabsAddAudioSourceEditor5 : EditorWindow
{
	// スクロールビューのスクロール位置を保持する変数
	private Vector2 scrollPos;

	// 検索結果として見つかったPrefabを格納するリスト
	private List<GameObject> tmpPrefabs = new List<GameObject>();

	// 各Prefabの選択状態を保持するための辞書
	private Dictionary<GameObject, bool> prefabSelection = new Dictionary<GameObject, bool>();

	// 検索対象フォルダーのパスを指定（自由に変更可能）
	private string targetFolderPath = "Assets/";

	// CSVファイル出力先
	private string csvFilePath = "Assets/TMPPrefabsList.csv";

	// Unityエディタのメニューに「TMP付きPrefabを検索」を追加
	[MenuItem("Kashiwabara/特定フォルダー内のTextMeshProコンポーネントが付いているPrefabを検索し、チェックをつけたものにAudioSourceコンポーネントを自動追加する、さらに検索結果をCSVファイルに書き出す")]
	private static void Init()
	{
		// ウィンドウを作成して表示する
		FindTMPPrefabsAddAudioSourceEditor5 window = GetWindow<FindTMPPrefabsAddAudioSourceEditor5>();
		window.titleContent = new GUIContent("TMP付きPrefab検索");
		window.Show();
	}

	// エディターウィンドウの描画処理
	private void OnGUI()
	{
		// フォルダーのパスを入力できるようにする
		EditorGUILayout.LabelField("検索対象フォルダーのパス:");
		targetFolderPath = EditorGUILayout.TextField(targetFolderPath);

		// CSVファイルパスを入力
		EditorGUILayout.LabelField("CSVファイル出力先:");
		csvFilePath = EditorGUILayout.TextField(csvFilePath);

		// 検索を開始するためのボタン
		if (GUILayout.Button("TMP付きPrefabを検索"))
		{
			// ボタンが押されたらPrefab検索処理を実行
			FindTMPPrefabs();
		}

		// AudioSourceコンポーネントを追加するためのボタン
		if (GUILayout.Button("選択したPrefabにAudioSourceを追加"))
		{
			AddAudioSourceToSelectedPrefabs();
		}

		// CSVファイル出力用ボタン
		if (GUILayout.Button("Prefab名をCSVに書き出す"))
		{
			ExportPrefabsToCSV();
		}

		// 見つかったPrefabの数を表示する
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("見つかったPrefab数: " + tmpPrefabs.Count, EditorStyles.boldLabel);
		EditorGUILayout.Space();

		// 検索結果をスクロール可能なビューで表示
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		// 検索結果のPrefabをチェックボックス付きリストで表示
		foreach (GameObject prefab in tmpPrefabs)
		{
			EditorGUILayout.BeginHorizontal();

			// チェックボックスを表示
			prefabSelection[prefab] = EditorGUILayout.Toggle(prefabSelection[prefab], GUILayout.Width(20));

			// Prefab名を表示し、クリックするとプロジェクト内でそのPrefabをハイライトする
			if (GUILayout.Button(prefab.name, EditorStyles.objectField))
			{
				Selection.activeObject = prefab;
				EditorGUIUtility.PingObject(prefab);
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();
	}

	// 指定フォルダー内のPrefabからTextMeshProまたはTextMeshProUGUIコンポーネントを含むものを検索する関数
	private void FindTMPPrefabs()
	{
		tmpPrefabs.Clear();
		prefabSelection.Clear();

		string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolderPath });

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

			if (prefab != null)
			{
				if (prefab.GetComponentInChildren<TextMeshProUGUI>(true) != null ||
					prefab.GetComponentInChildren<TextMeshPro>(true) != null)
				{
					tmpPrefabs.Add(prefab);
					prefabSelection[prefab] = false;
				}
			}
		}

		Debug.Log($"TMP付きPrefabが {tmpPrefabs.Count} 個見つかりました。");
	}

	// チェックが付いたPrefab内のTMP付きオブジェクトにAudioSourceを追加する関数
	private void AddAudioSourceToSelectedPrefabs()
	{
		foreach (GameObject prefab in tmpPrefabs)
		{
			if (prefabSelection[prefab])
			{
				GameObject prefabInstance = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(prefab));

				TMP_Text[] tmpObjects = prefabInstance.GetComponentsInChildren<TMP_Text>(true);
				foreach (TMP_Text tmp in tmpObjects)
				{
					if (tmp.GetComponent<AudioSource>() == null)
					{
						tmp.gameObject.AddComponent<AudioSource>();
					}
				}

				PrefabUtility.SaveAsPrefabAsset(prefabInstance, AssetDatabase.GetAssetPath(prefab));
				PrefabUtility.UnloadPrefabContents(prefabInstance);
			}
		}

		Debug.Log("選択したPrefabへのAudioSourceコンポーネント追加が完了しました。");
	}

	// 検索結果のPrefab名をCSVファイルに書き出す関数
	private void ExportPrefabsToCSV()
	{
		using (StreamWriter sw = new StreamWriter(csvFilePath, false))
		{
			sw.WriteLine("Prefab Name");
			foreach (GameObject prefab in tmpPrefabs)
			{
				sw.WriteLine(prefab.name);
			}
		}
		AssetDatabase.Refresh();
		Debug.Log("Prefab名をCSVに書き出しました: " + csvFilePath);
	}
}