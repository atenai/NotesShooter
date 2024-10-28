using UnityEngine;

public class FadeGamePlay : Fade
{
    new void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (Goal.singletonInstance.IsGoal == true)
        {
            isFade = true;
        }

        FadeOut();
    }
}
