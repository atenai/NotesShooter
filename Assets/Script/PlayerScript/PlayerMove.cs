using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float MoveNum = 12.0f;

    void Update()
    {
        this.transform.Translate(0.0f, 0.0f, MoveNum * Time.deltaTime);
    }
}
