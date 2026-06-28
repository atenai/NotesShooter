using UnityEngine;

/// <summary>
/// 基底クラス
/// 派生クラスのリスト(BlueGun・RedGun)
/// </summary>
public interface IGun
{
	public int CurrentBullet { get; }
	public bool IsReloadTrigger { get; }
	void ShotSystem(GameObject gunObject, GameObject shootPoint, GameObject cartridgePoint);
	void AutoReloadTrigger();
	void ManualReloadTrigger();
	void ReloadSystem(GameObject gunObject);
}
