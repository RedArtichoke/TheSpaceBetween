using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arduinoStart : MonoBehaviour
{

    public SerialController serialControllerScript;
    public bool connectArduino;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (connectArduino && serialControllerScript.arduinoConnected == true)
        {
            
        }

    }
}
