using UnityEngine;
using UnityEngine.Events;

public class TestUnityEvent : MonoBehaviour
{
    public UnityEvent testUnityEvent;//必ずpublicにしないとエラーが出る

    void Start()
    {
        testUnityEvent.AddListener(TestEvent1);
        testUnityEvent.AddListener(TestEvent2);
        testUnityEvent.AddListener(TestEvent3);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            testUnityEvent.Invoke();
        }
    }

    void TestEvent1()
    {
        Debug.Log("UnityEvent1が呼ばれた!");
    }
    void TestEvent2()
    {
        Debug.Log("UnityEvent2が呼ばれた!");
    }
    void TestEvent3()
    {
        Debug.Log("UnityEvent3が呼ばれた!");
    }
}
