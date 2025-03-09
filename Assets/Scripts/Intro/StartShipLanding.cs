using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartShipLanding : MonoBehaviour
{
    public float interactionRange = 5f;
    public LayerMask button;

    private Camera playerCamera;

    public ShipFlight shipFlight;
    
    public AudioSource audioSource1;
    public AudioSource audioSource2;

    public CanvasGroup interactGroup;

    public AudioSource speaker;
    public AudioClip ship7;

    public GameObject buttonLight;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, button))
            {
                StartTravel();
                StartCoroutine(StopInteractprompt());

                speaker.Stop();
                buttonLight.SetActive(false);

                speaker.PlayOneShot(ship7);
                gameObject.layer = LayerMask.NameToLayer("Default");
                interactionRange = 0;
            }
        }
    }

    public void StartTravel()
    {
        shipFlight.MoveShip();
        audioSource1.Play();
        audioSource2.Play();
    }

    public IEnumerator StopInteractprompt()
    {
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        interactGroup.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            interactGroup.alpha = alpha;
            yield return null;  
        }
        
        interactGroup.alpha = 0f;
    }
}
