using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartShipBreakingAnim : MonoBehaviour
{
    public Animator ship;

    public doorOpen researchDoor;

    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ship.SetTrigger("BreakShip");
            audioSource.Play();
            researchDoor.SetLockState(false);
            Destroy(gameObject);
        }
    }
}
