using UnityEngine;
using UnityEngine.UI;

public class TitleManager : BaseSceneManager
{
	[SerializeField] Text textPressAnyKey;

	float textAlfa = 0.0f;
	bool isAlfa = false;
	float textSpeed = 1.0f;

	void Update()
	{
		FadeTrigger();
		FadeOut();
		PressAnyButton();
	}

	void PressAnyButton()
	{
		string text = (isFade == true) ? textPressAnyKey.text = "" : textPressAnyKey.text = "Press Any Key";

		const float max = 1.0f;
		if (max <= textAlfa)
		{
			isAlfa = true;
		}

		const float min = 0.0f;
		if (textAlfa <= min)
		{
			isAlfa = false;
		}

		var result = (isAlfa == true) ? textAlfa -= textSpeed * Time.deltaTime : textAlfa += textSpeed * Time.deltaTime;

		textPressAnyKey.color = new Color(0.0f, 255.0f, 255.0f, textAlfa);
	}
}
