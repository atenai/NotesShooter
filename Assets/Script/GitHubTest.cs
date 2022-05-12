using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GitHubTest : MonoBehaviour
{

    string s_GitHub = "GitHubTest";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(s_GitHub);
        Debug.Log("s_GitHubTest2");
        Debug.Log("SSHを設定していないのに何故か上手くいく");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
