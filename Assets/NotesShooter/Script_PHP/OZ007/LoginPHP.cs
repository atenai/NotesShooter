using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine.SceneManagement;

public class LoginPHP : MonoBehaviour
{
    [SerializeField] InputField userNameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Button loginButton;
    [SerializeField] Text infoText;

    void Start()
    {
        loginButton.onClick.AddListener(() =>
        {
            StartCoroutine(LoginRoutine(userNameInput.text, passwordInput.text));
        });
    }

    IEnumerator LoginRoutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post($"http://localhost/OZ007/Login.php", form))
        {
            www.redirectLimit = 0;
            www.timeout = 10;

            yield return www.SendWebRequest();

            //Debug.Log("Request URL : " + url);
            //Debug.Log("Result      : " + www.result);
            //Debug.Log("ResponseCode: " + www.responseCode);
            //Debug.Log("Error       : " + www.error);

            //string location = www.GetResponseHeader("Location");
            //Debug.Log("Location    : " + location);

            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("リクエスト中");
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log("リクエスト成功");
                    break;

                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log(@"サーバとの通信に失敗。リクエストが接続できなかった、セキュリティで保護されたチャネルを確立できなかったなど。");
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(@"サーバがエラー応答を返した。サーバとの通信には成功したが、接続プロトコルで定義されているエラーを受け取った。");
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log(@"データの処理中にエラーが発生。リクエストはサーバとの通信に成功したが、受信したデータの処理中にエラーが発生。データが破損しているか、正しい形式ではないなど。");
                    break;
            }

            Debug.Log("受信: " + www.downloadHandler.text);

            // サーバーからの応答をトリムして、余分な空白を削除
            string resp = www.downloadHandler.text.Trim();
            bool registered = false;
            if (bool.TryParse(resp, out registered))
            {
                if (registered == true)
                {
                    infoText.text = "ログインに成功しました！";
                    infoText.color = Color.blue;
                    yield return new WaitForSeconds(1.0f); // 成功メッセージを表示するための短い遅延
                    SceneManager.LoadScene("Title");
                }
                else
                {
                    infoText.text = "ユーザー名またはパスワードが正しくありません！";
                    infoText.color = Color.red;
                }
            }
            else
            {
                infoText.text = "サーバーの応答が不正です。";
                infoText.color = Color.red;
                Debug.LogWarning("Unexpected response: " + resp);
            }
        }
    }
}
