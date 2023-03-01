using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLeftMove : TargetMove
{
    void Start()
    {
        dropSpeed = -0.1f;
    }

    void Update()
    {
        transform.Translate(this.dropSpeed, 0, 0);
        if (transform.position.x < -50.0f)
        {
            Destroy(gameObject);
        }
    }
}
