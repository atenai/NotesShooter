using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeGamePlay : Fade
{
    void Start()
    {
        alfa = 0.0f;
        speed = 0.01f;
        isFade = false;
    }

    void Update()
    {
        if (Goal.b_Goal == true)
        {
            isFade = true;
        }

        if (isFade == true)
        {
            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alfa);
            alfa += speed;
        }

        if (alfa >= 1)
        {
            //ステージ１シーンへ
            SceneManager.LoadScene("Result");
            isFade = false;
        }
    }
}
