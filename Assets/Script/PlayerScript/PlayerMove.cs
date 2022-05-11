using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public static float MoveNum;

    // Start is called before the first frame update
    void Start()
    {
        MoveNum = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(0.0f,0.0f,MoveNum);
    }
}
