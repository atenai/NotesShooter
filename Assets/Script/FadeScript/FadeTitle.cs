using UnityEngine;
using UnityEngine.UI;

public class FadeTitle : Fade
{
    [SerializeField] AudioClip StartSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Text textPressAnyKey;

    float textAlfa = 0.0f;
    bool isAlfa = false;
    float textSpeed = 1.0f;

    new void Start()
    {
        base.Start();
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

        PressAnyButton();
    }

    void PressAnyButton()
    {
        if (isFade == false)
        {
            textPressAnyKey.text = "Press Any Key";
        }
        else if (isFade == true)
        {
            textPressAnyKey.text = "";
        }

        const float max = 1.0f;
        if (max <= textAlfa)
        {
            isAlfa = true;
        }
        const float min = 0.0f;
        if (textAlfa <= min)
        {
            isAlfa = false;
        }

        if (isAlfa == true)
        {
            textAlfa -= textSpeed * Time.deltaTime;
        }
        else if (isAlfa == false)
        {
            textAlfa += textSpeed * Time.deltaTime;
        }

        textPressAnyKey.color = new Color(0.0f, 255.0f, 255.0f, textAlfa);
    }
}
