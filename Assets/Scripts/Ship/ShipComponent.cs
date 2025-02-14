using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public enum ShipComponentIdentity
    {
        Engine,
        SteeringWheel,
        Tesseract,
        Powercell
    }

    public ShipComponentIdentity identity;
}
