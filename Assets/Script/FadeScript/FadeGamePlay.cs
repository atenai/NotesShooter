using UnityEngine;

public class FadeGamePlay : Fade
{
    Goal goal;

    new void Start()
    {
        base.Start();

        goal = GameObject.Find("Goal").GetComponent<Goal>();
    }

    void Update()
    {
        if (goal.IsGoal == true)
        {
            isFade = true;
        }

        FadeOut();
    }
}
