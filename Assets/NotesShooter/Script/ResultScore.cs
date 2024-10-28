using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    [SerializeField] Text scoreText;

    void Start()
    {
        scoreText.text = PlayerPrefs.GetInt("SCORE", 0).ToString();
    }
}
