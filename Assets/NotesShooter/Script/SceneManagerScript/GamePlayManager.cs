using UnityEngine;

public class GamePlayManager : BaseSceneManager
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
