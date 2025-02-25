using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class IntroCutscene : MonoBehaviour
{
    public bool Intro;
    public GameObject IntroScene;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text2Title;
    [SerializeField] private TextMeshProUGUI text3;
    [SerializeField] private TextMeshProUGUI text3Title;
    [SerializeField] private TextMeshProUGUI text4;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private CanvasGroup canvasGroup; 
    [SerializeField] private CanvasGroup moveControlsGroup;
    [SerializeField] private GameObject moveControls;
    [SerializeField] private GameObject InteractControls;
    public PlayerMovementController playerMovement;

    private void Start()
    {
        //Disable player movement
        playerMovement.enabled = false;

        //Yogurt cup presents...
        text.gameObject.SetActive(false);

        //The space between
        text2.gameObject.SetActive(false);
        text2Title.gameObject.SetActive(false);

        //Heartbeat
        text3.gameObject.SetActive(false);
        text3Title.gameObject.SetActive(false);
        
        //Location
        text4.gameObject.SetActive(false);

        //Ocupation
        text5.gameObject.SetActive(false);

        if(Intro)
        {
            IntroScene.SetActive(true);
            StartCoroutine(FadeCutscene());
        }    
        else 
        {
            IntroScene.SetActive(false);
        }
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.P))
        {
            Intro = false;
            IntroScene.SetActive(false);
            StopCoroutine(FadeCutscene());

            //Re enable player movement
            playerMovement.enabled = true;
        }
    }

    private IEnumerator FadeCutscene()
    {
        yield return new WaitForSeconds(2f);

        text.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(3f);
        text.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(1f);
        text2.gameObject.SetActive(true);
        text2Title.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);
        text2.gameObject.GetComponent<TextFadeIn>().DisableText();
        text2Title.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(1f);
        text3.gameObject.SetActive(true);
        text3Title.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);
        text3.gameObject.GetComponent<TextFadeIn>().DisableText();
        text3Title.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(1f);
        text4.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);
        text4.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(1f);
        text5.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);
        text5.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(1f);


        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        canvasGroup.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;  
        }
        
        canvasGroup.alpha = 0f;

        //Re enable player movement
        playerMovement.enabled = true;

        yield return new WaitForSeconds(2f);

        moveControls.SetActive(true);

        yield return new WaitForSeconds (8f);

        fadeDuration = 1f;
        elapsedTime = 0f;
        
        moveControlsGroup.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            moveControlsGroup.alpha = alpha;
            yield return null;  
        }
        yield return new WaitForSeconds (1f);

        moveControls.SetActive(false);
        

        yield return new WaitForSeconds (3f);

        InteractControls.SetActive(true);
    }
}

