using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

/// <summary>
/// 撃った瞬間に銃口で光るプレハブを用意して、プレイヤーの銃へ割り当てるエディタ専用の道具。
/// 何度実行しても同じ結果になる。
/// </summary>
public static class MuzzleFlashBuilder
{
	const string prefabPath = "Assets/NotesShooter/Prefab/Player/MuzzleFlash.prefab";
	const string playerPrefabPath = "Assets/NotesShooter/Prefab/Player/Player.prefab";
	[Tooltip("銃口の光はくっきりした小さい輝きが合う")]
	const string flareDataPath = "Assets/URP_Flares_Pack/Prefabs/Point/Point_Light_Flare_2.asset";

	[Tooltip("火薬の光。少し赤みのある白")]
	static readonly Color flashColor = new Color(1.0f, 0.87f, 0.65f, 1.0f);

	[MenuItem("Tools/NotesShooter/銃のマズルフラッシュを用意する")]
	public static void Build()
	{
		LensFlareDataSRP flareData = AssetDatabase.LoadAssetAtPath<LensFlareDataSRP>(flareDataPath);
		if (flareData == null)
		{
			Debug.LogError("レンズフレアのアセットが見つかりませんでした: " + flareDataPath);
			return;
		}

		GameObject prefab = CreatePrefab(flareData);
		AssignToPlayer(prefab);

		AssetDatabase.SaveAssets();
		Debug.Log("銃のマズルフラッシュを用意しました");
	}

	static GameObject CreatePrefab(LensFlareDataSRP flareData)
	{
		GameObject root = new GameObject("MuzzleFlash");

		//銃と手前の地面を照らす明かり。影は落とさない
		Light muzzleLight = root.AddComponent<Light>();
		muzzleLight.type = LightType.Point;
		muzzleLight.color = flashColor;
		muzzleLight.range = 8.0f;
		muzzleLight.intensity = 0.0f;
		muzzleLight.shadows = LightShadows.None;

		//銃口のギラつき
		LensFlareComponentSRP flare = root.AddComponent<LensFlareComponentSRP>();
		flare.lensFlareData = flareData;
		flare.intensity = 0.0f;
		//明るさは自前で動かすので、ライトの形に任せない
		flare.attenuationByLightShape = false;
		flare.useOcclusion = false;
		flare.allowOffScreen = false;
		//銃口はカメラのすぐ前にあるので、かなり小さくしないと画面を覆ってしまう
		flare.scale = 0.05f;
		flare.distanceAttenuationCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);
		flare.scaleByDistanceCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);

		root.AddComponent<MuzzleFlash>();

		GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
		Object.DestroyImmediate(root);

		return prefab;
	}

	/// <summary>
	/// 左右の銃へ割り当てる。銃はPlayerのプレハブの中に入っている
	/// </summary>
	static void AssignToPlayer(GameObject muzzleFlashPrefab)
	{
		GameObject root = PrefabUtility.LoadPrefabContents(playerPrefabPath);

		int assigned = 0;

		foreach (LeftGun gun in root.GetComponentsInChildren<LeftGun>(true))
		{
			SerializedObject serialized = new SerializedObject(gun);
			serialized.FindProperty("muzzleFlashPrefab").objectReferenceValue = muzzleFlashPrefab;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			assigned++;
		}

		foreach (RightGun gun in root.GetComponentsInChildren<RightGun>(true))
		{
			SerializedObject serialized = new SerializedObject(gun);
			serialized.FindProperty("muzzleFlashPrefab").objectReferenceValue = muzzleFlashPrefab;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			assigned++;
		}

		if (0 < assigned)
		{
			PrefabUtility.SaveAsPrefabAsset(root, playerPrefabPath);
			Debug.Log("銃 " + assigned + " 丁にマズルフラッシュを割り当てました");
		}
		else
		{
			Debug.LogWarning(playerPrefabPath + " に銃が見つかりませんでした");
		}

		PrefabUtility.UnloadPrefabContents(root);
	}
}
