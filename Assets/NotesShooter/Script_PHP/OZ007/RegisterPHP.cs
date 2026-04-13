using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegisterPHP : MonoBehaviour
{
    [SerializeField] InputField userNameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Button registerButton;

    void Start()
    {
        registerButton.onClick.AddListener(() =>
        {
            StartCoroutine(RegisterUser(userNameInput.text, passwordInput.text));
        });
    }

    IEnumerator RegisterUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post($"http://localhost/OZ007/RegisterUser.php", form))
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

            Debug.Log("Received: " + www.downloadHandler.text);
        }
    }
}
