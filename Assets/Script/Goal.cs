using UnityEngine;

public class Goal : MonoBehaviour
{
    public static bool b_Goal = false;

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
