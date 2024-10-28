using UnityEngine;

public class TargetLeftMoveCustom : TargetLeftMove
{
    void Start()
    {
        this.dropSpeed = 50.0f;
    }

    public override void Update()
    {
        transform.Translate(-this.dropSpeed * Time.deltaTime, 0, 0);
        if (transform.position.x < 5.0f)
        {
            dropSpeed = 0.0f;
        }
    }
}
