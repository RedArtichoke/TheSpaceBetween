using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffects : MonoBehaviour
{
    [Header("Impact Settings")]
    public float forceThreshold = 5f;

    [Header("Effects")]
    public ParticleSystem smokeEffectPrefab;
    //public AudioClip impactSound;

    [Header("Sound Variation")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= forceThreshold)
        {
            ContactPoint contact = collision.contacts[0];

            ParticleSystem smoke = Instantiate(smokeEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
            
            smoke.Play();
            smoke.transform.localScale *= 0.3f;

            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }
    }
}
