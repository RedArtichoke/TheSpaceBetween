using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public float interactionRange = 5f;  
    public LayerMask doorLayer;  

    private Camera playerCamera;

    public AudioSource audioSource;
    public AudioClip openDoor;
    public AudioClip closeDoor;

    private KeyBindManager keyBindManager;

    void Start()
    {
        keyBindManager = FindObjectOfType<KeyBindManager>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(keyBindManager.interactKey))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, doorLayer))
            {
                doorOpen door = hit.collider.GetComponent<doorOpen>();
                if (door != null)
                {
                    if (door.IsLocked)
                    {
                        audioSource.clip = closeDoor;
                        audioSource.Play();

                        door.LockFlash();

                        Debug.Log("The door is locked.");
                    }
                    else
                    {
                        audioSource.clip = openDoor;
                        audioSource.Play();

                        door.OpenDoor();
                    }
                }
            }
        }
    }
}
