using UnityEngine;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(BlueGun・RedGun)
/// </summary>
public interface IGun
{
	public int CurrentBullet { get; }
	public bool IsReloadTime { get; }
	void ShotSystem(GameObject gunObject);
	void AutoReloadTrigger();
	void ManualReloadTrigger();
	void ReloadSystem(GameObject gunObject);
}
