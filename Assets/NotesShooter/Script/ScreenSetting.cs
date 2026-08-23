using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSetting : MonoBehaviour
{
	[Tooltip("マウスカーソルのオン/オフ")]
	[SerializeField] bool isCursor = false;
	[Tooltip("FPSのオン/オフ")]
	[SerializeField] bool isFPS = false;
	//PCビルドでは参照しないので「未使用」の警告が出る。設定値はインスペクターで保持したいので警告だけ止める
#pragma warning disable 0414
	[Tooltip("Androidでの最大フレームレート。端末のリフレッシュレートとこの値の小さい方が使われる(発熱や電池が気になる場合は60に下げる)")]
	[Range(60, 120)][SerializeField] int androidMaxFrameRate = 120;
#pragma warning restore 0414
	[Tooltip("フレームレート")]
	int frameCount;
	float prevTime;
	float fps = 0.0f;

	private void Awake()
	{
#if UNITY_ANDROID//端末がAndroidだった場合の処理

		//モバイルではvSyncCountが無視され、targetFrameRateの既定が30になるため明示的に指定する
		//端末のリフレッシュレート(90Hzや120Hzの端末がある)に合わせるとカメラの動きが更に滑らかになる
		int refreshRate = (int)Screen.currentResolution.refreshRateRatio.value;
		Application.targetFrameRate = Mathf.Clamp(refreshRate, 60, androidMaxFrameRate);

		//GPUが描画に追いつかない時にフレームが無制限に溜まり、その分のGPUメモリを確保し続けて最後にメモリ不足で落ちる事があるので、溜められるフレーム数に上限を付ける
		QualitySettings.maxQueuedFrames = 2;

#endif //終了

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理

		Screen.SetResolution(1920, 1080, true, 60);
		Application.targetFrameRate = 60;//フレームレートの設定

		CursorActive();

#endif //終了
	}

	void Start()
	{
		StartFPS();
	}

	/// <summary>
	/// フレームレートの初期化処理
	/// </summary> 
	void StartFPS()
	{
		frameCount = 0;
		prevTime = 0.0f;
	}

	void Update()
	{
#if UNITY_EDITOR//Unityエディター上での処理

		//Tキーでマウスカーソルを出すorマウスカーソルを消す
		if (Input.GetKeyDown(KeyCode.T))
		{
			isCursor = isCursor ? false : true;
		}
		CursorActive();

		//タイトルシーンへ
		if (Input.GetKey(KeyCode.Y))
		{
			SceneManager.LoadScene("Title");
		}

		//UキーでFPSを出すorFPSを消す
		if (Input.GetKeyDown(KeyCode.U))
		{
			isFPS = isFPS ? false : true;
		}
		UpdateFPS();

#endif //終了   

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
		//Escapeキーでゲーム終了
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Quit();
		}
#endif //終了  
	}

	/// <summary>
	/// マウスカーソルのオン/オフ処理 
	/// </summary>
	void CursorActive()
	{
		if (isCursor == false)
		{
			//マウスカーソルを消す
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		else if (isCursor == true)
		{
			//マウスカーソルを出す
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	/// <summary>
	/// フレームレート計算
	/// </summary>
	void UpdateFPS()
	{
		if (isFPS == true)
		{
			++frameCount;
			float time = Time.realtimeSinceStartup - prevTime;

			if (0.5f <= time)
			{
				fps = frameCount / time;

				frameCount = 0;
				prevTime = Time.realtimeSinceStartup;
			}
		}
	}

	/// <summary>
	/// ゲーム終了
	/// </summary> 
	void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
	}

	void OnGUI()
	{
#if UNITY_EDITOR
		GUIStyle styleGreen = new GUIStyle();
		styleGreen.fontSize = 30;
		GUIStyleState styleStateGreen = new GUIStyleState();
		styleStateGreen.textColor = Color.green;
		styleGreen.normal = styleStateGreen;

		if (isFPS == true)
		{
			GUI.Box(new Rect(10, 10, 100, 100), "フレームレート : ", styleGreen);
			GUI.Box(new Rect(250, 10, 100, 100), fps.ToString(), styleGreen);
		}

#endif
	}
}
