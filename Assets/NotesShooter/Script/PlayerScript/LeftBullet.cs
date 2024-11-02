using UnityEngine;

public class LeftBullet : MonoBehaviour
{
    //トリガーとの接触時に呼ばれるコールバック
    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("BlueTarget") || hit.CompareTag("PurpleTarget") || hit.CompareTag("Wall") || hit.CompareTag("Drum"))
        {
            //Debug.Log("LeftBulletに当たったよ");

            Destroy(gameObject);
        }
    }
}
