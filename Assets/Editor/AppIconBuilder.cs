using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// アプリのアイコンを描いて書き出し、Androidのアイコンに設定するエディタ専用の道具。
/// 絵を用意しなくて済むよう、計算で描いている。
/// 何度実行しても同じ絵になる。
/// </summary>
public static class AppIconBuilder
{
	const string iconPath = "Assets/NotesShooter/Textures/AppIcon.png";

	[Tooltip("書き出す大きさ。Androidの各サイズへはUnityが縮小してくれる")]
	const int iconSize = 512;

	[Tooltip("画面の奥の色。他の画面の背景と同じ")]
	static readonly Color deepColor = new Color(0.055f, 0.271f, 0.408f, 1.0f);
	[Tooltip("明るい方の青")]
	static readonly Color midColor = new Color(0.208f, 0.588f, 0.788f, 1.0f);
	[Tooltip("差し色の水色")]
	static readonly Color accentColor = new Color(0.404f, 0.827f, 1.0f, 1.0f);
	[Tooltip("的についている照準の輪の色")]
	static readonly Color reticleColor = new Color(0.78f, 0.78f, 1.0f, 1.0f);

	[MenuItem("Tools/NotesShooter/アプリのアイコンを作る")]
	public static void Build()
	{
		Texture2D texture = DrawIcon();

		byte[] png = texture.EncodeToPNG();
		Object.DestroyImmediate(texture);

		string directory = Path.GetDirectoryName(iconPath);
		if (Directory.Exists(directory) == false)
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllBytes(iconPath, png);
		AssetDatabase.ImportAsset(iconPath, ImportAssetOptions.ForceUpdate);

		ApplyImportSettings();
		AssignToAndroid();

		Debug.Log("アプリのアイコンを作りました: " + iconPath);
	}

	// ------------------------------------------------------------
	// 絵を描く
	// ------------------------------------------------------------

	static Texture2D DrawIcon()
	{
		Texture2D texture = new Texture2D(iconSize, iconSize, TextureFormat.RGBA32, false);
		Color[] pixels = new Color[iconSize * iconSize];

		for (int y = 0; y < iconSize; y++)
		{
			for (int x = 0; x < iconSize; x++)
			{
				//SetPixelsは配列の先頭が画像の下端なので、yはそのまま「上向きが正」になる
				Vector2 point = new Vector2(x + 0.5f, y + 0.5f);
				pixels[y * iconSize + x] = DrawPixel(point);
			}
		}

		texture.SetPixels(pixels);
		texture.Apply();

		return texture;
	}

	static Color DrawPixel(Vector2 point)
	{
		Color color = Background(point);

		//的についている照準の輪。ゲーム中の的と同じ形にして、遊んだ人が分かるようにする
		float ring = RingMask(point, new Vector2(256, 256), 196.0f, 13.0f);
		float ticks = TickMask(point, new Vector2(256, 256), 196.0f);
		float reticle = Mathf.Max(ring, ticks);
		color = Blend(color, reticleColor, reticle * 0.75f);

		//音符。小さく表示された時に一番残る形なので、白で大きく置く
		float note = NoteMask(point);
		//縁に濃い影を付けて、明るい背景でも形が分かるようにする
		float shadow = NoteMask(point + new Vector2(-6.0f, 6.0f));
		color = Blend(color, new Color(0.02f, 0.10f, 0.18f, 1.0f), Mathf.Clamp01(shadow - note) * 0.55f);
		color = Blend(color, Color.white, note);

		return color;
	}

	/// <summary>
	/// 左上が明るく、四隅が暗い青のグラデーション
	/// </summary>
	static Color Background(Vector2 point)
	{
		Vector2 uv = point / iconSize;

		//左上から差す光
		float light = 1.0f - Mathf.Clamp01(Vector2.Distance(uv, new Vector2(0.28f, 0.78f)) * 1.35f);
		Color color = Color.Lerp(deepColor, midColor, light * light);

		//斜めに走る光の筋。ゲームのレンズフレアと同じ雰囲気を出す
		float streak = 1.0f - Mathf.Clamp01(Mathf.Abs((uv.x - uv.y) - 0.12f) * 7.0f);
		color = Color.Lerp(color, accentColor, streak * streak * 0.18f);

		//四隅を落として、丸く切り抜かれても中央に目が行くようにする
		float vignette = 1.0f - Mathf.Clamp01((Vector2.Distance(uv, new Vector2(0.5f, 0.5f)) - 0.32f) * 2.2f);
		color = Color.Lerp(deepColor * 0.55f, color, vignette);

		color.a = 1.0f;
		return color;
	}

	/// <summary>
	/// 輪っか。中心からの距離が半径付近なら1に近づく
	/// </summary>
	static float RingMask(Vector2 point, Vector2 center, float radius, float thickness)
	{
		float distance = Mathf.Abs(Vector2.Distance(point, center) - radius);
		return 1.0f - SmoothEdge(thickness - 2.0f, thickness, distance);
	}

	/// <summary>
	/// 輪の上下左右に付く短い目盛り
	/// </summary>
	static float TickMask(Vector2 point, Vector2 center, float radius)
	{
		Vector2 local = point - center;

		float horizontal = BarMask(Mathf.Abs(local.x), Mathf.Abs(local.y), radius);
		float vertical = BarMask(Mathf.Abs(local.y), Mathf.Abs(local.x), radius);

		return Mathf.Max(horizontal, vertical);
	}

	static float BarMask(float along, float across, float radius)
	{
		//輪をまたぐ長さの短い棒
		float inside = 1.0f - SmoothEdge(radius + 26.0f, radius + 28.0f, along);
		float outside = SmoothEdge(radius - 28.0f, radius - 26.0f, along);
		float width = 1.0f - SmoothEdge(11.0f, 13.0f, across);

		return Mathf.Min(Mathf.Min(inside, outside), width);
	}

	/// <summary>
	/// 連桁の付いた二つの音符。丸い頭と縦棒と、上をつなぐ帯で作る
	/// </summary>
	static float NoteMask(Vector2 point)
	{
		Vector2 headA = new Vector2(186.0f, 168.0f);
		Vector2 headB = new Vector2(330.0f, 198.0f);

		float mask = 0.0f;

		//傾けた楕円の頭
		mask = Mathf.Max(mask, EllipseMask(point, headA, 58.0f, 44.0f, -22.0f));
		mask = Mathf.Max(mask, EllipseMask(point, headB, 58.0f, 44.0f, -22.0f));

		//頭の右側から上へ伸びる縦棒
		mask = Mathf.Max(mask, RectMask(point, 226.0f, 248.0f, headA.y, 362.0f));
		mask = Mathf.Max(mask, RectMask(point, 370.0f, 392.0f, headB.y, 392.0f));

		//縦棒の上をつなぐ帯。右上がりに傾ける
        if (226.0f <= point.x && point.x <= 392.0f)
        {
            float rate = (point.x - 226.0f) / (392.0f - 226.0f);
            float top = Mathf.Lerp(362.0f, 392.0f, rate);
            mask = Mathf.Max(mask, RectMask(point, 226.0f, 392.0f, top - 42.0f, top));
        }

		return mask;
	}

	static float EllipseMask(Vector2 point, Vector2 center, float radiusX, float radiusY, float degree)
	{
		Vector2 local = point - center;

		float radian = degree * Mathf.Deg2Rad;
		float cos = Mathf.Cos(radian);
		float sin = Mathf.Sin(radian);
		Vector2 rotated = new Vector2(local.x * cos - local.y * sin, local.x * sin + local.y * cos);

		//楕円を円に直してから距離を測る
		Vector2 scaled = new Vector2(rotated.x / radiusX, rotated.y / radiusY);
		float distance = scaled.magnitude;

		//縁の1ピクセル分だけなめらかにする
		float smooth = 1.0f / Mathf.Min(radiusX, radiusY);
		return 1.0f - SmoothEdge(1.0f - smooth, 1.0f + smooth, distance);
	}

	static float RectMask(Vector2 point, float left, float right, float bottom, float top)
	{
		float x = Mathf.Min(SmoothEdge(left - 1.0f, left + 1.0f, point.x),
			1.0f - SmoothEdge(right - 1.0f, right + 1.0f, point.x));
		float y = Mathf.Min(SmoothEdge(bottom - 1.0f, bottom + 1.0f, point.y),
			1.0f - SmoothEdge(top - 1.0f, top + 1.0f, point.y));

		return Mathf.Min(x, y);
	}

	/// <summary>
	/// 境界をなめらかにする。edge0以下で0、edge1以上で1になる。
	/// Unityの Mathf.SmoothStep は「2つの値をtで補間する」別物なので使えない
	/// </summary>
	static float SmoothEdge(float edge0, float edge1, float x)
	{
		float rate = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
		return rate * rate * (3.0f - 2.0f * rate);
	}

	static Color Blend(Color background, Color foreground, float rate)
	{
		rate = Mathf.Clamp01(rate);
		return Color.Lerp(background, foreground, rate);
	}

	// ------------------------------------------------------------
	// 設定
	// ------------------------------------------------------------

	static void ApplyImportSettings()
	{
		TextureImporter importer = AssetImporter.GetAtPath(iconPath) as TextureImporter;
		if (importer == null)
		{
			return;
		}

		//アイコンは縮小して使われるので、圧縮による滲みを避ける
		importer.textureType = TextureImporterType.Default;
		importer.mipmapEnabled = false;
		importer.npotScale = TextureImporterNPOTScale.None;
		importer.alphaIsTransparency = true;
		importer.textureCompression = TextureImporterCompression.Uncompressed;
		importer.maxTextureSize = iconSize;
		importer.SaveAndReimport();
	}

	static void AssignToAndroid()
	{
		Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
		if (icon == null)
		{
			Debug.LogError("書き出したアイコンを読み込めませんでした");
			return;
		}

		int[] sizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);
		Texture2D[] icons = new Texture2D[sizes.Length];
		for (int i = 0; i < icons.Length; i++)
		{
			//どの大きさにも同じ絵を渡す。実際の縮小はUnityが行う
			icons[i] = icon;
		}

		PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
		AssetDatabase.SaveAssets();

		Debug.Log("Androidのアイコン " + icons.Length + " 枠に設定しました");
	}
}
