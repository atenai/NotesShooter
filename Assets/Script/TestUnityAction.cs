using UnityEngine;
using UnityEngine.Events;

public class TestUnityAction : MonoBehaviour
{
    UnityAction testUnityAction;

    void Start()
    {
        testUnityAction = TestAction;//UnityActionの関数登録時には、()を付けてはいけない！
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            testUnityAction.Invoke();
        }
    }

    void TestAction()
    {
        Debug.Log("UnityActionが呼ばれた!");
    }
}
