using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public static bool b_Goal = false;

    // Start is called before the first frame update
    void Start()
    {
        b_Goal = false;
    }

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            b_Goal = true;
        }
    }
}
