using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// PHPからデータを取得するテストスクリプト
/// </summary>
public class TestPHP : MonoBehaviour
{
    //string url = "http://localhost/OZ007/GetDate.php";
    //string url = "http://127.0.0.1/OZ007/GetDate.php";//127.0.0.1 はその端末自身を指しすIPアドレス
    string url = "http://192.168.1.2/OZ007/GetDate.php";//XAMPPを動かしているPCのローカルIPアドレス

    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.redirectLimit = 0;
            www.timeout = 10;

            yield return www.SendWebRequest();

            Debug.Log("Request URL : " + url);
            Debug.Log("Result      : " + www.result);
            Debug.Log("ResponseCode: " + www.responseCode);
            Debug.Log("Error       : " + www.error);

            string location = www.GetResponseHeader("Location");
            Debug.Log("Location    : " + location);

            if (www.result != UnityWebRequest.Result.Success)
            {
                yield break;
            }

            Debug.Log("Received: " + www.downloadHandler.text);
        }
    }
}