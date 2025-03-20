using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffDoor : MonoBehaviour
{
    public GameObject door;
    public GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.SetActive(false);
            wall.SetActive(true);
            Destroy(gameObject);
        }
    }
}
