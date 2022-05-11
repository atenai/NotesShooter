using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true, 60);//解像度の設定
        Application.targetFrameRate = 60;//フレームレートの設定
    }

    // Update is called once per frame
    void Update()
    {

        //Cキーでマウスカーソルを出す
        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //Escapeキーでゲーム終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();//ゲーム終了
        }

        //タイトルシーンへ
        if (Input.GetKey(KeyCode.T))
        {
            SceneManager.LoadScene("Title");
        }
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
