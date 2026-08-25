using UnityEngine;

public class ResultManager : BaseSceneManager
{
    new void Start()
    {
        base.Start();

        //リザルトに来るたびに数え、規定回数ごとにインターステーシャル広告を出す
        if (AdsManager.SingletonInstance != null)
        {
            AdsManager.SingletonInstance.ShowAdsInterstitialCount();
        }
    }

    void Update()
    {
        FadeIn();
        FadeTrigger();

        FadeOut();
    }
}
