using UnityEngine;

public class TargetRightMoveCustom : TargetRightMove
{
    void Start()
    {
        this.dropSpeed = 50.0f;
    }

    public override void Update()
    {
        transform.Translate(this.dropSpeed * Time.deltaTime, 0, 0);
        if (-5.0f < transform.position.x)
        {
            dropSpeed = 0.0f;
        }
    }
}
