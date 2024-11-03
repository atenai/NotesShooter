using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(FadeTitle・FadeGamePlay・FadeResult)
/// </summary>
public class BaseSceneManager : MonoBehaviour
{
    [SerializeField] protected Image image;
    [SerializeField] protected AudioClip audioClip;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected string sceneName = "";

    protected float alfa = 0.0f;
    protected float speed = 2.5f;
    protected bool isFade = false;

    protected void Start()
    {
        alfa = 0.0f;
        isFade = false;
    }

    protected void FadeTrigger()
    {
        if (Input.anyKeyDown && isFade == false)
        {
            //SE再生
            audioSource.PlayOneShot(audioClip);
            isFade = true;
        }
    }

    protected void FadeOut()
    {
        if (isFade == true)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alfa);
            alfa += speed * Time.deltaTime;
        }

        if (1.0f <= alfa)
        {
            SceneChange(sceneName);
            isFade = false;
        }
    }

    protected void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
    }

}
