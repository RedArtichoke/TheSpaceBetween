using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    public enum KeycardIdentity
    {
        ResearchRoom,
        MaintenanceRoom
    }

    public KeycardIdentity identity;  
}
