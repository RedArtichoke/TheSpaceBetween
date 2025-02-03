using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirePuzzle : MonoBehaviour
{
    public GameObject plugged;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plug"))
        {
            PluggedIn();
            Destroy(other.gameObject);
        }
    }

    public void PluggedIn()
    {
        plugged.SetActive(true);

    }
}
