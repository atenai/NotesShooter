using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(FadeTitle・FadeGamePlay・FadeResult)
/// </summary>
public class Fade : MonoBehaviour
{
    protected float alfa = 0.0f;
    protected float speed = 2.5f;
    protected bool isFade = false;

    protected void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
    }

    protected void Start()
    {
        alfa = 0.0f;
        isFade = false;
    }
}
