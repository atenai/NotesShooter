using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeTitle : Fade
{
    [SerializeField] AudioClip StartSound;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        alfa = 0.0f;
        speed = 0.025f;
        isFade = false;
    }

    void Update()
    {
        if (Input.anyKeyDown && isFade == false)
        {
            //SE再生
            audioSource.PlayOneShot(StartSound);
            isFade = true;
        }

        if (isFade == true)
        {
            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alfa);
            alfa += speed;
        }

        if (1 <= alfa)
        {
            SceneChange(sceneName);
            isFade = false;
        }
    }
}
