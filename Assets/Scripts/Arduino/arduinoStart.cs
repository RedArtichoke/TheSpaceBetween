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

    private bool hideArduinoUI = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (serialControllerScript.arduinoConnected == true && hideArduinoUI == false)
        {
            connectArduinoUI.SetActive(true);
        }

    }

    public void noArduino()
    {
        hideArduinoUI = true;
        connectArduinoUI.SetActive(false);
        noArduinoUI.SetActive(true);
    }

    public void callibration()
    {

        hideArduinoUI = true;
        connectArduinoUI.SetActive(false);
        callibrationUI.SetActive(true);

        arduinoControllerScript.StartCalibration();
    }


}
