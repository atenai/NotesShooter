using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float MoveNum = 0.2f;

    void Update()
    {
        this.transform.Translate(0.0f, 0.0f, MoveNum);
    }
}
