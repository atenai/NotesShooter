using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarGamePad : MonoBehaviour
{
    [SerializeField] ScrollRect ScrollRect;

    void Update()
    {
        float v = Input.GetAxis("Vertical");

        if (Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Vertical") <= 0)
            {
                ScrollRect.verticalNormalizedPosition = Mathf.Lerp(ScrollRect.verticalNormalizedPosition, v, 0.01f);
            }


            if (Input.GetAxis("Vertical") >= 0)
            {
                ScrollRect.verticalNormalizedPosition = Mathf.Lerp(ScrollRect.verticalNormalizedPosition, v + 1, 0.01f);
            }
        }
    }
}
