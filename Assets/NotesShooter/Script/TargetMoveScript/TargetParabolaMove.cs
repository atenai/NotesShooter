using UnityEngine;

public class TargetParabolaMove : MonoBehaviour
{
    void Update()
    {
        this.GetComponent<Rigidbody>().AddForce(transform.right * -4.0f);//速すぎるとすり抜けてしまう
        this.GetComponent<Rigidbody>().AddForce(transform.up * 0.4f);//速すぎるとすり抜けてしまう
    }
}
