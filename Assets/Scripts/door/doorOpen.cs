using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpen : MonoBehaviour
{
    private Animator animator;
    public bool IsLocked = true;  
    public Light doorLight;  
    public Light doorlightback;
    private bool isOpen = false;  
    private float closeDelay = 5f;  

    public Keycard.KeycardIdentity requiredKeycardIdentity;

    public AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateLightColor();  
    }

    public void OpenDoor()
    {
        if (animator != null && !isOpen && !IsLocked)
        {
            animator.SetTrigger("Open");
            isOpen = true;
            
            audioSource.Play();

            StartCoroutine(CloseDoorAfterDelay());
        }
    }

    public void SetLockState(bool locked)
    {
        IsLocked = locked;
        UpdateLightColor();
    }

    private void UpdateLightColor()
    {
        if (doorLight != null)
        {
            doorLight.color = IsLocked ? Color.red : Color.green;
            doorlightback.color = IsLocked ? Color.red : Color.green;
        }
    }

    private IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        if (animator != null && isOpen)
        {
            audioSource.Play();
            
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Keycard"))
        {
            Keycard keycard = other.GetComponent<Keycard>();
            if (keycard != null && keycard.identity == requiredKeycardIdentity)
            {
                SetLockState(false);  // Unlock the door
                Debug.Log("Correct keycard detected. Door is now unlocked.");
            }
            else
            {
                Debug.Log("Incorrect keycard identity.");
            }
        }
    }
}

