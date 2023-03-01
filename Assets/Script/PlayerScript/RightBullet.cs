using UnityEngine;

public class RightBullet : MonoBehaviour
{
    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {
        //接触対象はRightTargetタグですか？
        if (hit.CompareTag("RightTarget") || hit.CompareTag("PurpleTarget") || hit.CompareTag("Wall") || hit.CompareTag("Drum"))
        {
            //Debug.Log("RightBulletに当たったよ");
            //何らかの処理
            //このコンポーネントを持つGameObjectを破棄する
            Destroy(gameObject);
        }
    }
}
