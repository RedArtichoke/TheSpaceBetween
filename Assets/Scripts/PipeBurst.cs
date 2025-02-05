using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBurst : MonoBehaviour
{
    public float beatsPerMinute;
    public GameObject pipeBurst;
    public GameObject burstEffect;

    public AudioSource pipeAudio;

    public GameObject burstMetal;

    void Start()
    {
        pipeBurst.SetActive(false);
        burstEffect.SetActive(false);
        burstMetal.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && beatsPerMinute > 100)
        {
            if (Random.value < 0.3f) // 30% chance
            {
                burstPipe();
            }
        }
    }

    public void burstPipe()
    {
        pipeBurst.SetActive(true);
        burstEffect.SetActive(true);
        burstMetal.SetActive(true);
        StartCoroutine(playAudio());
    }

    public IEnumerator playAudio()
    {
        yield return new WaitForSeconds(0.2f);
        pipeAudio.Play();

    }
}
