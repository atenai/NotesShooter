using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    const float MoveNum = 12.0f;

    void FixedUpdate()
    {
        this.transform.Translate(0.0f, 0.0f, MoveNum * Time.deltaTime);
    }
}
