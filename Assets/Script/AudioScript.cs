using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{

    public AudioClip getSE;
    AudioSource audioSource;

    private bool b_SEActive = true;

    // Start is called before the first frame update
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        b_SEActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (b_SEActive == true)
        {
            this.audioSource.PlayOneShot(this.getSE);
            b_SEActive = false;
        }
    }
}
