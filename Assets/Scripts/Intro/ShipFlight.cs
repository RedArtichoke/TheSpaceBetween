using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipFlight : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The transform you want this GameObject to move toward.")]
    public Transform target;

    [Tooltip("How fast you want the object to move.")]
    public float speed = 5f;

    private bool isMoving = false;

    public GameObject invisibleWall;

    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public GameObject FPrompt;
    public CanvasGroup FGroup;

    public GameObject EPrompt;
    public CanvasGroup EGroup;

    public AudioSource speaker;
    public AudioClip speaker8;

    //public rotatingDoorOpen door;
    public doorOpen door;

    public GameObject paAudio;

    public TextMeshProUGUI uiGoalText;
    public TextMeshProUGUI objectiveText;

    public GameObject railingRaise;
    public GameObject tutLandingMarker;


    public void MoveShip()
    {
        tutLandingMarker.SetActive(false);
        StartCoroutine(MoveToTarget());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SkipCutscene();
        }
    }

    IEnumerator MoveToTarget()
    {
        isMoving = true;
        
        while (Vector3.Distance(transform.position, target.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
            yield return null; // Wait a frame before looping again
        }
        
        transform.position = target.position;
        invisibleWall.SetActive(true);

        audioSource1.Stop();
        audioSource2.Stop();

        paAudio.SetActive(true);

        FPrompt.SetActive(true);
        StartCoroutine(StopFlashlightprompt());
        door.SetLockState(false);

        railingRaise.SetActive(true);

        speaker.PlayOneShot(speaker8);
        
        isMoving = false;

        uiGoalText.text = "Maintenace";

        objectiveText.text = "Fix the Maintenace issue";
    }

    public void SkipCutscene()
    {
        Debug.Log("SKipped");

        transform.position = target.position;
        invisibleWall.SetActive(true);

        audioSource1.Stop();
        audioSource2.Stop();
        door.SetLockState(false);

        paAudio.SetActive(true);
        railingRaise.SetActive(true);
        speaker.PlayOneShot(speaker8);

    }

    public IEnumerator StopFlashlightprompt()
    {
        yield return new WaitForSeconds (10f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        FGroup.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            FGroup.alpha = alpha;
            yield return null;  
        }
        
        FGroup.alpha = 0f;

        FPrompt.SetActive(false);

        yield return new WaitForSeconds (2f);

        EPrompt.SetActive(true);

        yield return new WaitForSeconds (10f);

        fadeDuration = 1f;
        elapsedTime = 0f;
        
        EGroup.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            EGroup.alpha = alpha;
            yield return null;  
        }
        
        EGroup.alpha = 0f;

        EPrompt.SetActive(false);


    }
}

