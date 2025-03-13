using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    

    // Start is called before the first frame update
    void Start()
    {
        shipFlight.enabled = false;
        originalScale = breathingCircle.transform.localScale;

        player.enabled=false;
        camera.enabled=false;

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

        StartCoroutine(introTransition());


    }

    public void callibration()
    {
        StartCoroutine(calibrationStop());
        callibrationUI.SetActive(true);

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

        StartCoroutine(callibrationIntro());
    }

    private IEnumerator startArduino()
    {
        yield return new WaitForSeconds(10);
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
        yield return new WaitForSeconds(5);
        introCalibrationText.SetActive(false);
        callibration();

    }

    private IEnumerator calibrationStop()
    {
        yield return new WaitForSeconds(15);
        StopCalibration();
        calibrationEnd.SetActive(true);
        StartCoroutine(introTransition());




    }

    public IEnumerator introTransition()
    {
        yield return new WaitForSeconds(5);
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

}
