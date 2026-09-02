using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;

/// <summary>
/// リザルト画面の見た目を組み直すエディタ専用の道具。
/// 中身が「SCORE」と数字だけで、巨大な空のガラス板が場所を取っていたので、
/// スコアを主役に据えて、ランク・撃破数・ハイスコアとの差を周りに置き直す。
/// 何度実行しても同じ結果になる。
/// </summary>
public static class ResultUIBuilder
{
	static readonly Color accentColor = new Color(0.404f, 0.827f, 1.0f, 1.0f);
	static readonly Color softTextColor = new Color(0.804f, 0.906f, 0.965f, 1.0f);
	static readonly Color deepColor = new Color(0.055f, 0.271f, 0.408f, 1.0f);
	[Tooltip("一番上のランクだけ青系から外して、特別さを出す")]
	static readonly Color goldColor = new Color(1.0f, 0.855f, 0.4f, 1.0f);

	const string scenePath = "Assets/NotesShooter/Scenes/Result.unity";
	const string shaderFolder = "Assets/NotesShooter/Shader/";

	static Font font;
	static Material backgroundMaterial;
	static Material panelMaterial;
	static Material buttonMaterial;
	static Sprite barSprite;

	[MenuItem("Tools/NotesShooter/リザルトのUIを組み直す")]
	public static void Build()
	{
		if (EditorSceneManager.GetActiveScene().path != scenePath)
		{
			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
		}

		font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		backgroundMaterial = AssetDatabase.LoadAssetAtPath<Material>(shaderFolder + "M_LiquidGlassBackground.mat");
		panelMaterial = AssetDatabase.LoadAssetAtPath<Material>(shaderFolder + "M_LiquidGlassPanel.mat");
		buttonMaterial = AssetDatabase.LoadAssetAtPath<Material>(shaderFolder + "M_LiquidGlassButton.mat");
		barSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

		if (font == null || backgroundMaterial == null || panelMaterial == null || buttonMaterial == null || barSprite == null)
		{
			Debug.LogError("リザルトの組み直しに必要なフォントかマテリアルかスプライトが見つかりませんでした");
			return;
		}

		Canvas canvas = Object.FindObjectOfType<Canvas>();
		Transform canvasTransform = canvas.transform;

		//他の画面と揃える。ここだけ1000x500の横幅基準になっていて、
		//端末によって縦の余白が変わってしまっていた
		CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 1.0f;

		//前の作りで使っていた物と、この道具が前回作った物を消す
		string[] removeNames =
		{
			"Image_Back", "Image_Result", "Panel_Glass",
			"Line_GlassTop", "Line_GlassBottom", "Line_GlassLeft", "Line_GlassRight",
			"Panel_Header", "Line_Header", "Text_Title", "Text_StageName",
			"Text_ScoreLabel", "Text_Score", "Text_NewRecord", "Line_Footer", "Text_HighScore",
			"Text_ResultLabel", "Text_RankLabel", "Panel_RankBadge",
			"Image_GaugeTrack", "Text_ScoreDelta", "Panel_NewRecord", "Panel_Stats",
			"Text_NextLabel", "Button_StageSelect", "Button_Title", "Text_Version",
		};
		foreach (string name in removeNames)
		{
			DestroyByName(canvasTransform, name);
		}

		//背景。他の画面と同じ、ゆっくり動く水色
		Image background = canvasTransform.Find("Image_Background") != null
			? canvasTransform.Find("Image_Background").GetComponent<Image>()
			: CreateImage(canvasTransform, "Image_Background", null, Color.white);
		background.material = backgroundMaterial;
		background.sprite = null;
		background.color = Color.white;
		background.raycastTarget = false;
		Stretch(background.rectTransform);
		background.transform.SetAsFirstSibling();

		//見出し。タイトルやステージセレクトと同じ位置に置いて、同じ画面の続きに見せる
		CreateText(canvasTransform, "Text_ResultLabel", "RESULT", 26, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -40), new Vector2(700, 40));
		Text textStageName = CreateText(canvasTransform, "Text_StageName", "ステージ 1", 62, Color.white, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -80), new Vector2(900, 86));

		//右上のランク。角丸の半径を大きさの半分にすると円になる
		CreateText(canvasTransform, "Text_RankLabel", "RANK", 26, accentColor, TextAnchor.MiddleRight,
			new Vector2(1, 1), new Vector2(1, 1), new Vector2(-230, -85), new Vector2(300, 40));
		Image rankBadge = CreateImage(canvasTransform, "Panel_RankBadge", panelMaterial, Color.white);
		SetRect(rankBadge.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-64, -30), new Vector2(150, 150));
		rankBadge.raycastTarget = false;
		AddLiquidGlassRect(rankBadge.gameObject, 75.0f);
		Text textRank = CreateText(rankBadge.transform, "Text_Rank", "S", 84, goldColor, TextAnchor.MiddleCenter,
			new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
		Stretch(textRank.rectTransform);

		Image headerLine = CreateImage(canvasTransform, "Line_Header", null, new Color(1, 1, 1, 0.4f));
		SetRect(headerLine.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -206), new Vector2(-120, 2));
		headerLine.raycastTarget = false;

		//主役のスコア
		CreateText(canvasTransform, "Text_ScoreLabel", "SCORE", 34, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -250), new Vector2(500, 44));
		Text textScore = CreateText(canvasTransform, "Text_Score", "0", 200, Color.white, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -296), new Vector2(1000, 230));
		Shadow shadow = textScore.gameObject.AddComponent<Shadow>();
		shadow.effectColor = new Color(0, 0, 0, 0.35f);
		shadow.effectDistance = new Vector2(4, -6);

		//自己ベストに対する今回の位置をゲージで見せる
		Image gaugeTrack = CreateImage(canvasTransform, "Image_GaugeTrack", null, new Color(1, 1, 1, 0.18f));
		SetRect(gaugeTrack.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -540), new Vector2(700, 8));
		gaugeTrack.sprite = null;
		gaugeTrack.type = Image.Type.Simple;
		gaugeTrack.raycastTarget = false;

		//組み込みのスプライトは角丸の縁を持っていて、数pxの高さに潰すと縁が全体に広がって
		//黒っぽい塊に見えてしまう。素の板を左端から横に伸ばして表す
		Image gauge = CreateImage(gaugeTrack.transform, "Image_ScoreGauge", null, accentColor);
		gauge.sprite = null;
		gauge.type = Image.Type.Simple;
		gauge.raycastTarget = false;
		RectTransform gaugeRect = gauge.rectTransform;
		gaugeRect.anchorMin = new Vector2(0, 0);
		gaugeRect.anchorMax = new Vector2(0, 1);
		gaugeRect.pivot = new Vector2(0, 0.5f);
		gaugeRect.offsetMin = Vector2.zero;
		gaugeRect.offsetMax = Vector2.zero;

		//ベストに届かなかった時はここに差を出す
		Text textDelta = CreateText(canvasTransform, "Text_ScoreDelta", "", 32, softTextColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -566), new Vector2(800, 48));

		//更新した時だけ出す。画面で唯一の不透明な色面にして目立たせる
		Image newRecordPanel = CreateImage(canvasTransform, "Panel_NewRecord", null, accentColor);
		SetRect(newRecordPanel.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -562), new Vector2(520, 58));
		newRecordPanel.sprite = barSprite;
		newRecordPanel.type = Image.Type.Sliced;
		newRecordPanel.raycastTarget = false;
		Text textNewRecord = CreateText(newRecordPanel.transform, "Text_NewRecord", "NEW RECORD", 30, deepColor, TextAnchor.MiddleCenter,
			new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
		Stretch(textNewRecord.rectTransform);

		//右側にこのプレイの内訳。ステージセレクトの情報パネルと同じ作りにする
		Image statsPanel = CreateImage(canvasTransform, "Panel_Stats", panelMaterial, Color.white);
		SetRect(statsPanel.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-64, -250), new Vector2(460, 300));
		statsPanel.raycastTarget = false;
		AddLiquidGlassRect(statsPanel.gameObject, 30.0f);

		CreateText(statsPanel.transform, "Text_StatsTitle", "このプレイ", 24, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(28, -22), new Vector2(400, 36));
		Image statsLine = CreateImage(statsPanel.transform, "Line_Stats", null, new Color(1, 1, 1, 0.35f));
		SetRect(statsLine.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(28, -64), new Vector2(404, 2));
		statsLine.raycastTarget = false;

		Text textShot = CreateStatRow(statsPanel.transform, "Shot", "たおした的", "0", -86);
		Text textHighScore = CreateStatRow(statsPanel.transform, "HighScore", "これまでのベスト", "0", -152);
		Text textRate = CreateStatRow(statsPanel.transform, "Rate", "達成率", "0 %", -218);

		//次にどうするかを選ばせる。画面のどこを触っても進む作りをやめて、行き先をボタンで分ける
		CreateText(canvasTransform, "Text_NextLabel", "つぎはどうしますか？", 30, softTextColor, TextAnchor.MiddleLeft,
			new Vector2(0, 0), new Vector2(0, 0), new Vector2(64, 400), new Vector2(800, 44));

		Button buttonStageSelect = CreateMenuButton(canvasTransform, "Button_StageSelect", "ステージセレクト", new Vector2(64, 190));
		Button buttonTitle = CreateMenuButton(canvasTransform, "Button_Title", "タイトルへ", new Vector2(516, 190));

		//下側。ステージセレクトと同じ位置に線を置く
		Image footerLine = CreateImage(canvasTransform, "Line_Footer", null, new Color(1, 1, 1, 0.45f));
		SetRect(footerLine.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 150), new Vector2(-120, 2));
		footerLine.raycastTarget = false;

		Text textVersion = CreateText(canvasTransform, "Text_Version", "v " + Application.version, 22, softTextColor, TextAnchor.MiddleRight,
			new Vector2(1, 0), new Vector2(1, 0), new Vector2(-60, 62), new Vector2(400, 34));

		//元からある案内文はボタンに置き換わったので隠す。
		//これはプレハブインスタンスの一部でDestroyImmediateでは消せない
		Transform quitTransform = canvasTransform.Find("Text_GameQuit");
		if (quitTransform != null)
		{
			quitTransform.gameObject.SetActive(false);
		}

		//フェード用の黒画像は必ず一番手前。押せてしまうとタップが吸われるので当たり判定は切る
		Image fadeImage = null;
		Transform fadeTransform = canvasTransform.Find("Image_FadeResult");
		if (fadeTransform != null)
		{
			fadeImage = fadeTransform.GetComponent<Image>();
			fadeImage.raycastTarget = false;
			fadeTransform.SetAsLastSibling();
		}

		WireResultScore(textScore, textStageName, textHighScore, textShot, textRank, textDelta, textRate,
			gauge, newRecordPanel.gameObject, textNewRecord);
		WireButtons(buttonStageSelect, buttonTitle);

		//背景を全面に描くので、カメラは空を描かなくて良い
		Camera camera = Camera.main;
		if (camera != null)
		{
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = deepColor;
		}

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		EditorSceneManager.SaveOpenScenes();
		Debug.Log("リザルトのUIを組み直しました");
	}

	static void WireResultScore(Text textScore, Text textStageName, Text textHighScore, Text textShot,
		Text textRank, Text textDelta, Text textRate, Image gauge, GameObject newRecordObject, Text textNewRecord)
	{
		ResultScore resultScore = Object.FindObjectOfType<ResultScore>();
		if (resultScore == null)
		{
			Debug.LogError("ResultScoreが見つかりませんでした");
			return;
		}

		SerializedObject serialized = new SerializedObject(resultScore);
		serialized.FindProperty("textScore").objectReferenceValue = textScore;
		serialized.FindProperty("textStageName").objectReferenceValue = textStageName;
		serialized.FindProperty("textHighScore").objectReferenceValue = textHighScore;
		serialized.FindProperty("textTargetCount").objectReferenceValue = textShot;
		serialized.FindProperty("textRank").objectReferenceValue = textRank;
		serialized.FindProperty("textDiff").objectReferenceValue = textDelta;
		serialized.FindProperty("textAchieveRate").objectReferenceValue = textRate;
		serialized.FindProperty("imageScoreGauge").objectReferenceValue = gauge;
		serialized.FindProperty("newRecordGameObject").objectReferenceValue = newRecordObject;
		serialized.FindProperty("textNewRecord").objectReferenceValue = textNewRecord;

		//ランクの判定表。割合の高い順に並べる。
		//ステージごとに取れる点の上限が全く違う（今の記録で5500と12800）ので、
		//決め打ちの点数で区切ると、あるステージは永久にSを取れない事になってしまう。
		//自分のベストに対して何%届いたかで見る
		string[] rankNames = { "S", "A", "B", "C", "D" };
		int[] leastRates = { 100, 85, 65, 40, 0 };
		Color[] rankColors = { goldColor, accentColor, Color.white, softTextColor, new Color(0.804f, 0.906f, 0.965f, 0.7f) };

		SerializedProperty thresholds = serialized.FindProperty("rankThresholds");
		thresholds.arraySize = rankNames.Length;
		for (int i = 0; i < rankNames.Length; i++)
		{
			SerializedProperty element = thresholds.GetArrayElementAtIndex(i);
			element.FindPropertyRelative("rankName").stringValue = rankNames[i];
			element.FindPropertyRelative("leastRatePercent").intValue = leastRates[i];
			element.FindPropertyRelative("rankColor").colorValue = rankColors[i];
		}

		serialized.ApplyModifiedPropertiesWithoutUndo();
	}

	/// <summary>
	/// ボタンの押した時の行き先を繋ぎ直す
	/// </summary>
	static void WireButtons(Button buttonStageSelect, Button buttonTitle)
	{
		ResultManager manager = Object.FindObjectOfType<ResultManager>();
		if (manager == null)
		{
			Debug.LogError("ResultManagerが見つかりませんでした");
			return;
		}

		SetOnClick(buttonStageSelect, new UnityAction(manager.RequestStageSelect));
		SetOnClick(buttonTitle, new UnityAction(manager.RequestTitle));
	}

	static void SetOnClick(Button button, UnityAction action)
	{
		//前に繋いだ物が残っていると二重に呼ばれるので、一度全部外す
		for (int i = button.onClick.GetPersistentEventCount() - 1; 0 <= i; i--)
		{
			UnityEventTools.RemovePersistentListener(button.onClick, i);
		}

		UnityEventTools.AddPersistentListener(button.onClick, action);
	}

	/// <summary>
	/// 情報パネルの中の「見出し + 数字」の一行。数字の側を返す
	/// </summary>
	static Text CreateStatRow(Transform parent, string name, string label, string value, float y)
	{
		CreateText(parent, "Text_" + name + "Label", label, 24, softTextColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(28, y), new Vector2(260, 44));
		return CreateText(parent, "Text_" + name, value, 34, Color.white, TextAnchor.MiddleRight,
			new Vector2(1, 1), new Vector2(1, 1), new Vector2(-28, y + 2), new Vector2(280, 48));
	}

	/// <summary>
	/// 下に並べる大きめのボタン。押せる場所である事が分かるよう下線を付ける
	/// </summary>
	static Button CreateMenuButton(Transform parent, string name, string label, Vector2 position)
	{
		Image panel = CreateImage(parent, name, buttonMaterial, Color.white);
		SetRect(panel.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), position, new Vector2(420, 160));
		AddLiquidGlassRect(panel.gameObject, 30.0f);

		Button button = panel.gameObject.AddComponent<Button>();
		button.targetGraphic = panel;

		Text text = CreateText(panel.transform, "Text_Label", label, 34, Color.white, TextAnchor.MiddleCenter,
			new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 10), new Vector2(400, 50));

		Image underline = CreateImage(panel.transform, "Image_Underline", null, accentColor);
		SetRect(underline.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 28), new Vector2(240, 5));
		underline.raycastTarget = false;

		return button;
	}

	static void AddLiquidGlassRect(GameObject target, float radius)
	{
		LiquidGlassRect glass = target.GetComponent<LiquidGlassRect>();
		if (glass == null)
		{
			glass = target.AddComponent<LiquidGlassRect>();
		}

		SerializedObject glassObject = new SerializedObject(glass);
		glassObject.FindProperty("radius").floatValue = radius;
		glassObject.ApplyModifiedPropertiesWithoutUndo();
	}

	static Image CreateImage(Transform parent, string name, Material material, Color color)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.layer = LayerMask.NameToLayer("UI");
		gameObject.transform.SetParent(parent, false);

		Image image = gameObject.AddComponent<Image>();
		image.color = color;
		if (material != null)
		{
			image.material = material;
		}

		return image;
	}

	static Text CreateText(Transform parent, string name, string content, int fontSize, Color color, TextAnchor alignment,
		Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.layer = LayerMask.NameToLayer("UI");
		gameObject.transform.SetParent(parent, false);

		Text text = gameObject.AddComponent<Text>();
		text.font = font;
		text.text = content;
		text.fontSize = fontSize;
		text.color = color;
		text.alignment = alignment;
		text.raycastTarget = false;
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.verticalOverflow = VerticalWrapMode.Overflow;

		Vector2 pivot = new Vector2(
			anchorMin.x == anchorMax.x ? anchorMin.x : 0.5f,
			anchorMin.y == anchorMax.y ? anchorMin.y : 0.5f);
		SetRect(text.rectTransform, anchorMin, anchorMax, pivot, position, size);

		return text;
	}

	static void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
	{
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
		rectTransform.pivot = pivot;
		rectTransform.anchoredPosition = position;
		rectTransform.sizeDelta = size;
	}

	static void Stretch(RectTransform rectTransform)
	{
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
	}

	static void DestroyByName(Transform parent, string name)
	{
		//プレハブインスタンスの一部などDestroyImmediateで消せない相手だと、
		//Findが同じ物を返し続けて終わらなくなるので、消えなければ打ち切る
		Transform target = parent.Find(name);
		while (target != null)
		{
			GameObject gameObject = target.gameObject;
			Object.DestroyImmediate(gameObject);

			if (gameObject != null)
			{
				Debug.LogWarning(name + " は消せませんでした。プレハブの一部の可能性があります");
				return;
			}

			target = parent.Find(name);
		}
	}
}
