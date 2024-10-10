using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heartRateAnimator : MonoBehaviour
{
    public float beatsPerMinute = 150.0f;
    public float pulseMagnitude = 0.1f;

    private Vector3 initialScale;
    private float pulseSpeed;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        pulseSpeed = beatsPerMinute / 60.0f * Mathf.PI * 2;
        float time = Time.time * pulseSpeed;
        float scaleFactor = 1 + Mathf.Sin(time) * Mathf.Exp(-Mathf.Pow(time % (2 * Mathf.PI) - Mathf.PI, 2)) * pulseMagnitude;
        transform.localScale = initialScale * scaleFactor;
    }
}
