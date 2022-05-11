using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{

    public AudioClip getSE;
    AudioSource aud;

    private bool b_SE = true;

    // Start is called before the first frame update
    void Start()
    {
        this.aud = GetComponent<AudioSource>();
        b_SE = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (b_SE == true)
        {
            this.aud.PlayOneShot(this.getSE);
            b_SE = false;
        }
    }
}
