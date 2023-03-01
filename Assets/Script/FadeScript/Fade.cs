using UnityEngine;
using UnityEngine.SceneManagement;

//基底クラス//
//派生クラスのリスト(FadeTitle・FadeGamePlay・FadeResult)
public class Fade : MonoBehaviour
{
    [SerializeField] protected float alfa = 0.0f;
    [SerializeField] protected float speed;

    [SerializeField] protected bool isFade = false;

    [SerializeField] protected string sceneName = "";

    protected void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
    }
}
