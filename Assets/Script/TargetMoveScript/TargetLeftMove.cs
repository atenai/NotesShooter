using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLeftMove : TargetMove
{
    public virtual void Update()
    {
        transform.Translate(-this.dropSpeed, 0, 0);
        if (transform.position.x < -50.0f)
        {
            Destroy(gameObject);
        }
    }
}
