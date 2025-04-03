using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpen : MonoBehaviour
{
    public string doorID;
    private Animator animator;
    public bool IsLocked = true;  
    public Light doorLight;  
    public Light doorlightback;
    private bool isOpen = false;  
    private float closeDelay = 5f;  

    public Keycard.KeycardIdentity requiredKeycardIdentity;

    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioSource audioSource3;

    public Transform smokepoint1;
    public Transform smokepoint2;
    public Transform smokepoint3;
    public Transform smokepoint4;

    public GameObject SmokePrefab;
    public GameObject SmokePrefab2;

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
            audioSource2.Play();

            Instantiate(SmokePrefab, smokepoint1.transform);
            Instantiate(SmokePrefab, smokepoint2.transform);
            Instantiate(SmokePrefab, smokepoint3.transform);
            Instantiate(SmokePrefab2, smokepoint4.transform);

            StartCoroutine(CloseDoorAfterDelay());
        }
    }

    public void OpenDoorCloset()
    {
        if (animator != null && !isOpen && !IsLocked)
        {
            animator.SetTrigger("Open");
            isOpen = true;
            
            audioSource.Play();
            audioSource2.Play();

            Instantiate(SmokePrefab, smokepoint1.transform);
            Instantiate(SmokePrefab, smokepoint2.transform);
            Instantiate(SmokePrefab, smokepoint3.transform);
            Instantiate(SmokePrefab2, smokepoint4.transform);

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

    public void LockFlash()
    {
        StartCoroutine(FlashingLight());
    }

    private IEnumerator FlashingLight()
    {
        float flashDuration = 1.0f;     
        float flashInterval = 0.2f;      
        float elapsedTime = 0f;
        
        bool lightsOn = false;

        while (elapsedTime < flashDuration)
        {
            lightsOn = !lightsOn;
            doorLight.enabled = lightsOn;
            doorlightback.enabled = lightsOn;

            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }

        doorLight.enabled = true;
        doorlightback.enabled = true;
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
            SetLockState(false);
            Debug.Log("Correct keycard detected. Door is now unlocked.");

            keycard.RegisterDoorUnlock(doorID); 

            audioSource3.Play();
            StartCoroutine(FlashingLight());
            OpenDoor();
        }
        else
        {
            Debug.Log("Incorrect keycard identity.");
        }
    }


        if (other.CompareTag("KeyItem"))
        {
            OpenDoor();
        }

        
    }
}

