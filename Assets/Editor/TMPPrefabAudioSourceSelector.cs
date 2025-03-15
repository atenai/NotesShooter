using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

// TextMeshProコンポーネント（TextMeshProまたはTextMeshProUGUI）が付いているPrefabを検索・表示し、
// チェックを付けたPrefabだけAudioSourceコンポーネントを自動追加するエディター拡張
public class TMPPrefabAudioSourceSelector : EditorWindow
{
	// スクロールビューのスクロール位置を保持する変数
	private Vector2 scrollPos;

	// 検索結果として見つかったPrefabを格納するリスト
	private List<GameObject> tmpPrefabs = new List<GameObject>();

	// 各Prefabの選択状態を保持するための辞書
	private Dictionary<GameObject, bool> prefabSelection = new Dictionary<GameObject, bool>();

	// Unityエディタのメニューに「TMP付きPrefabを検索」を追加
	[MenuItem("Kashiwabara/TMP付きPrefabを検索しチェックをつけたものにAudioSourceコンポーネントを自動追加する")]
	private static void Init()
	{
		// ウィンドウを作成して表示する
		TMPPrefabAudioSourceSelector window = GetWindow<TMPPrefabAudioSourceSelector>();
		window.titleContent = new GUIContent("TMP付きPrefab検索");
		window.Show();
	}

	// エディターウィンドウの描画処理
	private void OnGUI()
	{
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

	// プロジェクト内のPrefabからTextMeshProまたはTextMeshProUGUIコンポーネントを含むものを検索する関数
	private void FindTMPPrefabs()
	{
		tmpPrefabs.Clear();
		prefabSelection.Clear();

		string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

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
}
