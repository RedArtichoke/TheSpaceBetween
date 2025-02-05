using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightParticles : MonoBehaviour
{
    public GameObject particles;
    void OnDisable()
    {
        particles.SetActive(true);
    }
}
