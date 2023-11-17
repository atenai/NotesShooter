using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float MoveNum = 14.0f;

    void Update()
    {
        this.transform.Translate(0.0f, 0.0f, MoveNum * Time.deltaTime);
    }
}
