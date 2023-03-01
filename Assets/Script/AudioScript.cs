using UnityEngine;

public class AudioScript : MonoBehaviour
{

    public AudioClip getSE;
    AudioSource audioSource;

    private bool b_SEActive = true;

    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        b_SEActive = true;
    }

    void Update()
    {
        if (b_SEActive == true)
        {
            this.audioSource.PlayOneShot(this.getSE);
            b_SEActive = false;
        }
    }
}
