using UnityEngine;

public class RedBullet : MonoBehaviour
{
    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("RedTarget") || hit.CompareTag("PurpleTarget") || hit.CompareTag("Wall") || hit.CompareTag("Drum"))
        {
            Destroy(gameObject);
        }
    }
}
