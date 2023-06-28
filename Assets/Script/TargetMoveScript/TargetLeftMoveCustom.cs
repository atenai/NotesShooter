using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLeftMoveCustom : TargetLeftMove
{
    void Start()
    {
        this.dropSpeed = 1.0f;
    }

    public override void Update()
    {
        transform.Translate(-this.dropSpeed, 0, 0);
        if (transform.position.x < 5.0f)
        {
            dropSpeed = 0.0f;
        }
    }
}
