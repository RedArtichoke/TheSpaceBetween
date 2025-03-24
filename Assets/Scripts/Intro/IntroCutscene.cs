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
    public GameObject HUD;

    [SerializeField] private TextMeshProUGUI text;
    //[SerializeField] private TextMeshProUGUI text2;
    //[SerializeField] private TextMeshProUGUI text2Title;
    [SerializeField] private Image GameTitle;
    [SerializeField] private TextMeshProUGUI text3;
    [SerializeField] private TextMeshProUGUI text3Title;
    [SerializeField] private TextMeshProUGUI text4;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private CanvasGroup canvasGroup; 
    [SerializeField] private CanvasGroup moveControlsGroup;
    [SerializeField] private GameObject moveControls;
    [SerializeField] private GameObject InteractControls;
    [SerializeField] private GameObject noteInventory;
    public PlayerMovementController playerMovement;
    public FpsCameraController playerCamera;
    public doorOpen closetDoor;
    public GameObject UIComponents;
    public AudioSource speaker;
    public AudioClip voice2;
    public AudioClip voice3;
    public AudioClip voice4;
    public AudioClip voice5;
    public AudioClip voice6;
    public GameObject suit;
    public GameObject crosshair;
    public PowerController powerController;
    public GameObject button1;
    public GameObject button2;

    public GameObject battery;

    public bool flashbang;

    private bool introSkipped = false;

    public TextMeshProUGUI uiGoalText;

    public TextMeshProUGUI objectiveText;

    public GameObject flashBangText;
    public GameObject flashBangText2;
    public GameObject flashBangText3;

    public AudioSource computerSound;
    public AudioSource staticSound;
    public AudioClip computerclip2;
    public AudioClip bedClip;
    private KeyBindManager keyBindManager;

    public GameObject tutBatteryMarker; 
    public GameObject tutSuitMarker;
    public GameObject tutLandingMarker;

    //public TextMeshProUGUI sillyYogurt; // Reference to the TextMeshProUGUI component
    //public AudioSource typingSound; // Optional typing sound
    public float baseTypingSpeed = 0.1f; // Base typing speed (seconds per character)
    public float randomSpeedFactor = 0.05f; // Variation in typing speed
    public bool useGlitchEffect = false; // Toggle glitch effect

    private void Start()
    {
        keyBindManager = FindObjectOfType<KeyBindManager>();
        playerCamera.enabled = true;

        //Disable Suit
        suit.SetActive(false);

        //Disable HUD
        HUD.SetActive(false);

        //Disable player movement
        playerMovement.enabled = false;

        //Disable UI components
        UIComponents.SetActive(false);

        //Yogurt cup presents...
        text.gameObject.SetActive(false);

        //The space between

        GameTitle.gameObject.SetActive(false);

        //Heartbeat
        text3.gameObject.SetActive(false);
        text3Title.gameObject.SetActive(false);
        
        //Location
        text4.gameObject.SetActive(false);

        //Ocupation
        text5.gameObject.SetActive(false);

        powerController.enabled = false;

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
        if(Input.GetKey(KeyCode.P) && !introSkipped)
        {
            introSkipped = true;
            
            Intro = false;
            IntroScene.SetActive(false);
            
            StopAllCoroutines();
            
            HideIntroElements();
            
            // Re enable player movement
            playerMovement.enabled = true;

            //Re enable UI components
            UIComponents.SetActive(true);

            HUD.SetActive(true);

            crosshair.SetActive(true);

            noteInventory.SetActive(true);

            powerController.enabled = true;

            uiGoalText.text = "Fix the Maintenance Issue";

            objectiveText.text = "Go To the Maintenance Room";

            flashBangText.SetActive(true);
            flashBangText2.SetActive(true);
            flashBangText3.SetActive(true);
            staticSound.Stop();

        }
    }

    private IEnumerator FadeCutscene()
    {
        yield return new WaitForSeconds(2f);
        
        text.gameObject.SetActive(true);
        StartCoroutine(TypeText(text, "Silly Yogurt Cup Presents...", 0.1f, 0.05f, true));
        computerSound.Play();
        
        yield return new WaitForSeconds(5f);
        //text.gameObject.GetComponent<TextFadeIn>().DisableText();
        StartCoroutine(FadeOutText(text, 3f));

        yield return new WaitForSeconds(5f);
        //text2.gameObject.SetActive(true);
        //text2Title.gameObject.SetActive(true);
        GameTitle.gameObject.SetActive(true);
        StartCoroutine(FadeInImage(GameTitle, 3f));
        computerSound.clip = computerclip2;
        computerSound.Play();

        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeOutImage(GameTitle, 3f));
        //text2.gameObject.GetComponent<TextFadeIn>().DisableText();
        //text2Title.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(4f);
        text3.gameObject.SetActive(true);
        text3Title.gameObject.SetActive(true);
        

        yield return new WaitForSeconds(4f);
        text3.gameObject.GetComponent<TextFadeIn>().DisableText();
        text3Title.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(1f);
        text4.gameObject.SetActive(true);
        StartCoroutine(TypeText(text4, "LOCATION\nOuter Space, Orbiting Planet Saturn", 0.1f, 0.05f, true));


        yield return new WaitForSeconds(7f);
        StartCoroutine(FadeOutText(text4, 3f));
        //text4.gameObject.GetComponent<TextFadeIn>().DisableText();

        yield return new WaitForSeconds(5f);
        text5.gameObject.SetActive(true);
        StartCoroutine(TypeText(text5, "OCCUPATION\nIndependent Maintenance Contractor", 0.1f, 0.05f, true));

        yield return new WaitForSeconds(4f);
        //ext5.gameObject.GetComponent<TextFadeIn>().DisableText();
        StartCoroutine(FadeOutText(text5, 3f));

        yield return new WaitForSeconds(3f);

        staticSound.Stop();
        
        yield return new WaitForSeconds(3f);
        

        computerSound.clip = bedClip;
        computerSound.Play();

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
        noteInventory.SetActive(true);

        speaker.Play();

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
        
        yield return new WaitForSeconds (20f);
        // InteractControls.SetActive(true);

        //Enable suit
        suit.SetActive(true);
        tutSuitMarker.SetActive(true);
        closetDoor.OpenDoorCloset();
    }

    // Add a new method to hide all UI elements
    private void HideIntroElements()
    {
        // Hide all text elements
        text.gameObject.SetActive(false);
        //text2.gameObject.SetActive(false);
        //text2Title.gameObject.SetActive(false);
        GameTitle.gameObject.SetActive(false);
        text3.gameObject.SetActive(false);
        text3Title.gameObject.SetActive(false);
        text4.gameObject.SetActive(false);
        text5.gameObject.SetActive(false);
        
        // Hide controls
        moveControls.SetActive(false);
        InteractControls.SetActive(false);
        
        // Set canvas groups to invisible
        canvasGroup.alpha = 0f;
        moveControlsGroup.alpha = 0f;
    }

    public void PlayFlashlightAudio()
    {
        speaker.clip = voice2;
        speaker.Play();
        UIComponents.SetActive(true);

        StartCoroutine(FlashlightControlPromptOpen());
    }

    public IEnumerator FlashlightControlPromptOpen()
    {
        bool flashPrompt = true;
        while (flashPrompt)
        {
            if(Input.GetKey(keyBindManager.flashlightKey))
            {
                flashPrompt = false;
            }

            yield return null;
        }

        PlayFlashlightAudio2();
        flashBangText.SetActive(true);
        flashBangText2.SetActive(true);
        flashBangText3.SetActive(true);

        uiGoalText.text = "Use your Flashbang";
    }

    public void PlayFlashlightAudio2()
    {
        speaker.clip = voice4;
        speaker.Play();

        StartCoroutine(FlashlightControlPromptClose());
    }

    public IEnumerator FlashlightControlPromptClose()
    {
        yield return new WaitForSeconds(1f);
        flashbang = true;

        while (flashbang)
        {
            yield return null;
        }

        uiGoalText.text = "Pickup the Battery";
        tutBatteryMarker.SetActive(true);

        PlayFlashlightAudio3();
    }

    public void PlayFlashlightAudio3()
    {
        speaker.clip = voice5;
        speaker.Play();

        StartCoroutine(FinalTutorialPrompt());
    }

    public IEnumerator FinalTutorialPrompt()
    {
        bool needPower = true;

        battery.layer = LayerMask.NameToLayer("Pickup");

        while (needPower)
        {
            if(powerController.power >= 85)
            {
                needPower = false;
            }
            yield return null;


        }

        uiGoalText.text = "Press the Landing Button";
        tutLandingMarker.SetActive(true);

        speaker.clip = voice6;
        speaker.Play();

        button1.SetActive(false);
        button2.SetActive(true);
    }


    IEnumerator TypeText(TextMeshProUGUI textComponent, string text, float typingSpeed, float speedVariation, bool glitchEffect)
    {
        textComponent.text = ""; // Reset text before typing

        // Typing animation
        foreach (char letter in text)
        {
            // Random typing speed for variation
            float delay = typingSpeed + Random.Range(-speedVariation, speedVariation);

            // Apply a glitch effect (random letter flicker)
            if (glitchEffect && Random.value > 0.8f)
            {
                textComponent.text += RandomLetter();
                yield return new WaitForSeconds(0.02f);
                textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 1);
            }

            // Append correct letter
            textComponent.text += letter;

            //// Play typing sound (optional)
            //if (typingSound != null)
            //{
            //    typingSound.Play();
            //}

            yield return new WaitForSeconds(delay);
        }

        // Add a blinking cursor effect
        StartCoroutine(BlinkingCursor(textComponent));
        // Add fading out effect
         // Fades out over 3 seconds
    }

    // Random letter for glitch effect
    private char RandomLetter()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return chars[Random.Range(0, chars.Length)];
    }

    // Coroutine to handle cursor blinking effect
    private IEnumerator BlinkingCursor(TextMeshProUGUI textComponent)
    {
        while (true)
        {
            textComponent.text = textComponent.text + "|";
            yield return new WaitForSeconds(0.5f);
            textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Coroutine to fade out text
    private IEnumerator FadeOutText(TextMeshProUGUI text, float fadeDuration)
    {
        float startAlpha = text.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, 0, time / fadeDuration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0); // Ensure fully faded
    }




    IEnumerator FadeInImage(Image image, float duration)
    {
        Color originalColor = image.color;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Start fully transparent

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Ensure fully visible
    }

    IEnumerator FadeOutImage(Image image, float duration)
    {
        Color originalColor = image.color;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Start fully visible

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure fully transparent
    }


}

