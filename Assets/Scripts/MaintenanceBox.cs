using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintenanceBox : MonoBehaviour
{
    public float beatsPerMinute;
    public GameObject normalBox;
    public GameObject burstEffect;

    public AudioSource burstAudio;

    public GameObject[] lights;

    public GameObject[] lightParticles;
    public SphereCollider colliderSphere;

    public GameObject box;
    public GameObject brokenBox;

    void Start()
    {
        burstEffect.SetActive(false);
        brokenBox.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && beatsPerMinute > 100)
        {
            if (Random.value < 0.3f) // 30% chance
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
