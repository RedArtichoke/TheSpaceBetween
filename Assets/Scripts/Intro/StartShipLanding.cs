using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public bool isTakeOff;
    public GameObject gameEndCanvas;
    public CanvasGroup blackScreen;

    public GameObject UIComponents;
    private KeyBindManager keyBindManager;

    public endgameGameInfo gameInfo;

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
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, button))
            {
                if(isTakeOff)
                {
                    StartCoroutine(EndGame());
                    buttonLight.GetComponent<Light>().enabled = false;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    interactionRange = 0;
                }
                else
                {
                    StartTravel();
                    StartCoroutine(StopInteractprompt());

                    speaker.Stop();

                    buttonLight.GetComponent<Light>().enabled = false;

                    speaker.PlayOneShot(ship7);
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    interactionRange = 0;
                }
                
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

    public IEnumerator EndGame()
    {
        gameEndCanvas.SetActive(true);

        yield return new WaitForSeconds (1f);

        UIComponents.SetActive(false);

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        blackScreen.alpha = 0f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            blackScreen.alpha = alpha;
            yield return null;  
        }

        gameInfo.stopTimer();

        yield return new WaitForSeconds (1f);

        SceneManager.LoadScene("EndGame");
    }
}
