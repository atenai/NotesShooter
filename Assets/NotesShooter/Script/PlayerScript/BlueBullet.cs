using UnityEngine;

public class BlueBullet : MonoBehaviour
{
    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("BlueTarget") || hit.CompareTag("PurpleTarget") || hit.CompareTag("Wall") || hit.CompareTag("Drum"))
        {
            Destroy(gameObject);
        }
    }
}
