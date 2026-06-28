using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 右手の銃のUI
/// </summary>
public class RightGunUICommonView : MonoBehaviour
{
	[Tooltip("ヒットレティクル")]
	[SerializeField] Image hitReticule;
	[Tooltip("ヒットレティクルが消失するスピード")]
	float hitReticuleSpeed = 10.0f;
	[Tooltip("ヒットレティクルを表示するか？？")]
	bool isHitReticule = false;
	public bool IsHitReticule
	{
		get { return isHitReticule; }
		set { isHitReticule = value; }
	}
	[Tooltip("ヒットレティクルカラー")]
	[SerializeField] Color color;

	void Start()
	{

	}

	void Update()
	{
		UpdateHitReticule();
	}

	/// <summary>
	/// ヒットレティクル
	/// </summary> 
	void UpdateHitReticule()
	{
		if (isHitReticule == true)
		{
			hitReticule.color = color;
		}

		if (isHitReticule == false)
		{
			hitReticule.color = Color.Lerp(hitReticule.color, Color.clear, Time.deltaTime * hitReticuleSpeed);
		}

		isHitReticule = false;
	}
}
