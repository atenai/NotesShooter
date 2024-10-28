using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSetting : MonoBehaviour
{
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
    }

    void Update()
    {
        //Escapeキーでゲーム終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();//ゲーム終了
        }

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

    //ゲーム終了
    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }
}
