using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetActiveEven : MonoBehaviour
{
    public GameObject CubeObject1;//ライトキューブオブジェクト
    public GameObject CubeObject2;//レフトキューブオブジェクト

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            CubeObject1.SetActive(true);
            CubeObject2.SetActive(true);
        }
    }
}
