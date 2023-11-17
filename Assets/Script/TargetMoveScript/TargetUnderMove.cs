using UnityEngine;

public class TargetUnderMove : TargetMove
{
    void Start()
    {
        dropSpeed = -3.0f;
    }

    void Update()
    {
        transform.Translate(0, this.dropSpeed * Time.deltaTime, 0);
        if (transform.position.y < -1.0f)
        {
            Destroy(gameObject);
        }
    }
}
