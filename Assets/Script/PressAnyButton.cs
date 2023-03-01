using UnityEngine;
using UnityEngine.UI;

public class PressAnyButton : MonoBehaviour
{
    bool isStart = false;

    Text PressAnyKey_text;

    float alfa = 0.0f;//テキスト・α値
    const float max = 1.0f;
    const float min = 0.0f;
    bool isAlfa = false;
    [SerializeField] float plas_alfa = 0.015f;

    void Start()
    {
        isStart = false;

        //Textコンポーネント取得
        PressAnyKey_text = GameObject.Find("Press Any Key").GetComponent<Text>();

        //テキストカラー初期化
        PressAnyKey_text.color = new Color(0.0f, 255.0f, 255.0f, alfa);

        plas_alfa = 0.015f;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            isStart = true;
        }

        if (isStart == false)
        {
            PressAnyKey_text.text = "Press Any Button";
        }
        else
        {
            PressAnyKey_text.text = "";
        }

        if (alfa >= max)
        {
            isAlfa = true;
        }
        if (alfa <= min)
        {
            isAlfa = false;
        }

        if (isAlfa == true)
        {
            alfa -= plas_alfa;
        }
        else
        {
            alfa += plas_alfa;
        }

        PressAnyKey_text.color = new Color(0.0f, 255.0f, 255.0f, alfa);
    }
}
