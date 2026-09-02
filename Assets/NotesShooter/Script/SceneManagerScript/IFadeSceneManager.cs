/// <summary>
/// フェードイン・フェードアウトを行うシーンマネージャーのインターフェース
/// </summary>
public interface IFadeSceneManager
{
	/// <summary>
	/// フェードイン・フェードアウトの初期化処理
	/// </summary>
	public void InitFade();
	/// <summary>
	/// 真っ黒な状態から少しずつ透明にする
	/// </summary>
	public void FadeIn();
	/// <summary>
	/// 透明な状態から少しずつ真っ黒にする
	/// </summary>
	public void FadeOut();
	/// <summary>
	/// シーンを切り替える
	/// </summary>
	/// <param name="name"></param>
	public void SceneChange(string name);
}
