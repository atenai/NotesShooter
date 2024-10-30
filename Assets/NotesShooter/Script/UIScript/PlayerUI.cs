using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] RawImage imageReticle;

    void Start()
    {
        imageReticle.color = new Color(255.0f, 255.0f, 255.0f, 150);
    }

    void Update()
    {
        if (FPSCamera.SingletonInstance.IsTargethit == true)
        {
            imageReticle.color = new Color32(255, 0, 0, 150);
        }
        else
        {
            imageReticle.color = new Color32(255, 255, 255, 150);
        }
    }
}
