using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// ガラスのシェーダーを使っているボタンに、押した時の演出を付けて回るエディタ専用の道具。
/// 対象はビルド設定に入っている全シーンと、NotesShooterのプレハブ全部。
/// 何度実行しても同じ結果になる。
/// </summary>
public static class UIButtonPressEffectInstaller
{
	const string glassShaderName = "NotesShooter/LiquidGlass";
	const string prefabFolder = "Assets/NotesShooter/Prefab";

	[Tooltip("押している間の色。少し暗く青寄りにして、押したのが分かるようにする")]
	static readonly Color pressedColor = new Color(0.72f, 0.82f, 0.90f, 1.0f);
	[Tooltip("押せないボタンの色")]
	static readonly Color disabledColor = new Color(1.0f, 1.0f, 1.0f, 0.45f);
	[Tooltip("色が変わりきるまでの時間。長いと押した瞬間の反応が鈍く感じる")]
	const float fadeDuration = 0.06f;
	[Tooltip("押している間の大きさ")]
	const float pressedScale = 0.92f;

	[MenuItem("Tools/NotesShooter/ガラスのボタンに押した演出を付ける")]
	public static void Install()
	{
		List<string> report = new List<string>();

		InstallToPrefabs(report);
		InstallToScenes(report);

		if (report.Count == 0)
		{
			Debug.LogWarning("ガラスのシェーダーを使ったボタンが見つかりませんでした");
			return;
		}

		Debug.Log("押した演出を付けました (" + report.Count + "個)\n" + string.Join("\n", report));
	}

	static void InstallToPrefabs(List<string> report)
	{
		string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabFolder });
		foreach (string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			GameObject root = PrefabUtility.LoadPrefabContents(path);

			int changed = ApplyToTree(root, path, report);

			if (0 < changed)
			{
				PrefabUtility.SaveAsPrefabAsset(root, path);
			}

			PrefabUtility.UnloadPrefabContents(root);
		}
	}

	static void InstallToScenes(List<string> report)
	{
		string openedScenePath = EditorSceneManager.GetActiveScene().path;
		EditorSceneManager.SaveOpenScenes();

		foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
		{
			if (buildScene.enabled == false)
			{
				continue;
			}

			UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(buildScene.path, OpenSceneMode.Single);

			int changed = 0;
			foreach (GameObject root in scene.GetRootGameObjects())
			{
				changed += ApplyToTree(root, buildScene.path, report);
			}

			if (0 < changed)
			{
				EditorSceneManager.MarkSceneDirty(scene);
				EditorSceneManager.SaveOpenScenes();
			}
		}

		//元々開いていたシーンへ戻しておく
		if (string.IsNullOrEmpty(openedScenePath) == false)
		{
			EditorSceneManager.OpenScene(openedScenePath, OpenSceneMode.Single);
		}
	}

	/// <summary>
	/// 対象の中のガラスのボタンを全部直す。直した数を返す
	/// </summary>
	static int ApplyToTree(GameObject root, string ownerPath, List<string> report)
	{
		int changed = 0;

		foreach (Button button in root.GetComponentsInChildren<Button>(true))
		{
			if (IsGlassButton(button) == false)
			{
				continue;
			}

			ApplyColors(button);

			UIButtonPressEffect effect = button.GetComponent<UIButtonPressEffect>();
			if (effect == null)
			{
				effect = button.gameObject.AddComponent<UIButtonPressEffect>();
			}

			//既に付いている物も同じ設定に揃える
			SerializedObject serialized = new SerializedObject(effect);
			serialized.FindProperty("pressedScale").floatValue = pressedScale;
			serialized.ApplyModifiedPropertiesWithoutUndo();

			EditorUtility.SetDirty(button.gameObject);
			report.Add("  " + ownerPath + " : " + GetPath(button.transform));
			changed++;
		}

		return changed;
	}

	/// <summary>
	/// そのボタンの見た目がガラスのシェーダーで描かれているか
	/// </summary>
	static bool IsGlassButton(Button button)
	{
		Graphic graphic = button.targetGraphic;
		if (graphic == null)
		{
			graphic = button.GetComponent<Graphic>();
		}

		if (graphic == null || graphic.material == null || graphic.material.shader == null)
		{
			return false;
		}

		return graphic.material.shader.name == glassShaderName;
	}

	/// <summary>
	/// 押した時に色が変わるようにする。
	/// ガラスのシェーダー側も頂点カラーを反映するようにしてあるので、
	/// 大きさの変化と合わせて押した事が伝わる
	/// </summary>
	static void ApplyColors(Button button)
	{
		button.transition = Selectable.Transition.ColorTint;

		if (button.targetGraphic == null)
		{
			button.targetGraphic = button.GetComponent<Graphic>();
		}

		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		//スマホには「乗せている」状態が無いので、通常時と同じにしておく
		colors.highlightedColor = Color.white;
		colors.pressedColor = pressedColor;
		colors.selectedColor = Color.white;
		colors.disabledColor = disabledColor;
		colors.colorMultiplier = 1.0f;
		colors.fadeDuration = fadeDuration;
		button.colors = colors;
	}

	static string GetPath(Transform target)
	{
		string path = target.name;
		Transform parent = target.parent;
		while (parent != null)
		{
			path = parent.name + "/" + path;
			parent = parent.parent;
		}

		return path;
	}
}
