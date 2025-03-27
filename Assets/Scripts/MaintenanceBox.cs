using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintenanceBox : MonoBehaviour
{
    public HeartRateAnimator heartRateScript;
    //public float beatsPerMinute;
    public GameObject normalBox;
    public GameObject burstEffect;

    public AudioSource burstAudio;

    public GameObject[] lights;

    public GameObject[] lightParticles;
    public SphereCollider colliderSphere;

    public GameObject box;
    public GameObject brokenBox;

    private HeartRateSimulator heartRateSimulator;

    void Start()
    {
        heartRateSimulator = GameObject.FindWithTag("HeartRateSimulator").GetComponent<HeartRateSimulator>();

        burstEffect.SetActive(false);
        brokenBox.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && heartRateScript.beatsPerMinute >= 100)
        {
            if (Random.value < 0.5f) // 50%
            {
                burstBox();
            }
        }
    }

    public void burstBox()
    {
        // Deactivate all lights in the array
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }

        foreach (GameObject lightParticle in lightParticles)
        {
            lightParticle.SetActive(true);
        }

        burstEffect.SetActive(true);
        burstAudio.Play();
        colliderSphere.enabled = false;

        brokenBox.SetActive(true);
        box.GetComponent<MeshRenderer>().enabled = false;

        heartRateSimulator.BumpUp();

        //StartCoroutine(playAudio());
    }


    public IEnumerator playAudio()
    {
        yield return new WaitForSeconds(0.2f);
        burstAudio.Play();
        //yield return new WaitForSeconds(2f);
        //burstAudio.enabled = false;

    }
}
