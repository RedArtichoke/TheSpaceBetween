using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    private SphereCollider triggerRange;

    public enum AudioState
    {
        DontDestroyOnEnter,
        DestroyOnEnter
    }

    [SerializeField]
    private AudioState audioState;

    private bool readyToDestroy;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        triggerRange = GetComponent<SphereCollider>();

        readyToDestroy = false;

        if (triggerRange != null)
        {
            triggerRange.isTrigger = true;
        }
    }

    void Update()
    {
        if (readyToDestroy && !audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player has entered the trigger zone!");
            if (audioSource != null)
            {
                audioSource.Play();

                if (audioState == AudioState.DestroyOnEnter)
                {
                    readyToDestroy = true;
                    triggerRange.enabled = false;
                }
            }
        }
    }

}
