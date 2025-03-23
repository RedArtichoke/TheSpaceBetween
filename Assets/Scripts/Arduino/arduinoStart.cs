using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class arduinoStart : MonoBehaviour
{

    public SerialController serialControllerScript;
    public ArduinoHandler arduinoControllerScript;

    public GameObject connectArduinoUI;
    public GameObject noArduinoUI;
    public GameObject callibrationUI;

    public GameObject breathingCircle;

    private bool hideArduinoUI = false;
    private Vector3 originalScale;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float breathingSpeed = 2.0f;
    private bool isBreathing = false;

    public GameObject introText;
    public GameObject introCalibrationText;
    public GameObject calibrationEnd;
    public GameObject arduinoController;

    public GameObject intro;
    public GameObject arduinoIntroController;

    public PlayerMovementController player;
    public FpsCameraController camera;

    public ShipFlight shipFlight;

    public TextMeshProUGUI calibrationText;
    public Image calibrationCircle;

    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI textComponent2;
    public TextMeshProUGUI textComponent3;
    public TextMeshProUGUI textComponent4;
    public float baseTypingSpeed = 0.05f;  // Base delay between letters
    public float deletingSpeed = 0.01f;  // Base delay between letters
    public float randomSpeedFactor = 0.02f; // Random variance
    //public AudioSource typingSound; // Optional: Add a sci-fi beep sound
    public bool useGlitchEffect = true;

    private string fullText;
    private string fullText2;
    private string fullText3;
    private string fullText4;
    private Coroutine typingCoroutine;
    private Coroutine typingCoroutine2;
    private Coroutine typingCoroutine3;
    private Coroutine typingCoroutine4;

    // Start is called before the first frame update
    void Start()
    {
        shipFlight.enabled = false;
        originalScale = breathingCircle.transform.localScale;

        player.enabled = false;
        camera.enabled = false;

        fullText = textComponent.text;
        fullText2 = textComponent2.text;
        fullText3 = textComponent3.text;
        fullText4 = textComponent4.text;

        textComponent.text = "";  // Start with empty text
        textComponent2.text = "";  // Start with empty text
        textComponent3.text = "";  // Start with empty text
        textComponent4.text = "";  // Start with empty text

        StartTyping();

        StartCoroutine(startArduino());
    }

    // Update is called once per frame
    void Update()
    {
        if (isBreathing && breathingCircle != null)
        {
            BreathingEffect();
        }

        // Check for P key press to skip intro sequence
        if (Input.GetKeyDown(KeyCode.P))
        {
            SkipIntroSequence();
            shipFlight.enabled = true;
        }
    }

    public void noArduino()
    {
        hideArduinoUI = true;
        connectArduinoUI.SetActive(false);
        noArduinoUI.SetActive(true);
        arduinoController.SetActive(false);

        StartTyping2();

        StartCoroutine(introTransition());


    }

    public void callibration()
    {
        StartCoroutine(calibrationStop());
        callibrationUI.SetActive(true);
        StartCoroutine(FadeInText(calibrationText, 1.5f));
        StartCoroutine(FadeInImage(calibrationCircle, 1.5f));


        arduinoControllerScript.StartCalibration();

        isBreathing = true;
    }

    public void StopCalibration()
    {
        isBreathing = false;  // Stop breathing effect
        if (breathingCircle != null)
        {
            breathingCircle.transform.localScale = originalScale; // Reset scale
        }

        callibrationUI.SetActive(false);

    }

    private void BreathingEffect()
    {
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * breathingSpeed) + 1) / 2);
        breathingCircle.transform.localScale = originalScale * scale;
    }

    public void calibrationStart()
    {
        hideArduinoUI = true;
        connectArduinoUI.SetActive(false);

        introCalibrationText.SetActive(true);
        StartTyping3();

        StartCoroutine(callibrationIntro());
    }

    private IEnumerator startArduino()
    {
        yield return new WaitForSeconds(12);
        introText.SetActive(false);
        if (serialControllerScript.arduinoConnected == true && hideArduinoUI == false)
        {
            connectArduinoUI.SetActive(true);
        }
        else
        {
            noArduino();
        }

    }

    private IEnumerator callibrationIntro()
    {
        yield return new WaitForSeconds(12);
        introCalibrationText.SetActive(false);


        
        callibration();

    }

    private IEnumerator calibrationStop()
    {
        yield return new WaitForSeconds(15);
        StopCalibration();
        calibrationEnd.SetActive(true);

        StartTyping4();


        StartCoroutine(introTransition());




    }

    public IEnumerator introTransition()
    {
        
        yield return new WaitForSeconds(12);
        intro.SetActive(true);
        shipFlight.enabled = true;
        arduinoIntroController.SetActive(false);
        


    }

    /// <summary>
    /// Skips all intro sequences and transitions directly to gameplay
    /// </summary>
    public void SkipIntroSequence()
    {
        // Stop all running coroutines
        StopAllCoroutines();

        // Hide all UI elements
        if (introText != null) introText.SetActive(false);
        if (connectArduinoUI != null) connectArduinoUI.SetActive(false);
        if (noArduinoUI != null) noArduinoUI.SetActive(false);
        if (callibrationUI != null) callibrationUI.SetActive(false);
        if (introCalibrationText != null) introCalibrationText.SetActive(false);
        if (calibrationEnd != null) calibrationEnd.SetActive(false);
        if (arduinoController != null) arduinoController.SetActive(false);
        if (arduinoIntroController != null) arduinoIntroController.SetActive(false);

        // Stop breathing effect if active
        isBreathing = false;
        if (breathingCircle != null)
        {
            breathingCircle.transform.localScale = originalScale;
        }

        // Enable player controls
        if (player != null) player.enabled = true;
        if (camera != null) camera.enabled = true;

        // Show main game UI
        if (intro != null) intro.SetActive(true);
    }

    public void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    public void StartTyping2()
    {
        if (typingCoroutine2 != null)
            StopCoroutine(typingCoroutine2);

        typingCoroutine2 = StartCoroutine(TypeText2());
    }

    public void StartTyping3()
    {
        if (typingCoroutine3 != null)
            StopCoroutine(typingCoroutine3);

        typingCoroutine3 = StartCoroutine(TypeText3());
    }

    public void StartTyping4()
    {
        if (typingCoroutine4 != null)
            StopCoroutine(typingCoroutine4);

        typingCoroutine4 = StartCoroutine(TypeText4());

        //delay
        StartCoroutine(delay()); // Fades out over 2 seconds
        
    }

    IEnumerator TypeText()
    {
        textComponent.text = "";
        foreach (char letter in fullText)
        {
            // Random typing speed for variation
            float delay = baseTypingSpeed + Random.Range(-randomSpeedFactor, randomSpeedFactor);

            // Apply a glitch effect (random letter flicker)
            if (useGlitchEffect && Random.value > 0.8f)
            {
                textComponent.text += RandomLetter();
                yield return new WaitForSeconds(0.02f);
                textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 1);
            }

            // Append correct letter
            textComponent.text += letter;

            // Play typing sound
            //if (typingSound != null)
            //  typingSound.Play();

            yield return new WaitForSeconds(delay);
        }

        // Add a blinking cursor effect
        StartCoroutine(BlinkingCursor());
        //StartCoroutine(DeleteText());
        StartCoroutine(FadeOutText(textComponent, 3f)); // Fades out over 2 seconds
    }

    IEnumerator TypeText2()
    {
        textComponent2.text = "";
        foreach (char letter in fullText2)
        {
            // Random typing speed for variation
            float delay = baseTypingSpeed + Random.Range(-randomSpeedFactor, randomSpeedFactor);

            // Apply a glitch effect (random letter flicker)
            if (useGlitchEffect && Random.value > 0.8f)
            {
                textComponent2.text += RandomLetter();
                yield return new WaitForSeconds(0.02f);
                textComponent2.text = textComponent2.text.Substring(0, textComponent2.text.Length - 1);
            }

            // Append correct letter
            textComponent2.text += letter;

            // Play typing sound
            //if (typingSound != null)
            //  typingSound.Play();

            yield return new WaitForSeconds(delay);
        }

        // Add a blinking cursor effect
        StartCoroutine(BlinkingCursor2());
        //StartCoroutine(DeleteText2());
        StartCoroutine(FadeOutText(textComponent2, 3f)); // Fades out over 2 seconds
    }

    IEnumerator TypeText3()
    {
        textComponent3.text = "";
        foreach (char letter in fullText3)
        {
            // Random typing speed for variation
            float delay = baseTypingSpeed + Random.Range(-randomSpeedFactor, randomSpeedFactor);

            // Apply a glitch effect (random letter flicker)
            if (useGlitchEffect && Random.value > 0.8f)
            {
                textComponent3.text += RandomLetter();
                yield return new WaitForSeconds(0.02f);
                textComponent3.text = textComponent3.text.Substring(0, textComponent3.text.Length - 1);
            }

            // Append correct letter
            textComponent3.text += letter;

            // Play typing sound
            //if (typingSound != null)
            //  typingSound.Play();

            yield return new WaitForSeconds(delay);
        }

        // Add a blinking cursor effect
        StartCoroutine(BlinkingCursor3());
        //StartCoroutine(DeleteText3());
        StartCoroutine(FadeOutText(textComponent3, 2f)); // Fades out over 2 seconds
    }

    IEnumerator TypeText4()
    {
        textComponent4.text = "";
        foreach (char letter in fullText4)
        {
            // Random typing speed for variation
            float delay = baseTypingSpeed + Random.Range(-randomSpeedFactor, randomSpeedFactor);

            // Apply a glitch effect (random letter flicker)
            if (useGlitchEffect && Random.value > 0.8f)
            {
                textComponent4.text += RandomLetter();
                yield return new WaitForSeconds(0.02f);
                textComponent4.text = textComponent4.text.Substring(0, textComponent4.text.Length - 1);
            }

            // Append correct letter
            textComponent4.text += letter;

            // Play typing sound
            //if (typingSound != null)
            //  typingSound.Play();

            yield return new WaitForSeconds(delay);
        }

        // Add a blinking cursor effect
        StartCoroutine(BlinkingCursor4());

        //StartCoroutine(DeleteText3());
        
    }

    char RandomLetter()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        return chars[Random.Range(0, chars.Length)];
    }

    IEnumerator BlinkingCursor()
    {
        while (true)
        {
            textComponent.text += "|"; // Cursor on
            yield return new WaitForSeconds(0.5f);
            textComponent.text = textComponent.text.TrimEnd('|'); // Cursor off
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator BlinkingCursor2()
    {
        while (true)
        {
            textComponent2.text += "|"; // Cursor on
            yield return new WaitForSeconds(0.5f);
            textComponent2.text = textComponent2.text.TrimEnd('|'); // Cursor off
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator BlinkingCursor3()
    {
        while (true)
        {
            textComponent3.text += "|"; // Cursor on
            yield return new WaitForSeconds(0.5f);
            textComponent3.text = textComponent3.text.TrimEnd('|'); // Cursor off
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator BlinkingCursor4()
    {
        while (true)
        {
            textComponent4.text += "|"; // Cursor on
            yield return new WaitForSeconds(0.5f);
            textComponent4.text = textComponent4.text.TrimEnd('|'); // Cursor off
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator DeleteText()
    {
        yield return new WaitForSeconds(1f); // Wait before deleting

        while (textComponent.text.Length > 0) // Keep deleting until empty
        {
            textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 1);
            yield return new WaitForSeconds(deletingSpeed);
        }


    }

    IEnumerator DeleteText2()
    {
        yield return new WaitForSeconds(1f); // Wait before deleting

        while (textComponent2.text.Length > 0) // Keep deleting until empty
        {
            textComponent2.text = textComponent2.text.Substring(0, textComponent2.text.Length - 1);
            yield return new WaitForSeconds(deletingSpeed);
        }


    }

    IEnumerator DeleteText3()
    {
        yield return new WaitForSeconds(1f); // Wait before deleting

        while (textComponent3.text.Length > 0) // Keep deleting until empty
        {
            textComponent3.text = textComponent3.text.Substring(0, textComponent3.text.Length - 1);
            yield return new WaitForSeconds(deletingSpeed);
        }


    }

    IEnumerator FadeOutText(TextMeshProUGUI textComponent, float fadeDuration)
    {
        Color originalColor = textComponent.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure it's fully transparent
    }

    IEnumerator FadeInText(TextMeshProUGUI textComponent, float fadeDuration)
    {
        Color originalColor = textComponent.color;
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Start fully transparent

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Ensure fully visible
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

    IEnumerator delay()
    {
        yield return new WaitForSeconds(4f); // Wait before deleting

        StartCoroutine(FadeOutText(textComponent4, 2f)); // Fades out over 2 seconds
    }

}