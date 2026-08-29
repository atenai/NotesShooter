using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;

/// <summary>
/// ステージセレクト画面の見た目を組み直すエディタ専用の道具。
/// 縦スクロールの仕組みはそのまま使い、周りの飾りとカードの見た目だけを作り直す。
/// 何度実行しても同じ結果になるよう、この道具が作った物は毎回消してから作り直す。
/// </summary>
public static class StageSelectUIBuilder
{
	static readonly Color accentColor = new Color(0.404f, 0.827f, 1.0f, 1.0f);
	static readonly Color softTextColor = new Color(0.804f, 0.906f, 0.965f, 1.0f);
	static readonly Color clearedColor = new Color(0.541f, 0.898f, 0.729f, 1.0f);

	const string scenePath = "Assets/NotesShooter/Scenes/StageSelect.unity";
	const string shaderFolder = "Assets/NotesShooter/Shader/";
	const string prefabFolder = "Assets/NotesShooter/Prefab/StageSelect/";

	static Font font;
	static Material backgroundMaterial;
	static Material panelMaterial;
	static Material buttonMaterial;
	static Sprite barSprite;
	static Sprite circleSprite;
	static Sprite checkSprite;

	[MenuItem("Tools/NotesShooter/ステージセレクトのUIを組み直す")]
	public static void Build()
	{
		LoadAssets();
		BuildStageCardPrefab();
		BuildBonusStageCardPrefab();
		BuildScene();
	}

	static void LoadAssets()
	{
		font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		backgroundMaterial = AssetDatabase.LoadAssetAtPath<Material>(shaderFolder + "M_LiquidGlassBackground.mat");
		panelMaterial = AssetDatabase.LoadAssetAtPath<Material>(shaderFolder + "M_LiquidGlassPanel.mat");
		buttonMaterial = AssetDatabase.LoadAssetAtPath<Material>(shaderFolder + "M_LiquidGlassButton.mat");
		//UIのスプライトはunity_builtin_extraに入っていて、Resources.GetBuiltinResourceでは取れずnullが返る。
		//nullのままだとImageのFilled指定が無視され、ゲージが常に満タンで描かれてしまう
		barSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
		circleSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
		checkSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");

		if (font == null || barSprite == null || circleSprite == null || checkSprite == null)
		{
			Debug.LogError("組み込みのフォントかスプライトが取得できませんでした。"
				+ "font=" + (font != null) + " bar=" + (barSprite != null)
				+ " circle=" + (circleSprite != null) + " check=" + (checkSprite != null));
		}

		if (backgroundMaterial == null || panelMaterial == null || buttonMaterial == null)
		{
			Debug.LogError("リキッドグラスのマテリアルが見つかりませんでした");
		}
	}

	// ------------------------------------------------------------
	// ステージのカード
	// ------------------------------------------------------------

	static void BuildStageCardPrefab()
	{
		string path = prefabFolder + "Image_StageSelectButton.prefab";
		GameObject root = PrefabUtility.LoadPrefabContents(path);

		Transform background = ArrangeCardCommon(root);
		Transform button = Find(root, "Button");
		Text buttonText = ReplaceButtonLabel(button);

		//クリア済みを示すマーク。右上に小さく出す
		Transform completeMark = Find(root, "Image_CompleteMark");
		SetRect((RectTransform)completeMark, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-18, -14), new Vector2(52, 52));
		Image completeMarkImage = completeMark.GetComponent<Image>();
		completeMarkImage.sprite = circleSprite;
		completeMarkImage.color = new Color(1, 1, 1, 0.18f);
		completeMarkImage.raycastTarget = false;
		completeMark.SetAsLastSibling();

		Transform checkMark = Find(root, "Image");
		if (checkMark != null)
		{
			Stretch((RectTransform)checkMark);
			Image checkImage = checkMark.GetComponent<Image>();
			checkImage.sprite = checkSprite;
			checkImage.color = clearedColor;
			checkImage.raycastTarget = false;
		}

		StageSelectButton component = root.GetComponent<StageSelectButton>();
		SerializedObject serialized = new SerializedObject(component);
		serialized.FindProperty("stageSelectButton").objectReferenceValue = root;
		serialized.FindProperty("buttonGameObject").objectReferenceValue = button.gameObject;
		serialized.FindProperty("button").objectReferenceValue = button.GetComponent<Button>();
		serialized.FindProperty("buttonText").objectReferenceValue = buttonText;
		serialized.FindProperty("completeMarkGameObject").objectReferenceValue = completeMark.gameObject;
		ApplyCommonReferences(serialized, root, background);
		serialized.ApplyModifiedPropertiesWithoutUndo();

		PrefabUtility.SaveAsPrefabAsset(root, path);
		PrefabUtility.UnloadPrefabContents(root);
	}

	static void BuildBonusStageCardPrefab()
	{
		string path = prefabFolder + "Image_BonusStageSelectButton.prefab";
		GameObject root = PrefabUtility.LoadPrefabContents(path);

		Transform background = ArrangeCardCommon(root);
		Transform button = Find(root, "Button");
		Text buttonText = ReplaceButtonLabel(button);

		//元から入っていた飾りの絵は今のデザインに合わないので隠しておく
		Transform icon = Find(root, "Image_Icon");
		if (icon != null)
		{
			icon.gameObject.SetActive(false);
		}

		BonusStageSelectButton component = root.GetComponent<BonusStageSelectButton>();
		SerializedObject serialized = new SerializedObject(component);
		serialized.FindProperty("bonusStageSelectButton").objectReferenceValue = root;
		serialized.FindProperty("buttonGameObject").objectReferenceValue = button.gameObject;
		serialized.FindProperty("button").objectReferenceValue = button.GetComponent<Button>();
		serialized.FindProperty("buttonText").objectReferenceValue = buttonText;
		serialized.FindProperty("icon").objectReferenceValue = icon != null ? icon.gameObject : null;
		ApplyCommonReferences(serialized, root, background);
		serialized.ApplyModifiedPropertiesWithoutUndo();

		PrefabUtility.SaveAsPrefabAsset(root, path);
		PrefabUtility.UnloadPrefabContents(root);
	}

	/// <summary>
	/// 2種類のカードで同じ部分を組み立てる。ガラス板の位置を返す
	/// </summary>
	static Transform ArrangeCardCommon(GameObject root)
	{
		//前回この道具が作った文字を消しておく
		DestroyByName(root.transform, "Text_Number");
		DestroyByName(root.transform, "Text_Label");
		DestroyByName(root.transform, "Text_Status");

		//カード自体は透明。スクロールを掴む為の当たり判定として残しておく
		Image rootImage = root.GetComponent<Image>();
		rootImage.color = new Color(1, 1, 1, 0);
		rootImage.material = null;

		//次のステージへ伸びる縦棒。カードの上辺から上へ伸ばす
		Transform verticalBar = Find(root, "Image_VerticalBar");
		SetRect((RectTransform)verticalBar, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(0, -12), new Vector2(12, 124));
		Image barImage = verticalBar.GetComponent<Image>();
		barImage.sprite = barSprite;
		barImage.type = Image.Type.Sliced;
		barImage.color = new Color(1, 1, 1, 0.18f);
		barImage.raycastTarget = false;
		verticalBar.SetSiblingIndex(0);

		Transform gauge = Find(root, "Image_VerticalBarGauge");
		Stretch((RectTransform)gauge);
		Image gaugeImage = gauge.GetComponent<Image>();
		gaugeImage.sprite = barSprite;
		gaugeImage.type = Image.Type.Filled;
		gaugeImage.fillMethod = Image.FillMethod.Vertical;
		gaugeImage.fillOrigin = (int)Image.OriginVertical.Bottom;
		gaugeImage.color = accentColor;
		gaugeImage.raycastTarget = false;

		//元は枠線の子だったガラス板を、カードの直下へ移してカード全体に広げる
		Transform frameLine = Find(root, "Image_FrameLine");
		Transform background = Find(root, "Image_Background");
		background.SetParent(root.transform, false);
		Stretch((RectTransform)background);
		Image backgroundImage = background.GetComponent<Image>();
		backgroundImage.material = panelMaterial;
		backgroundImage.sprite = null;
		backgroundImage.type = Image.Type.Simple;
		backgroundImage.color = Color.white;
		backgroundImage.raycastTarget = false;
		AddLiquidGlassRect(background.gameObject, 28.0f);
		background.SetSiblingIndex(1);

		//枠線は下線に作り替える。状態によって色が変わる
		SetRect((RectTransform)frameLine, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 16), new Vector2(-140, 5));
		Image frameLineImage = frameLine.GetComponent<Image>();
		frameLineImage.material = null;
		frameLineImage.sprite = null;
		frameLineImage.type = Image.Type.Simple;
		frameLineImage.raycastTarget = false;
		frameLine.SetSiblingIndex(2);

		//番号は左に大きく置く。見出しと状態はその右に縦へ積むので、
		//スタートボタンが出る大きいカードでも重ならない
		Text textNumber = CreateText(root.transform, "Text_Number", "1", 88, new Color(1, 1, 1, 0.85f), TextAnchor.MiddleCenter,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -18), new Vector2(120, 120));
		textNumber.transform.SetSiblingIndex(3);

		Text textLabel = CreateText(root.transform, "Text_Label", "ステージ 1", 32, Color.white, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(162, -34), new Vector2(320, 44));
		textLabel.transform.SetSiblingIndex(4);

		Text textStatus = CreateText(root.transform, "Text_Status", "", 22, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(162, -80), new Vector2(320, 34));
		textStatus.transform.SetSiblingIndex(5);

		//スタートボタンはカードの下側に置く
		Transform button = Find(root, "Button");
		SetRect((RectTransform)button, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 40), new Vector2(360, 80));
		Image buttonImage = button.GetComponent<Image>();
		buttonImage.sprite = null;
		buttonImage.type = Image.Type.Simple;
		buttonImage.material = buttonMaterial;
		buttonImage.color = Color.white;
		AddLiquidGlassRect(button.gameObject, 24.0f);
		button.SetAsLastSibling();

		return background;
	}

	/// <summary>
	/// ボタンの中の文字を、日本語が出せるuGUIのTextに置き換える
	/// </summary>
	static Text ReplaceButtonLabel(Transform button)
	{
		Transform oldLabel = button.Find("Text (TMP)");
		if (oldLabel != null)
		{
			Object.DestroyImmediate(oldLabel.gameObject);
		}

		Transform existing = button.Find("Text_ButtonLabel");
		if (existing != null)
		{
			Object.DestroyImmediate(existing.gameObject);
		}

		Text label = CreateText(button, "Text_ButtonLabel", "▶ スタート", 30, Color.white, TextAnchor.MiddleCenter,
			new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
		Stretch(label.rectTransform);

		return label;
	}

	static void ApplyCommonReferences(SerializedObject serialized, GameObject root, Transform background)
	{
		serialized.FindProperty("verticalBar").objectReferenceValue = Find(root, "Image_VerticalBar").gameObject;
		serialized.FindProperty("verticalBarGauge").objectReferenceValue = Find(root, "Image_VerticalBarGauge").GetComponent<Image>();
		serialized.FindProperty("frameLine").objectReferenceValue = Find(root, "Image_FrameLine").GetComponent<Image>();
		serialized.FindProperty("background").objectReferenceValue = background.GetComponent<Image>();
		serialized.FindProperty("textLabel").objectReferenceValue = Find(root, "Text_Label").GetComponent<Text>();
		serialized.FindProperty("textStatus").objectReferenceValue = Find(root, "Text_Status").GetComponent<Text>();
		serialized.FindProperty("textNumber").objectReferenceValue = Find(root, "Text_Number").GetComponent<Text>();
	}

	// ------------------------------------------------------------
	// シーン
	// ------------------------------------------------------------

	static void BuildScene()
	{
		if (EditorSceneManager.GetActiveScene().path != scenePath)
		{
			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
		}

		Canvas canvas = Object.FindObjectOfType<Canvas>();
		Transform canvasTransform = canvas.transform;

		//横に長い端末だと横幅基準では縦が足りなくなり、上下の要素が重なってしまうので高さ基準にする
		CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
		if (scaler != null)
		{
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920, 1080);
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			scaler.matchWidthOrHeight = 1.0f;
		}

		//前回この道具が作った物を消す
		DestroyByName(canvasTransform, "Image_Background");
		DestroyByName(canvasTransform, "Text_LogoSmall");
		DestroyByName(canvasTransform, "Text_ScreenTitle");
		DestroyByName(canvasTransform, "Line_Header");
		DestroyByName(canvasTransform, "Panel_Progress");
		DestroyByName(canvasTransform, "Text_StageSubName");
		DestroyByName(canvasTransform, "Image_AccentBar");
		DestroyByName(canvasTransform, "Text_StageName");
		DestroyByName(canvasTransform, "Text_Description");
		DestroyByName(canvasTransform, "Panel_StageInfo");
		DestroyByName(canvasTransform, "Line_Bottom");
		DestroyByName(canvasTransform, "Text_Hint");
		DestroyByName(canvasTransform, "Text_Version");

		//背景。画面全体をゆっくり動く水色で塗る
		Image background = CreateImage(canvasTransform, "Image_Background", backgroundMaterial, Color.white);
		Stretch(background.rectTransform);
		background.raycastTarget = false;
		background.transform.SetAsFirstSibling();

		//見出し
		CreateText(canvasTransform, "Text_LogoSmall", "NOTES SHOOTER", 26, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -40), new Vector2(700, 40));
		CreateText(canvasTransform, "Text_ScreenTitle", "ステージセレクト", 62, Color.white, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -80), new Vector2(900, 86));

		Image headerLine = CreateImage(canvasTransform, "Line_Header", null, new Color(1, 1, 1, 0.4f));
		SetRect(headerLine.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -186), new Vector2(-120, 2));
		headerLine.raycastTarget = false;

		//右上の、今が何ステージ目かの表示
		Image progressPanel = CreateImage(canvasTransform, "Panel_Progress", panelMaterial, Color.white);
		SetRect(progressPanel.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-60, -44), new Vector2(420, 96));
		progressPanel.raycastTarget = false;
		AddLiquidGlassRect(progressPanel.gameObject, 26.0f);
		CreateText(progressPanel.transform, "Text_ProgressLabel", "ステージ", 24, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(28, 0), new Vector2(220, 40));
		Text textProgress = CreateText(progressPanel.transform, "Text_Progress", "1 / 4", 40, Color.white, TextAnchor.MiddleRight,
			new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-28, 0), new Vector2(240, 56));

		//左側の、今遊べるステージの紹介
		Text textStageSubName = CreateText(canvasTransform, "Text_StageSubName", "STAGE 01", 30, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -260), new Vector2(700, 44));

		Image accentBar = CreateImage(canvasTransform, "Image_AccentBar", null, accentColor);
		SetRect(accentBar.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -310), new Vector2(6, 100));
		accentBar.raycastTarget = false;

		Text textStageName = CreateText(canvasTransform, "Text_StageName", "ステージ 1", 68, Color.white, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(92, -310), new Vector2(900, 100));

		Text textDescription = CreateText(canvasTransform, "Text_Description", "", 30, softTextColor, TextAnchor.UpperLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -430), new Vector2(820, 140));
		textDescription.horizontalOverflow = HorizontalWrapMode.Wrap;

		//左下のステージ情報
		Image infoPanel = CreateImage(canvasTransform, "Panel_StageInfo", panelMaterial, Color.white);
		SetRect(infoPanel.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(64, 190), new Vector2(460, 300));
		infoPanel.raycastTarget = false;
		AddLiquidGlassRect(infoPanel.gameObject, 30.0f);

		CreateText(infoPanel.transform, "Text_InfoTitle", "ステージ情報", 24, accentColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(28, -22), new Vector2(400, 36));
		Image infoLine = CreateImage(infoPanel.transform, "Line_Info", null, new Color(1, 1, 1, 0.35f));
		SetRect(infoLine.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(28, -64), new Vector2(404, 2));
		infoLine.raycastTarget = false;

		Text textInfoHighScore = CreateInfoRow(infoPanel.transform, "HighScore", "ハイスコア", "0", -86);
		Text textInfoState = CreateInfoRow(infoPanel.transform, "State", "じょうたい", "これから", -152);
		Text textInfoDifficulty = CreateInfoRow(infoPanel.transform, "Difficulty", "なんいど", "★☆☆", -218);

		//縦スクロールの一覧は右側へ寄せる。横幅は画面に合わせて伸び縮みさせる
		ScrollRect scrollRect = Object.FindObjectOfType<ScrollRect>();
		SetRect((RectTransform)scrollRect.transform, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f),
			new Vector2(450, 10), new Vector2(-980, 660));
		Image scrollBackground = scrollRect.GetComponent<Image>();
		if (scrollBackground != null)
		{
			scrollBackground.material = null;
			scrollBackground.sprite = null;
			scrollBackground.color = new Color(1, 1, 1, 0.05f);
		}

		//切り抜き用の画像そのものは描かない
		Mask viewportMask = scrollRect.GetComponentInChildren<Mask>(true);
		if (viewportMask != null)
		{
			viewportMask.showMaskGraphic = false;
		}

		StyleScrollbar(scrollRect);

		//下側
		Image bottomLine = CreateImage(canvasTransform, "Line_Bottom", null, new Color(1, 1, 1, 0.45f));
		SetRect(bottomLine.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 150), new Vector2(-120, 2));
		bottomLine.raycastTarget = false;

		CreateText(canvasTransform, "Text_Hint", "光っているステージのスタートを押すとはじまります", 28, softTextColor, TextAnchor.MiddleLeft,
			new Vector2(0, 0), new Vector2(0, 0), new Vector2(64, 54), new Vector2(1100, 48));

		Text textVersion = CreateText(canvasTransform, "Text_Version", "v 0.1", 22, softTextColor, TextAnchor.MiddleRight,
			new Vector2(1, 0), new Vector2(1, 0), new Vector2(-60, 164), new Vector2(400, 34));

		//タイトルへ戻るボタン。タイトル画面の「ゲーム終了」と同じ位置に揃える
		Image backImage = canvasTransform.Find("Button_BackToTitle").GetComponent<Image>();
		SetRect(backImage.rectTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-60, 42), new Vector2(260, 84));
		backImage.material = buttonMaterial;
		backImage.sprite = null;
		backImage.type = Image.Type.Simple;
		backImage.color = Color.white;
		AddLiquidGlassRect(backImage.gameObject, 30.0f);

		Text textBack = backImage.transform.Find("Text_Back").GetComponent<Text>();
		textBack.font = font;
		textBack.fontSize = 30;
		textBack.color = Color.white;
		textBack.alignment = TextAnchor.MiddleCenter;
		textBack.raycastTarget = false;

		//フェード用の黒画像は必ず一番手前に置く。押せてしまうと下のボタンが反応しなくなるので当たり判定は切る
		Image fadeImage = null;
		Transform fadeTransform = canvasTransform.Find("Image_FadeStageSelect");
		if (fadeTransform != null)
		{
			fadeImage = fadeTransform.GetComponent<Image>();
			fadeImage.raycastTarget = false;
			fadeTransform.SetAsLastSibling();
		}

		//管理役へ参照を渡す
		StageSelectManager manager = Object.FindObjectOfType<StageSelectManager>();
		SerializedObject serialized = new SerializedObject(manager);
		serialized.FindProperty("fadeImage").objectReferenceValue = fadeImage;
		serialized.FindProperty("textStageName").objectReferenceValue = textStageName;
		serialized.FindProperty("textStageSubName").objectReferenceValue = textStageSubName;
		serialized.FindProperty("textDescription").objectReferenceValue = textDescription;
		serialized.FindProperty("textProgress").objectReferenceValue = textProgress;
		serialized.FindProperty("textInfoHighScore").objectReferenceValue = textInfoHighScore;
		serialized.FindProperty("textInfoState").objectReferenceValue = textInfoState;
		serialized.FindProperty("textInfoDifficulty").objectReferenceValue = textInfoDifficulty;
		serialized.FindProperty("textVersion").objectReferenceValue = textVersion;
		ApplyStageInformations(serialized);
		serialized.ApplyModifiedPropertiesWithoutUndo();

		//戻るボタンの押した時の処理を繋ぎ直す
		Button backButton = backImage.GetComponent<Button>();
		for (int i = backButton.onClick.GetPersistentEventCount() - 1; 0 <= i; i--)
		{
			UnityEventTools.RemovePersistentListener(backButton.onClick, i);
		}
		UnityEventTools.AddPersistentListener(backButton.onClick, new UnityAction(manager.RequestTitle));

		//背景を全面に描くので、カメラは空を描かなくて良い
		Camera camera = Camera.main;
		if (camera != null)
		{
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = new Color(0.055f, 0.271f, 0.408f, 1.0f);
		}

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		EditorSceneManager.SaveOpenScenes();
		Debug.Log("ステージセレクトのUIを組み直しました");
	}

	static void ApplyStageInformations(SerializedObject serialized)
	{
		string[] stageNames = { "ステージ 1", "ステージ 2", "ステージ 3", "ボーナスステージ" };
		string[] subNames = { "STAGE 01", "STAGE 02", "STAGE 03", "BONUS STAGE" };
		string[] difficulties = { "★☆☆", "★★☆", "★★★", "★★★" };
		string[] descriptions =
		{
			"まずはここから。リズムに合わせて的を撃ちぬこう。",
			"的の動きが速くなる。ねらいを定めて素早く撃とう。",
			"的が入り乱れる終盤。集中力が試されるステージ。",
			"的をすべて撃ちぬいてスコアを稼ごう。",
		};

		SerializedProperty informations = serialized.FindProperty("stageInformations");
		informations.arraySize = stageNames.Length;
		for (int i = 0; i < stageNames.Length; i++)
		{
			SerializedProperty element = informations.GetArrayElementAtIndex(i);
			element.FindPropertyRelative("stageName").stringValue = stageNames[i];
			element.FindPropertyRelative("subName").stringValue = subNames[i];
			element.FindPropertyRelative("description").stringValue = descriptions[i];
			element.FindPropertyRelative("difficulty").stringValue = difficulties[i];
		}
	}

	static void StyleScrollbar(ScrollRect scrollRect)
	{
		Scrollbar scrollbar = scrollRect.verticalScrollbar;
		if (scrollbar == null)
		{
			return;
		}

		RectTransform rectTransform = (RectTransform)scrollbar.transform;
		rectTransform.sizeDelta = new Vector2(8, rectTransform.sizeDelta.y);

		Image scrollbarImage = scrollbar.GetComponent<Image>();
		if (scrollbarImage != null)
		{
			scrollbarImage.sprite = barSprite;
			scrollbarImage.type = Image.Type.Sliced;
			scrollbarImage.color = new Color(1, 1, 1, 0.10f);
		}

		Image handle = scrollbar.handleRect != null ? scrollbar.handleRect.GetComponent<Image>() : null;
		if (handle != null)
		{
			handle.sprite = barSprite;
			handle.type = Image.Type.Sliced;
			handle.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0.7f);
		}
	}

	// ------------------------------------------------------------
	// 部品づくり
	// ------------------------------------------------------------

	static Text CreateInfoRow(Transform parent, string name, string label, string value, float y)
	{
		CreateText(parent, "Text_Info" + name + "Label", label, 24, softTextColor, TextAnchor.MiddleLeft,
			new Vector2(0, 1), new Vector2(0, 1), new Vector2(28, y), new Vector2(240, 44));
		return CreateText(parent, "Text_Info" + name, value, 34, Color.white, TextAnchor.MiddleRight,
			new Vector2(1, 1), new Vector2(1, 1), new Vector2(-28, y + 2), new Vector2(300, 48));
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

	static Transform Find(GameObject root, string name)
	{
		foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
		{
			if (child.name == name)
			{
				return child;
			}
		}

		return null;
	}

	static void DestroyByName(Transform parent, string name)
	{
		Transform target = parent.Find(name);
		while (target != null)
		{
			Object.DestroyImmediate(target.gameObject);
			target = parent.Find(name);
		}
	}
}
