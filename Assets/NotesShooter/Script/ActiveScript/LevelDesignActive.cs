using UnityEngine;

public class LevelDesignActive : MonoBehaviour
{
    public GameObject ActiveTrueObject;//Trueオブジェクト
    public GameObject ActiveFalseObject;//Falseオブジェクト

    void OnTriggerEnter(Collider hit)
    {
        //接触対象はPlayerタグですか？
        if (hit.CompareTag("Player"))
        {
            ActiveTrueObject.SetActive(true);
            ActiveFalseObject.SetActive(false);
        }
    }
}
