using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceCameraRotation : MonoBehaviour
{
    public float maxRotationAngle = 45f;

    public float rotationSpeed = 20f;

    private float currentAngle = 0f;
    private bool rotatingRight = true;

    void Update()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;
        if (rotatingRight)
        {
            currentAngle += rotationStep;
            if (currentAngle >= maxRotationAngle)
            {
                currentAngle = maxRotationAngle;
                rotatingRight = false;
            }
        }
        else
        {
            currentAngle -= rotationStep;
            if (currentAngle <= -maxRotationAngle)
            {
                currentAngle = -maxRotationAngle;
                rotatingRight = true;
            }
        }

        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }
}
