using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRightMove : TargetMove
{
    // Start is called before the first frame update
    void Start()
    {
        dropSpeed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(this.dropSpeed, 0, 0);
        if (50.0f < transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
