using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Text;

// 特定フォルダー内のTextMeshProコンポーネント（TextMeshProまたはTextMeshProUGUI）が付いているPrefabを検索・表示し、
// チェックを付けたPrefabだけAudioSourceコンポーネントを自動追加するエディター拡張
// さらに検索結果をPrefab名（縦）とTMPオブジェクト名（横）でCSVファイルに書き出す
public class FindTMPPrefabsAddAudioSourceEditor6 : EditorWindow
{
	private Vector2 scrollPos;
	private List<GameObject> tmpPrefabs = new List<GameObject>();
	private Dictionary<GameObject, bool> prefabSelection = new Dictionary<GameObject, bool>();
	private string targetFolderPath = "Assets/";
	private string csvFilePath = "Assets/TMPPrefabsList.csv";
	private Dictionary<GameObject, List<string>> prefabTMPNames = new Dictionary<GameObject, List<string>>();
	private int maxTMPCount = 0;

	[MenuItem("Kashiwabara/特定フォルダー内のTextMeshProコンポーネントが付いているPrefabを検索し、チェックをつけたものにAudioSourceコンポーネントを自動追加する、さらに検索結果をPrefab名（縦）とTMPオブジェクト名（横）でCSVファイルに書き出す")]
	private static void Init()
	{
		FindTMPPrefabsAddAudioSourceEditor6 window = GetWindow<FindTMPPrefabsAddAudioSourceEditor6>();
		window.titleContent = new GUIContent("TMP付きPrefab検索");
		window.Show();
	}

	private void OnGUI()
	{
		EditorGUILayout.LabelField("検索対象フォルダーのパス:");
		targetFolderPath = EditorGUILayout.TextField(targetFolderPath);

		EditorGUILayout.LabelField("CSVファイル出力先:");
		csvFilePath = EditorGUILayout.TextField(csvFilePath);

		if (GUILayout.Button("TMP付きPrefabを検索"))
		{
			FindTMPPrefabs();
		}

		if (GUILayout.Button("選択したPrefabにAudioSourceを追加"))
		{
			AddAudioSourceToSelectedPrefabs();
		}

		if (GUILayout.Button("PrefabとTMP名をCSVに書き出す"))
		{
			ExportPrefabsAndTMPObjectsToCSV();
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("見つかったPrefab数: " + tmpPrefabs.Count, EditorStyles.boldLabel);
		EditorGUILayout.Space();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		foreach (GameObject prefab in tmpPrefabs)
		{
			EditorGUILayout.BeginHorizontal();
			prefabSelection[prefab] = EditorGUILayout.Toggle(prefabSelection[prefab], GUILayout.Width(20));

			if (GUILayout.Button(prefab.name, EditorStyles.objectField))
			{
				Selection.activeObject = prefab;
				EditorGUIUtility.PingObject(prefab);
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();
	}

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

	// 検索時にPrefab内のTMPオブジェクト名を事前に取得する
	private void FindTMPPrefabs()
	{
		tmpPrefabs.Clear();
		prefabSelection.Clear();
		prefabTMPNames.Clear();
		maxTMPCount = 0;

		string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolderPath });

		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

			if (prefab != null && (prefab.GetComponentInChildren<TextMeshProUGUI>(true) != null || prefab.GetComponentInChildren<TextMeshPro>(true) != null))
			{
				tmpPrefabs.Add(prefab);
				prefabSelection.Add(prefab, false);

				GameObject instance = PrefabUtility.LoadPrefabContents(path);
				TMP_Text[] tmpObjects = instance.GetComponentsInChildren<TMP_Text>(true);
				List<string> tmpNames = new List<string>();
				foreach (TMP_Text tmp in tmpObjects)
				{
					tmpNames.Add(tmp.gameObject.name);
				}
				prefabTMPNames.Add(prefab, tmpNames);

				if (maxTMPCount < tmpNames.Count)
				{
					maxTMPCount = tmpNames.Count;
				}

				PrefabUtility.UnloadPrefabContents(instance);
			}
		}

		Debug.Log($"TMP付きPrefabが {tmpPrefabs.Count} 個見つかりました。");
	}

	private void ExportPrefabsAndTMPObjectsToCSV()
	{
		using (StreamWriter sw = new StreamWriter(csvFilePath, false, Encoding.UTF8))
		{
			sw.Write("プレハブ名");

			for (int i = 1; i <= maxTMPCount; i++)
			{
				sw.Write($",テキストメッシュプロが付いたゲームオブジェクト : {i}");
			}
			sw.WriteLine();

			foreach (var pair in prefabTMPNames)
			{
				sw.Write(pair.Key.name);
				foreach (string tmpName in pair.Value)
				{
					sw.Write($",{tmpName}");
				}

				// TMPが最大数より少ない場合、空のセルを追加
				for (int i = pair.Value.Count; i < maxTMPCount; i++)
				{
					sw.Write(",");
				}

				sw.WriteLine();
			}
		}

		AssetDatabase.Refresh();
		Debug.Log("PrefabとTMPオブジェクト名をCSVに書き出しました: " + csvFilePath);
	}
}