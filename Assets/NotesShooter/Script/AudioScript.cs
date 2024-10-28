using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioClip getSE;
    AudioSource audioSource;

    bool isActive = true;

    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        isActive = true;
    }

    void Update()
    {
        if (isActive == true)
        {
            this.audioSource.PlayOneShot(getSE);
            isActive = false;
        }
    }
}
