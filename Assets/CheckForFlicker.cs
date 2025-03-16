using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForFlicker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FlickerLight flickerLight = other.GetComponentInChildren<FlickerLight>(); // Check children too
        if (flickerLight != null)
        {
            flickerLight.canFlicker = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FlickerLight flickerLight = other.GetComponentInChildren<FlickerLight>(); // Check children too
        if (flickerLight != null)
        {
            flickerLight.canFlicker = false;
        }
    }
}
