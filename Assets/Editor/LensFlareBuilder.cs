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
	const string flareFolder = "Assets/NotesShooter/VFX/LensFlare/";
	const string prefabFolder = "Assets/NotesShooter/Prefab/Target/";

	const string hitDataPath = flareFolder + "LFD_TargetHit.asset";
	const string spawnDataPath = flareFolder + "LFD_TargetSpawn.asset";
	const string hitPrefabPath = prefabFolder + "LensFlare_TargetHit.prefab";
	const string spawnPrefabPath = prefabFolder + "LensFlare_TargetSpawn.prefab";

	[Tooltip("的のプレハブ。ここへフレアを割り当てる")]
	static readonly string[] targetPrefabNames = { "BlueTarget", "RedTarget", "PurpleTarget" };
	[Tooltip("ゲーム中のシーン。カメラのポストプロセスを入れる")]
	static readonly string[] gameSceneNames = { "Stage2", "MasterStage" };

	[MenuItem("Tools/NotesShooter/的のレンズフレアを用意する")]
	public static void Build()
	{
		LensFlareDataSRP hitData = CreateHitData();
		LensFlareDataSRP spawnData = CreateSpawnData();

		GameObject hitPrefab = CreateFlarePrefab(hitPrefabPath, "LensFlare_TargetHit", hitData, 1.0f, 0.30f);
		GameObject spawnPrefab = CreateFlarePrefab(spawnPrefabPath, "LensFlare_TargetSpawn", spawnData, 0.7f, 0.45f);

		AssignToTargets(hitPrefab, spawnPrefab);
		AssignToScenes(hitPrefab, spawnPrefab);

		AssetDatabase.SaveAssets();
		Debug.Log("的のレンズフレアを用意しました");
	}

	// ------------------------------------------------------------
	// フレアのデータ
	// ------------------------------------------------------------

	static LensFlareDataSRP CreateHitData()
	{
		//壊した時は白から橙。爆発の閃光に寄せる
		LensFlareDataElementSRP core = CreateCircle(new Color(1.0f, 0.96f, 0.88f, 1.0f), 0.55f, 1.8f, 0.0f);
		LensFlareDataElementSRP halo = CreateCircle(new Color(1.0f, 0.72f, 0.40f, 0.30f), 1.4f, 0.6f, 0.0f);
		LensFlareDataElementSRP ghost = CreatePolygon(new Color(1.0f, 0.85f, 0.60f, 0.35f), 0.7f, 0.4f, 0.35f, 6);

		return SaveData(hitDataPath, new LensFlareDataElementSRP[] { halo, core, ghost });
	}

	static LensFlareDataSRP CreateSpawnData()
	{
		//出てくる時は水色。ゲームの他の画面と同じ色に合わせる
		LensFlareDataElementSRP core = CreateCircle(new Color(0.75f, 0.94f, 1.0f, 1.0f), 0.40f, 1.3f, 0.0f);
		LensFlareDataElementSRP halo = CreateCircle(new Color(0.40f, 0.83f, 1.0f, 0.28f), 1.1f, 0.5f, 0.0f);

		return SaveData(spawnDataPath, new LensFlareDataElementSRP[] { halo, core });
	}

	static LensFlareDataElementSRP CreateCircle(Color tint, float scale, float intensity, float position)
	{
		//テクスチャを持たない手続き的な円。絵を用意しなくて済む
		LensFlareDataElementSRP element = new LensFlareDataElementSRP();
		element.flareType = SRPLensFlareType.Circle;
		element.tint = tint;
		element.uniformScale = scale;
		element.localIntensity = intensity;
		element.position = position;
		element.blendMode = SRPLensFlareBlendMode.Additive;
		element.fallOff = 0.9f;
		element.edgeOffset = 0.1f;
		return element;
	}

	static LensFlareDataElementSRP CreatePolygon(Color tint, float scale, float intensity, float position, int sideCount)
	{
		LensFlareDataElementSRP element = new LensFlareDataElementSRP();
		element.flareType = SRPLensFlareType.Polygon;
		element.tint = tint;
		element.uniformScale = scale;
		element.localIntensity = intensity;
		element.position = position;
		element.blendMode = SRPLensFlareBlendMode.Additive;
		element.sideCount = sideCount;
		element.fallOff = 0.7f;
		element.edgeOffset = 0.2f;
		return element;
	}

	static LensFlareDataSRP SaveData(string path, LensFlareDataElementSRP[] elements)
	{
		LensFlareDataSRP data = AssetDatabase.LoadAssetAtPath<LensFlareDataSRP>(path);
		if (data == null)
		{
			data = ScriptableObject.CreateInstance<LensFlareDataSRP>();
			AssetDatabase.CreateAsset(data, path);
		}

		data.elements = elements;
		EditorUtility.SetDirty(data);

		return data;
	}

	// ------------------------------------------------------------
	// フレアのプレハブ
	// ------------------------------------------------------------

	static GameObject CreateFlarePrefab(string path, string name, LensFlareDataSRP data, float peakIntensity, float decayTime)
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
