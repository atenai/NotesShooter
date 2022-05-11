using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//基底クラス//
//派生クラスのリスト(FadeTitle・FadeGamePlay・FadeResult)
public class Fade : MonoBehaviour
{
    public float alfa = 0.0f;
    public float speed;

    public bool b_Fade = false;
}
