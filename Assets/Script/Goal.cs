using UnityEngine;

public class Goal : MonoBehaviour
{
    bool isGoal = false;
    public bool IsGoal => isGoal;


    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            isGoal = true;
        }
    }
}
