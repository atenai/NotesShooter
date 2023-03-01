using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeResult : Fade
{
    //サウンド
    public AudioClip StartSound;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        alfa = 0.0f;
        speed = 0.025f;
        isFade = false;
    }

    void Update()
    {
        if (Input.anyKeyDown && isFade == false)
        {
            //SE再生
            //音(GOALSound)を鳴らす
            audioSource.PlayOneShot(StartSound);

            isFade = true;
        }

        if (isFade == true)
        {
            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alfa);
            alfa += speed;
        }

        if (alfa >= 1)
        {
            SceneManager.LoadScene("Title");
            isFade = false;
        }
    }
}
