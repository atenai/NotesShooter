using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRightMoveCustom : TargetRightMove
{
    void Start()
    {
        this.dropSpeed = 1.0f;
    }

    public override void Update()
    {
        transform.Translate(this.dropSpeed, 0, 0);
        if (-5.0f < transform.position.x)
        {
            dropSpeed = 0.0f;
        }
    }
}
