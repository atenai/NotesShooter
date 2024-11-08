using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSetting : MonoBehaviour
{
    [Tooltip("マウスカーソルのオン/オフ")]
    [SerializeField] bool isCursor = false;
    [Tooltip("フレームレート")]
    int frameCount;
    float prevTime;
    float fps = 0.0f;

    private void Awake()
    {
#if UNITY_ANDROID//端末がAndroidだった場合の処理

#endif //終了

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理

        Screen.SetResolution(1920, 1080, true, 60);
        Application.targetFrameRate = 60;//フレームレートの設定

#endif //終了

#if UNITY_STANDALONE_WIN//端末がPCだった場合の処理
        CursorActive();
#endif //終了

        //マウスカーソルを消す
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        UpdateFPS();

#if UNITY_EDITOR//Unityエディター上での処理
        //Tキーでマウスカーソルを出すorマウスカーソルを消す
        if (Input.GetKeyDown(KeyCode.T))
        {
            isCursor = isCursor ? false : true;
            CursorActive();
        }
#endif //終了   

#if UNITY_EDITOR || UNITY_STANDALONE_WIN//Unityエディター上または端末がPCだった場合の処理
        //Escapeキーでゲーム終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();//ゲーム終了
        }
#endif //終了  

#if UNITY_EDITOR
        //タイトルシーンへ
        if (Input.GetKey(KeyCode.Y))
        {
            SceneManager.LoadScene("Title");
        }
#endif
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
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (0.5f <= time)
        {
            fps = frameCount / time;

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
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

        GUI.Box(new Rect(10, 10, 100, 100), "フレームレート : ", styleGreen);
        GUI.Box(new Rect(250, 10, 100, 100), fps.ToString(), styleGreen);
#endif
    }
}
