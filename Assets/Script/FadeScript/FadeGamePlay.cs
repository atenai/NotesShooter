using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeGamePlay : Fade
{
    // Start is called before the first frame update
    void Start()
    {
        alfa = 0.0f;
        speed = 0.01f;
        b_Fade = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Goal.b_Goal == true)
        {
            b_Fade = true;
        }

        if (b_Fade == true)
        {
            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alfa);
            alfa += speed;
        }

        if (alfa >= 1)
        {
            //ステージ１シーンへ
            SceneManager.LoadScene("Result");
            b_Fade = false;
        }
    }
}
