using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetUnderMove : TargetMove
{
    void Start()
    {
        dropSpeed = -0.05f;
    }

    void Update()
    {
        transform.Translate(0, this.dropSpeed, 0);
        if (transform.position.y < -1.0f)
        {
            Destroy(gameObject);
        }
    }
}
