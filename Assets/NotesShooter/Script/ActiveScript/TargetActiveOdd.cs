using UnityEngine;

public class TargetActiveOdd : MonoBehaviour
{
    public GameObject CubeObject1;//ライトキューブオブジェクト
    public GameObject CubeObject2;//レフトキューブオブジェクト
    public GameObject CubeObject3;//ライトキューブオブジェクト

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            CubeObject1.SetActive(true);
            CubeObject2.SetActive(true);
            CubeObject3.SetActive(true);
        }
    }
}
