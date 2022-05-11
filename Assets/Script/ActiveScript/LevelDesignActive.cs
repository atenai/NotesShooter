using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesignActive : MonoBehaviour
{

    public GameObject ActiveTrueObject;//Trueオブジェクト
    public GameObject ActiveFalseObject;//Falseオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
