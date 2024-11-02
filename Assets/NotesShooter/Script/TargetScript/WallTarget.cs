using UnityEngine;

/// <summary>
/// WallTarget（派生クラス）Target（基底クラス）
/// </summary>
public class WallTarget : Target
{
    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("BlueBullet") || hit.CompareTag("RedBullet") || hit.CompareTag("DrumCollider"))
        {

            //Debug.Log("壁に当たったよ");

            HitSE();

            Destroy(gameObject);
        }
    }
}
