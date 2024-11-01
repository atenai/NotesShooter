using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSetting : MonoBehaviour
{
    int frameCount;
    float prevTime;
    float fps = 0.0f;

    private void Awake()
    {
        //マウスカーソルを消す
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        Screen.SetResolution(1920, 1080, true, 60);//解像度の設定
        Application.targetFrameRate = 60;//フレームレートの設定

        StartFPS();
    }

    void StartFPS()
    {
        frameCount = 0;
        prevTime = 0.0f;
    }

    void Update()
    {
        //Escapeキーでゲーム終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();//ゲーム終了
        }

        UpdateFPS();

#if UNITY_EDITOR
        //Cキーでマウスカーソルを出す
        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //タイトルシーンへ
        if (Input.GetKey(KeyCode.T))
        {
            SceneManager.LoadScene("Title");
        }
#endif
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
        GUIStyle styleGreen = new GUIStyle();
        styleGreen.fontSize = 30;
        GUIStyleState styleStateGreen = new GUIStyleState();
        styleStateGreen.textColor = Color.green;
        styleGreen.normal = styleStateGreen;

        GUI.Box(new Rect(10, 10, 100, 100), "フレームレート : ", styleGreen);
        GUI.Box(new Rect(250, 10, 100, 100), fps.ToString(), styleGreen);
    }
}
