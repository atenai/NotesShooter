using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// 的を壊した時とスポーンした時に出すレンズフレアを用意するエディタ専用の道具。
/// フレアのデータとプレハブを作り、的のプレハブへ割り当てる。
/// 何度実行しても同じ結果になる。
/// </summary>
public static class LensFlareBuilder
{
	const string prefabFolder = "Assets/NotesShooter/Prefab/Target/";

	//プロジェクトに入っているURP_Flares_Packのアセットを使う。
	//破壊は光条のある明るい閃光、スポーンは星に横の光条が伸びるものを選んだ
	const string hitDataPath = "Assets/URP_Flares_Pack/Prefabs/Point/Point_Light_Flare_4.asset";
	const string spawnDataPath = "Assets/URP_Flares_Pack/Prefabs/Point/Point_Light_Flare_7.asset";
	const string hitPrefabPath = prefabFolder + "LensFlare_TargetHit.prefab";
	const string spawnPrefabPath = prefabFolder + "LensFlare_TargetSpawn.prefab";

	[Tooltip("的のプレハブ。ここへフレアを割り当てる")]
	static readonly string[] targetPrefabNames = { "BlueTarget", "RedTarget", "PurpleTarget" };
	[Tooltip("ゲーム中のシーン。カメラのポストプロセスを入れる")]
	static readonly string[] gameSceneNames = { "Stage2", "MasterStage" };

	[MenuItem("Tools/NotesShooter/的のレンズフレアを用意する")]
	public static void Build()
	{
		LensFlareDataSRP hitData = AssetDatabase.LoadAssetAtPath<LensFlareDataSRP>(hitDataPath);
		LensFlareDataSRP spawnData = AssetDatabase.LoadAssetAtPath<LensFlareDataSRP>(spawnDataPath);

		if (hitData == null || spawnData == null)
		{
			Debug.LogError("レンズフレアのアセットが見つかりませんでした。" + hitDataPath + " / " + spawnDataPath);
			return;
		}

		//このパックのフレアは太陽光源向けで大きいので、scaleで小さく絞る
		GameObject hitPrefab = CreateFlarePrefab(hitPrefabPath, "LensFlare_TargetHit", hitData, 0.7f, 0.30f, 0.16f);
		GameObject spawnPrefab = CreateFlarePrefab(spawnPrefabPath, "LensFlare_TargetSpawn", spawnData, 0.45f, 0.45f, 0.11f);

		AssignToTargets(hitPrefab, spawnPrefab);
		AssignToScenes(hitPrefab, spawnPrefab);

		AssetDatabase.SaveAssets();
		Debug.Log("的のレンズフレアを用意しました");
	}

	// ------------------------------------------------------------
	// フレアのプレハブ
	// ------------------------------------------------------------

	static GameObject CreateFlarePrefab(string path, string name, LensFlareDataSRP data, float peakIntensity, float decayTime, float scale)
	{
		GameObject root = new GameObject(name);

		LensFlareComponentSRP flare = root.AddComponent<LensFlareComponentSRP>();
		flare.lensFlareData = data;
		flare.intensity = 0.0f;
		//ライトが付いていないので、ライトの形で明るさを決める設定は切る
		flare.attenuationByLightShape = false;
		//遮蔽の判定は深度を何度も読むので、短く光るだけのこの用途では切る
		flare.useOcclusion = false;
		flare.allowOffScreen = false;
		//このパックは大きめに作られているので、ここで絞って的の演出に合う大きさにする
		flare.scale = scale;
		//距離で暗くならないようにしておく。的はプレイヤーのすぐ前に出る
		flare.distanceAttenuationCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);
		flare.scaleByDistanceCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);

		LensFlareBurst burst = root.AddComponent<LensFlareBurst>();
		SerializedObject serialized = new SerializedObject(burst);
		serialized.FindProperty("peakIntensity").floatValue = peakIntensity;
		serialized.FindProperty("decayTime").floatValue = decayTime;
		serialized.ApplyModifiedPropertiesWithoutUndo();

		GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
		Object.DestroyImmediate(root);

		return prefab;
	}

	// ------------------------------------------------------------
	// 的への割り当て
	// ------------------------------------------------------------

	static void AssignToTargets(GameObject hitPrefab, GameObject spawnPrefab)
	{
		foreach (string targetName in targetPrefabNames)
		{
			string path = prefabFolder + targetName + ".prefab";
			GameObject root = PrefabUtility.LoadPrefabContents(path);

			Target target = root.GetComponent<Target>();
			if (target == null)
			{
				Debug.LogWarning(path + " にTargetが付いていません");
				PrefabUtility.UnloadPrefabContents(root);
				continue;
			}

			SerializedObject serialized = new SerializedObject(target);
			serialized.FindProperty("hitLensFlarePrefab").objectReferenceValue = hitPrefab;
			serialized.FindProperty("spawnLensFlarePrefab").objectReferenceValue = spawnPrefab;
			serialized.ApplyModifiedPropertiesWithoutUndo();

			PrefabUtility.SaveAsPrefabAsset(root, path);
			PrefabUtility.UnloadPrefabContents(root);

			Debug.Log(targetName + " にレンズフレアを割り当てました");
		}
	}

	// ------------------------------------------------------------
	// カメラ
	// ------------------------------------------------------------

	/// <summary>
	/// シーンに直接置かれている的にも割り当てる。
	/// ステージの的はプレハブと繋がっていないので、プレハブ側だけ直しても何も起きない。
	/// あわせてカメラのポストプロセスも入れる。
	/// URPはレンズフレアをポストプロセスの中で描くので、切れていると何も出ない
	/// </summary>
	static void AssignToScenes(GameObject hitPrefab, GameObject spawnPrefab)
	{
		string openedScenePath = EditorSceneManager.GetActiveScene().path;
		EditorSceneManager.SaveOpenScenes();

		foreach (string sceneName in gameSceneNames)
		{
			string path = "Assets/NotesShooter/Scenes/" + sceneName + ".unity";
			UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

			int changed = 0;

			int assigned = 0;
			foreach (Target target in Object.FindObjectsOfType<Target>(true))
			{
				//壁は撃ち抜く的ではなく数も多いので対象にしない
				if (target is WallTarget)
				{
					continue;
				}

				SerializedObject targetObject = new SerializedObject(target);
				targetObject.FindProperty("hitLensFlarePrefab").objectReferenceValue = hitPrefab;
				targetObject.FindProperty("spawnLensFlarePrefab").objectReferenceValue = spawnPrefab;
				targetObject.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(target);
				assigned++;
			}

			if (0 < assigned)
			{
				changed++;
				Debug.Log(sceneName + " の的 " + assigned + " 個にレンズフレアを割り当てました");
			}
			foreach (Camera camera in Object.FindObjectsOfType<Camera>(true))
			{
				UnityEngine.Rendering.Universal.UniversalAdditionalCameraData cameraData =
					camera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
				if (cameraData == null || cameraData.renderPostProcessing == true)
				{
					continue;
				}

				cameraData.renderPostProcessing = true;
				EditorUtility.SetDirty(cameraData);
				changed++;
				Debug.Log(sceneName + " の " + camera.name + " のポストプロセスを有効にしました");
			}

			if (0 < changed)
			{
				EditorSceneManager.MarkSceneDirty(scene);
				EditorSceneManager.SaveOpenScenes();
			}
		}

		if (string.IsNullOrEmpty(openedScenePath) == false)
		{
			EditorSceneManager.OpenScene(openedScenePath, OpenSceneMode.Single);
		}
	}
}
