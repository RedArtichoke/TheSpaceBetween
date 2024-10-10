using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public float lerpSpeed = 0.1f;

    float xRotation = 0f;
    float currentXRotation = 0f;
    float yRotation = 0f;
    float currentYRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        currentXRotation = Mathf.Lerp(currentXRotation, xRotation, lerpSpeed);
        currentYRotation = Mathf.Lerp(currentYRotation, yRotation, lerpSpeed);

        transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }
}
