using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetUnderMove : TargetMove
{
    // Start is called before the first frame update
    void Start()
    {
        dropSpeed = -0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, this.dropSpeed, 0);
        if (transform.position.y < -1.0f)
        {
            Destroy(gameObject);
        }
    }
}
