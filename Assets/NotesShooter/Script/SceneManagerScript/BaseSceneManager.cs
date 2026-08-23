using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(FadeTitle・FadeGamePlay・FadeResult)
/// </summary>
public class BaseSceneManager : MonoBehaviour
{
	[SerializeField] protected Image image;
	[SerializeField] protected AudioClip audioClip;
	[SerializeField] protected AudioSource audioSource;
	[SerializeField] protected string sceneName = "";

	protected float alfa = 0.0f;
	protected float speed = 2.5f;
	protected bool isFade = false;

	[Tooltip("フェードインで薄くしていく黒画像の濃さ")]
	float fadeInAlfa = 1.0f;
	[Tooltip("フェードインが終わったか")]
	protected bool isFadeInEnd = false;

	protected void Start()
	{
		alfa = 0.0f;
		isFade = false;

		//シーンの開始時は真っ黒にしておき、FadeInで少しずつ透明にしていく
		fadeInAlfa = 1.0f;
		isFadeInEnd = false;
		image.color = new Color(image.color.r, image.color.g, image.color.b, fadeInAlfa);
	}

	/// <summary>
	/// シーン開始時のフェードイン。真っ黒の状態から少しずつ透明にする
	/// </summary>
	protected void FadeIn()
	{
		if (isFadeInEnd == true)
		{
			return;
		}

		fadeInAlfa -= speed * Time.deltaTime;

		const float min = 0.0f;
		if (fadeInAlfa <= min)
		{
			fadeInAlfa = min;
			isFadeInEnd = true;
		}

		image.color = new Color(image.color.r, image.color.g, image.color.b, fadeInAlfa);
	}

	protected void FadeTrigger()
	{
		if (Input.anyKeyDown && isFade == false)
		{
			isFade = true;
			audioSource.PlayOneShot(audioClip);
		}
	}

	protected void FadeOut()
	{
		if (isFade == true)
		{
			image.color = new Color(image.color.r, image.color.g, image.color.b, alfa);
			alfa += speed * Time.deltaTime;
		}

		const float max = 1.0f;
		if (max <= alfa)
		{
			isFade = false;
			SceneChange(sceneName);
		}
	}

	protected void SceneChange(string name)
	{
		SceneManager.LoadScene(name);
	}
}
