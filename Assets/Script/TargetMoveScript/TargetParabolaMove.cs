using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetParabolaMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Rigidbody>().AddForce(transform.right * -4.0f);//速すぎるとすり抜けてしまう
        this.GetComponent<Rigidbody>().AddForce(transform.up * 0.4f);//速すぎるとすり抜けてしまう
    }
}
