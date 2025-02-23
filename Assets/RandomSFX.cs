using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSFX : MonoBehaviour
{
    public ParticleSystem particleSystemSparks;

    public float minWaitTime = 10f;
    public float maxWaitTime = 30f;

    private void Start()
    {
        StartCoroutine(PlayParticleRoutine());
    }

    private IEnumerator PlayParticleRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Play the particle system if it has been assigned
            if (particleSystemSparks != null)
            {
                particleSystemSparks.Play();
            }
            else
            {
                Debug.LogWarning("ParticleSystem not assigned on " + gameObject.name);
            }
        }
    }
}
