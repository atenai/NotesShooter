using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeTitle : Fade
{
    //サウンド
    public AudioClip StartSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();

        alfa = 0.0f;
        speed = 0.025f;
        b_Fade = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && b_Fade == false)
        {
            //SE再生
            //音(GOALSound)を鳴らす
            audioSource.PlayOneShot(StartSound);

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
            SceneManager.LoadScene("MasterStage");
            b_Fade = false;
        }
    }
}
