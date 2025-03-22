using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// TextMeshProコンポーネント（TextMeshProまたはTextMeshProUGUI）が付いているPrefabを検索・表示し、
/// それらのオブジェクトにAudioSourceコンポーネントを自動追加するエディター拡張
/// </summary>
public class FindTMPPrefabsAddAudioSourceEditor2 : EditorWindow
{
	// スクロールビューのスクロール位置を保持する変数
	private Vector2 scrollPos;

	// 検索結果として見つかったPrefabを格納するリスト
	private List<GameObject> tmpPrefabs = new List<GameObject>();

	/// <summary>
	/// Unityエディタのメニューに「TMP付きPrefabを検索」を追加
	/// </summary>
	[MenuItem("Kashiwabara/TextMeshProコンポーネントが付いているPrefabを検索し、AudioSourceコンポーネントを追加する")]
	private static void Init()
	{
		// ウィンドウを作成して表示する
		FindTMPPrefabsAddAudioSourceEditor2 window = GetWindow<FindTMPPrefabsAddAudioSourceEditor2>();
		window.titleContent = new GUIContent("TextMeshProコンポーネントが付いているPrefabを検索し、AudioSourceコンポーネントを追加する");
		window.Show();
	}

	/// <summary>
	/// エディターウィンドウの描画処理
	/// </summary>
	private void OnGUI()
	{
		// 検索を開始するためのボタン
		if (GUILayout.Button("TMP付きPrefabを検索"))
		{
			// ボタンが押されたらPrefab検索処理を実行
			FindTMPPrefabs();
		}

		// AudioSourceコンポーネントを追加するためのボタン
		if (GUILayout.Button("AudioSourceを自動追加"))
		{
			AddAudioSourceToTMPPrefabs();
		}

		// 見つかったPrefabの数を表示する
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("見つかったPrefab数: " + tmpPrefabs.Count, EditorStyles.boldLabel);
		EditorGUILayout.Space();

		// 検索結果をスクロール可能なビューで表示
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		// 検索結果のPrefabをリスト表示
		foreach (GameObject prefab in tmpPrefabs)
		{
			// Prefab名を表示し、クリックするとプロジェクト内でそのPrefabをハイライトする
			if (GUILayout.Button(prefab.name, EditorStyles.objectField))
			{
				// 選択したPrefabをUnityエディタ内で選択状態に設定
				Selection.activeObject = prefab;
				// 選択したPrefabがプロジェクトウィンドウ内でハイライトされるようにする
				EditorGUIUtility.PingObject(prefab);
			}
		}

		EditorGUILayout.EndScrollView();
	}

	/// <summary>
	/// プロジェクト内のPrefabからTextMeshProまたはTextMeshProUGUIコンポーネントを含むものを検索する関数
	/// </summary>
	private void FindTMPPrefabs()
	{
		// 前回の検索結果をクリア
		tmpPrefabs.Clear();

		// プロジェクト内のすべてのPrefabのアセットGUIDを取得
		string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

		// すべてのPrefabを順番に確認
		foreach (string guid in guids)
		{
			// GUIDからPrefabのパスを取得
			string path = AssetDatabase.GUIDToAssetPath(guid);
			// パスからPrefabのゲームオブジェクトをロード
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

			if (prefab != null)
			{
				// Prefabの子孫オブジェクトまで含めて、TextMeshPro関連コンポーネントが存在するか確認
				if (prefab.GetComponentInChildren<TextMeshProUGUI>(true) != null ||
					prefab.GetComponentInChildren<TextMeshPro>(true) != null)
				{
					// コンポーネントが含まれていた場合はリストに追加
					tmpPrefabs.Add(prefab);
				}
			}
		}

		// コンソールに検索結果の件数を表示
		Debug.Log($"TMP付きPrefabが {tmpPrefabs.Count} 個見つかりました。");
	}

	/// <summary>
	/// 見つかったPrefab内のTMP付きオブジェクトにAudioSourceを追加する関数
	/// </summary>
	private void AddAudioSourceToTMPPrefabs()
	{
		foreach (GameObject prefab in tmpPrefabs)
		{
			// Prefabを編集可能な状態にする
			GameObject prefabInstance = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(prefab));

			// TMPコンポーネントを持つすべてのオブジェクトを取得
			TMP_Text[] tmpObjects = prefabInstance.GetComponentsInChildren<TMP_Text>(true);
			foreach (TMPro.TMP_Text tmp in tmpObjects)
			{
				// AudioSourceが既に存在しない場合のみ追加
				if (tmp.GetComponent<AudioSource>() == null)
				{
					tmp.gameObject.AddComponent<AudioSource>();
				}
			}

			// 編集したPrefabを保存して閉じる
			PrefabUtility.SaveAsPrefabAsset(prefabInstance, AssetDatabase.GetAssetPath(prefab));
			PrefabUtility.UnloadPrefabContents(prefabInstance);
		}

		Debug.Log("AudioSourceコンポーネントの追加が完了しました。");
	}
}
