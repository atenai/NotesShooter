using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isGoal = false;

    void Start()
    {
        isGoal = false;
    }

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            isGoal = true;
        }
    }
}
