using UnityEngine;

//基底クラス//
//派生クラスのリスト(FadeTitle・FadeGamePlay・FadeResult)
public class Fade : MonoBehaviour
{
    public float alfa = 0.0f;
    public float speed;

    public bool isFade = false;
}
