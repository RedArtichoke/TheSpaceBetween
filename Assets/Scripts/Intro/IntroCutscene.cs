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

    public bool introPlaying = true;

    public int SkipCount;

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
    public UIFlickerAnims UIComponents;
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

    public GameObject objectiveCanvas;

    public bool flashbang;

    private bool canCheckInput = false;
    private bool introSkipped = false;

    public TextMeshProUGUI uiGoalText;

    public TextMeshProUGUI objectiveText;

    public AudioSource computerSound;
    public AudioSource staticSound;
    public AudioSource logoIntro;
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

    public ControlPromptAnimator flashbangPrompt; 
    public ControlPromptAnimator flashLightPrompt; 
    public ControlPromptAnimator powerDisplay; 
    public ControlPromptAnimator damageDisplay; 
    public SubtitleText subtitleText;
    private GameObject instructionCanvas;

    public ShipFlight shipFlight;

    public AudioSource arduinoTYPINGAudio;

    private void OnEnable()
    {
        canCheckInput = false;
        StartCoroutine(EnableInputAfterDelay());
    }

    private IEnumerator EnableInputAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        canCheckInput = true;
    }

    private void Start()
    {
        Debug.Log("STARTING INTRO");
        keyBindManager = FindObjectOfType<KeyBindManager>();
        playerCamera.enabled = true;

        //Disable Suit
        suit.SetActive(false);

        //Disable HUD
        HUD.SetActive(false);

        //Disable player movement
        playerMovement.enabled = false;

        //Disable UI components
        UIComponents.gameObject.SetActive(false);

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

        DontDestroyOnLoad(computerSound.transform.parent.gameObject); //need for endgame info typing
    }
    void Update()
    {
        if (canCheckInput && Input.GetKeyDown(KeyCode.P))
        {
            if(SkipCount == 0)
            {
                HideIntroElements();
                StopAllCoroutines();
                introSkipped = true;
                introPlaying = false;
                staticSound.Stop();
                arduinoTYPINGAudio.Stop();

                StartCoroutine(SkippedFadeCutscene());

                
            }

            if(SkipCount == 2)
            {
                introSkipped = true;
                Intro = true;
                IntroScene.SetActive(false);

                StopAllCoroutines();

                // Re-enable player movement
                playerMovement.enabled = true;
                arduinoTYPINGAudio.Stop();

                // Re-enable UI components
                UIComponents.gameObject.SetActive(true);
                objectiveCanvas.SetActive(true);
                HUD.SetActive(true);
                crosshair.SetActive(true);
                noteInventory.SetActive(true);

                powerController.enabled = true;

                uiGoalText.text = "Fix the Maintenance Issue";
                objectiveText.text = "Go To the Maintenance Room";

                introPlaying = false;

                flashbangPrompt.ShowInstantly();
                flashLightPrompt.ShowInstantly();
                powerDisplay.ShowInstantly();
                damageDisplay.ShowInstantly();
                
                subtitleText.ClearSubtitles();

                shipFlight.SkipCutscene();
            }

            SkipCount +=1;
            
        }
    }

    private IEnumerator FadeCutscene()
    {
        yield return new WaitForSeconds(1f);
        
        text.gameObject.SetActive(true);
        
        StartCoroutine(TypeText(text, "Silly Yogurt Cup Presents...", 0.05f, 0.02f, true));
        
        
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOutText(text, 2f));

        yield return new WaitForSeconds(3f);
        GameTitle.gameObject.SetActive(true);
        StartCoroutine(FadeInImage(GameTitle, 2f));
        //computerSound.clip = computerclip2;
        logoIntro.Play();

        yield return new WaitForSeconds(2.5f);
        StartCoroutine(FadeOutImage(GameTitle, 2f));

        yield return new WaitForSeconds(5f);
        text4.gameObject.SetActive(true);
        StartCoroutine(TypeText(text4, "LOCATION\nOuter Space, Orbiting Planet Saturn", 0.05f, 0.02f, true));

        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeOutText(text4, 2f));

        yield return new WaitForSeconds(3f);
        text5.gameObject.SetActive(true);
        StartCoroutine(TypeText(text5, "OCCUPATION\nIndependent Maintenance Contractor", 0.05f, 0.02f, true));

        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeOutText(text5, 2f));

        yield return new WaitForSeconds(5f);

        staticSound.Stop();
        
        yield return new WaitForSeconds(2f);
        
        computerSound.clip = bedClip;
        computerSound.Play();

        float fadeDuration = 0.7f;
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

        yield return new WaitForSeconds(1f);

        moveControls.SetActive(true);
        noteInventory.SetActive(true);

        speaker.Play();
        UIComponents.gameObject.SetActive(true);
        instructionCanvas = uiGoalText.gameObject.transform.parent.gameObject;
        instructionCanvas.SetActive(false);
        subtitleText.gameObject.SetActive(true);
        subtitleText.PlayIntroSequence();

        yield return new WaitForSeconds(5f);

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
        yield return new WaitForSeconds(0.5f);

        moveControls.SetActive(false);
        
        yield return new WaitForSeconds(12f);
        // InteractControls.SetActive(true);

        //Enable suit
        suit.SetActive(true);
        tutSuitMarker.SetActive(true);
        closetDoor.OpenDoorCloset();
    }

    public IEnumerator SkippedFadeCutscene()
    {
        float fadeDuration = 0.7f;
        float elapsedTime = 0f;

        playerMovement.enabled = true;

        yield return new WaitForSeconds(1f);

        moveControls.SetActive(true);
        noteInventory.SetActive(true);

        speaker.Play();
        UIComponents.gameObject.SetActive(true);
        instructionCanvas = uiGoalText.gameObject.transform.parent.gameObject;
        instructionCanvas.SetActive(false);
        subtitleText.gameObject.SetActive(true);
        subtitleText.PlayIntroSequence();

        yield return new WaitForSeconds(5f);

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
        yield return new WaitForSeconds(0.5f);

        moveControls.SetActive(false);
        
        yield return new WaitForSeconds(12f);
        // InteractControls.SetActive(true);

        //Enable suit
        suit.SetActive(true);
        tutSuitMarker.SetActive(true);
        closetDoor.OpenDoorCloset();
    }

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
        subtitleText.PlayRampSuitSequence();
        speaker.Play();
        UIComponents.gameObject.SetActive(true);
        UIComponents.FlickerUI();
        instructionCanvas.SetActive(true);
        flashLightPrompt.RevealWithAnimation();
        powerDisplay.ShowInstantly();
        damageDisplay.ShowInstantly();
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
        flashbangPrompt.RevealWithAnimation();

        uiGoalText.text = "Use your Flashbang";
    }

    public void PlayFlashlightAudio2()
    {
        speaker.clip = voice4;
        speaker.Play();
        subtitleText.PlayFlashbangSequence();

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
        subtitleText.PlayPowerDrainSequence();
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
        subtitleText.PlayLandingSequence();
        button1.SetActive(false);
        button2.SetActive(true);
    }


    public IEnumerator TypeText(TextMeshProUGUI textComponent, string text, float typingSpeed, float speedVariation, bool glitchEffect)
    {
        textComponent.text = ""; // Reset text before typing
        computerSound.Play();
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
        computerSound.Stop();
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

