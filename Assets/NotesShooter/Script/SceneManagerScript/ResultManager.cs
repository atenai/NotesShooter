using UnityEngine;

public class ResultManager : BaseSceneManager
{
	[Tooltip("「タイトルへ」から飛ぶシーン名")]
	[SerializeField] string titleSceneName = "Title";
	[Tooltip("「ステージセレクト」から飛ぶシーン名")]
	[SerializeField] string stageSelectSceneName = "StageSelect";

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

		//画面のどこを触っても進む処理は使わない。
		//Input.anyKeyDownはボタンを押した時にも反応してしまい、
		//どのボタンを押してもFadeTriggerが先にタイトルへ行き先を決めてしまう
		FadeOut();
	}

	/// <summary>
	/// 「タイトルへ」ボタンから呼ばれる
	/// </summary>
	public void RequestTitle()
	{
		RequestSceneChange(titleSceneName);
	}

	/// <summary>
	/// 「ステージセレクト」ボタンから呼ばれる
	/// </summary>
	public void RequestStageSelect()
	{
		RequestSceneChange(stageSelectSceneName);
	}

	/// <summary>
	/// 行き先を決めてフェードアウトを始める
	/// </summary>
	void RequestSceneChange(string name)
	{
		//連打で二重に遷移しないようにする
		if (isFade == true || string.IsNullOrEmpty(name) == true)
		{
			return;
		}

		//基底クラスのFadeOutはsceneNameを見て移るので、押されたボタンの行き先を入れておく
		sceneName = name;
		isFade = true;

		if (audioSource != null && audioClip != null)
		{
			audioSource.PlayOneShot(audioClip);
		}
	}
}
