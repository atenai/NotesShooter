using UnityEngine;

public class RightBullet : MonoBehaviour
{
    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("RedTarget") || hit.CompareTag("PurpleTarget") || hit.CompareTag("Wall") || hit.CompareTag("Drum"))
        {
            //Debug.Log("RightBulletに当たったよ");

            Destroy(gameObject);
        }
    }
}
