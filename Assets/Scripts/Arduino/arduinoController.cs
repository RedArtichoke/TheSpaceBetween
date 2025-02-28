using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoHandler : MonoBehaviour
{

    public HeartRateAnimator heartRateScript;
    public PowerController powerScript;
    public SerialController serialControllerScript;

    private bool hasSentMessageFor100 = false;
    private bool hasSentMessageFor90 = false;
    private bool hasSentMessageFor80 = false;
    private bool hasSentMessageFor70 = false;
    private bool hasSentMessageFor60 = false;
    private bool hasSentMessageFor50 = false;
    private bool hasSentMessageFor40 = false;
    private bool hasSentMessageFor30 = false;
    private bool hasSentMessageFor20 = false;
    private bool hasSentMessageFor10 = false;
    private bool hasSentMessageFor0 = false;


    private List<float> bpmReadings = new List<float>();
    private float restingHeartRate = 0;
    private bool isCalibrating = false;
    private float calibrationDuration = 30f; // 30 seconds
    private float calibrationTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sendPowerLevel();

        if (isCalibrating)
        {
            calibrationTimer += Time.deltaTime;

            if (calibrationTimer >= calibrationDuration)
            {
                SetRestingHeartRate();
            }
        }


    }

    void OnMessageArrived(string msg)
    {
        Debug.Log("bpm: " + msg);
        float currentBPM = float.Parse(msg);

        // During calibration, store BPM readings
        if (isCalibrating)
        {
            bpmReadings.Add(currentBPM);
        }

        // Update heart rate in the animation script
        heartRateScript.beatsPerMinute = currentBPM;

        // Map BPM change if calibration is done
        if (restingHeartRate > 0)
        {
            float bpmChange = currentBPM - restingHeartRate;
            bpmChange = heartRateScript.BPMChange;
            Debug.Log("BPM Change: " + bpmChange);
            // You can map bpmChange to a value range as needed
        }
    }

    public void StartCalibration()
    {
        bpmReadings.Clear();
        isCalibrating = true;
        calibrationTimer = 0f;
        Debug.Log("Calibration started: Measuring heart rate...");
    }

    private void SetRestingHeartRate()
    {
        if (bpmReadings.Count > 0)
        {
            float sum = 0;
            foreach (float bpm in bpmReadings)
            {
                sum += bpm;
            }
            restingHeartRate = sum / bpmReadings.Count; // Get average BPM
            Debug.Log("Resting Heart Rate set to: " + restingHeartRate);
        }

        isCalibrating = false;
        bpmReadings.Clear();
    }

    public void sendFlashbang()
    {
        serialControllerScript.SendSerialMessage("L");
        Debug.Log("SENDING ARDUINO FLASHBANG");
    }

    void sendPowerLevel()
    {

        // 100 power range
        if (powerScript.power == 100f)
        {
            if (!hasSentMessageFor100)
            {
                serialControllerScript.SendSerialMessage("A");
                Debug.Log("SENDING ARDUINO POWER: 100");
                hasSentMessageFor100 = true;
            }
        }
        else
        {
            hasSentMessageFor100 = false;
        }

        // 90 power range
        if (90.0f < powerScript.power && powerScript.power < 99.9f)
        {
            if (!hasSentMessageFor90)
            {
                serialControllerScript.SendSerialMessage("B");
                Debug.Log("SENDING ARDUINO POWER: 90");
                hasSentMessageFor90 = true;
            }
        }
        else
        {
            hasSentMessageFor90 = false;
        }

        // 80 power range
        if (80.0f < powerScript.power && powerScript.power < 89.9f)
        {
            if (!hasSentMessageFor80)
            {
                serialControllerScript.SendSerialMessage("C");
                Debug.Log("SENDING ARDUINO POWER: 80");
                hasSentMessageFor80 = true;
            }
        }
        else
        {
            hasSentMessageFor80 = false;
        }

        // 70 power range
        if (70.0f < powerScript.power && powerScript.power < 79.9f)
        {
            if (!hasSentMessageFor70)
            {
                serialControllerScript.SendSerialMessage("D");
                Debug.Log("SENDING ARDUINO POWER: 70");
                hasSentMessageFor70 = true;
            }
        }
        else
        {
            hasSentMessageFor70 = false;
        }

        // 60 power range
        if (60.0f < powerScript.power && powerScript.power < 69.9f)
        {
            if (!hasSentMessageFor60)
            {
                serialControllerScript.SendSerialMessage("E");
                Debug.Log("SENDING ARDUINO POWER: 60");
                hasSentMessageFor60 = true;
            }
        }
        else
        {
            hasSentMessageFor60 = false;
        }

        // 50 power range
        if (50.0f < powerScript.power && powerScript.power < 59.9f)
        {
            if (!hasSentMessageFor50)
            {
                serialControllerScript.SendSerialMessage("F");
                Debug.Log("SENDING ARDUINO POWER: 50");
                hasSentMessageFor50 = true;
            }
        }
        else
        {
            hasSentMessageFor50 = false;
        }

        // 40 power range
        if (40.0f < powerScript.power && powerScript.power < 49.9f)
        {
            if (!hasSentMessageFor40)
            {
                serialControllerScript.SendSerialMessage("G");
                Debug.Log("SENDING ARDUINO POWER: 40");
                hasSentMessageFor40 = true;
            }
        }
        else
        {
            hasSentMessageFor40 = false;
        }

        // 30 power range
        if (30.0f < powerScript.power && powerScript.power < 39.9f)
        {
            if (!hasSentMessageFor30)
            {
                serialControllerScript.SendSerialMessage("H");
                Debug.Log("SENDING ARDUINO POWER: 30");
                hasSentMessageFor30 = true;
            }
        }
        else
        {
            hasSentMessageFor30 = false;
        }

        // 20 power range
        if (20.0f < powerScript.power && powerScript.power < 29.9f)
        {
            if (!hasSentMessageFor20)
            {
                serialControllerScript.SendSerialMessage("I");
                Debug.Log("SENDING ARDUINO POWER: 20");
                hasSentMessageFor20 = true;
            }
        }
        else
        {
            hasSentMessageFor20 = false;
        }

        // 10 power range
        if (10.0f < powerScript.power && powerScript.power < 19.9f)
        {
            if (!hasSentMessageFor10)
            {
                serialControllerScript.SendSerialMessage("J");
                Debug.Log("SENDING ARDUINO POWER: 10");
                hasSentMessageFor10 = true;
            }
        }
        else
        {
            hasSentMessageFor10 = false;
        }

        // 0 power range
        if (0.0f < powerScript.power && powerScript.power < 9.9f)
        {
            if (!hasSentMessageFor0)
            {
                serialControllerScript.SendSerialMessage("K");
                Debug.Log("SENDING ARDUINO POWER: 0");
                hasSentMessageFor0 = true;
            }
        }
        else
        {
            hasSentMessageFor0 = false;
        }


    }

    // Add this method to handle connection events
    void OnConnectionEvent(bool isConnected)
    {
        Debug.Log("Arduino " + (isConnected ? "connected" : "disconnected"));
        
        // You can add additional logic here if needed
        // For example, you might want to disable certain features when disconnected
    }

}
