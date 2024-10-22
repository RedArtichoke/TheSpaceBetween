using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCameraController : MonoBehaviour
{
    // Sensitivity of the mouse, because we all love a sensitive mouse
    public float mouseSensitivity = 100f;
    public Transform playerBody; // The body of the player, not to be confused with a real body
    public float lerpSpeed = 0.1f; // Speed of the lerp, not to be confused with a slurp

    private float xRotation = 0f; // X-axis rotation, because we like to spin
    private float currentXRotation = 0f; // Current X-axis rotation, for those who like to keep track
    private float yRotation = 0f; // Y-axis rotation, because spinning in two directions is better
    private float currentYRotation = 0f; // Current Y-axis rotation, for the organized spinners

    void Start()
    {
        // Lock that cursor like it's a treasure chest
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Make the cursor disappear like magic
    }

    void Update()
    {
        // Get the mouse movement, because we need to know where it's going
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust the xRotation based on mouseY, because up is down and down is up
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Keep the head from spinning too far

        // Adjust the yRotation based on mouseX, because left is right and right is left
        yRotation += mouseX;

        // Smoothly transition to the new rotation, like a gentle breeze
        currentXRotation = Mathf.Lerp(currentXRotation, xRotation, lerpSpeed);
        currentYRotation = Mathf.Lerp(currentYRotation, yRotation, lerpSpeed);

        // Apply the rotations, because we want to see the world from a new angle
        transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }
}
