using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// TextMeshProコンポーネントが付いているPrefabを検索するエディター拡張
/// </summary>
public class FindTMPPrefabsEditor : EditorWindow
{
	// スクロールビューのスクロール位置を保持する変数
	private Vector2 scrollPos;
	// 検索結果として見つかったPrefabを格納するリスト
	private List<GameObject> tmpPrefabs = new List<GameObject>();

	/// <summary>
	/// Unityエディタのメニューに「TMP付きPrefabを検索」を追加
	/// </summary>
	[MenuItem("Kashiwabara/TextMeshProコンポーネントが付いているPrefabを検索する")]
	private static void Init()
	{
		// ウィンドウを作成して表示する
		FindTMPPrefabsEditor window = GetWindow<FindTMPPrefabsEditor>();
		window.titleContent = new GUIContent("TextMeshProコンポーネントが付いているPrefabを検索する");
		window.Show();
	}

	/// <summary>
	/// エディターウィンドウの表示処理
	/// </summary>
	private void OnGUI()
	{
		// 検索を開始するためのボタン
		if (GUILayout.Button("TextMeshProコンポーネントが付いているPrefabを検索する"))
		{
			// ボタンが押されたらPrefab検索処理を実行
			FindTMPPrefabs();
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
		Debug.Log($"TMPが含まれるPrefabが {tmpPrefabs.Count} 個見つかりました。");
	}
}
