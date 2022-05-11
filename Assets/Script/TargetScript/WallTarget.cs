using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WallTarget（派生クラス）Target（基底クラス）
public class WallTarget : Target
{
    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {
        //接触対象はRightBulletまたはLeftBulletタグですか？
        if (hit.CompareTag("Bullet") || hit.CompareTag("RightBullet") || hit.CompareTag("DrumCollider") || hit.CompareTag("LeftBullet"))
        {

            //Debug.Log("壁に当たったよ");

            HitSE();

            Destroy(gameObject);
        }
    }
}
