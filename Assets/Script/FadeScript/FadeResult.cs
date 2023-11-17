using UnityEngine;
using UnityEngine.UI;

public class FadeResult : Fade
{
    public AudioClip StartSound;
    AudioSource audioSource;

    new void Start()
    {
        base.Start();

        audioSource = this.GetComponent<AudioSource>();
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
            this.GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alfa);
            alfa += speed * Time.deltaTime;
        }

        if (1.0f <= alfa)
        {
            SceneChange(sceneName);
            isFade = false;
        }
    }
}
