using UnityEngine;

public class TargetRightMove : TargetMove
{
    public virtual void Update()
    {
        transform.Translate(this.dropSpeed * Time.deltaTime, 0, 0);
        if (50.0f < transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
