using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginPHP : MonoBehaviour
{
    [SerializeField] InputField userNameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Button loginButton;


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

            if (www.result != UnityWebRequest.Result.Success)
            {
                yield break;
            }

            Debug.Log("受信: " + www.downloadHandler.text);
        }
    }
}
