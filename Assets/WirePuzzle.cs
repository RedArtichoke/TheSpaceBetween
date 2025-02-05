using System.Collections;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WirePuzzle : MonoBehaviour
{
    public GameObject plugged;
    public Transform pluggedPos;
    public LineRenderer wire1;
    public LineRenderer wire2;

    public Light lightFixture;
    public GameObject Particles;
    public AudioSource clickAudio;

    void Start()
    {
        lightFixture.color = Color.red;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plug"))
        {
            Destroy(other.gameObject);
            wire1.enabled = false;
            wire2.enabled = true;
            PluggedIn();
        }
    }


    public void PluggedIn()
    {
        plugged.SetActive(true);
        Particles.SetActive(true);
        clickAudio.Play();
        lightFixture.color = Color.green;
    }
}
