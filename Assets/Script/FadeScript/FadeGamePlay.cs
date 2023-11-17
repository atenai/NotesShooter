using UnityEngine;
using UnityEngine.UI;

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

        if (isFade == true)
        {
            this.GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alfa);
            alfa += speed * Time.deltaTime;
        }

        if (1.0f <= alfa)
        {
            SceneChange(sceneName);
            isFade = false;
        }
    }
}
