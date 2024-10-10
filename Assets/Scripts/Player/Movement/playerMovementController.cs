using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed = 5.0f;
    public Transform cameraTransform;
    public Camera playerCamera;
    public float bobFrequency = 2.0f;
    public float neutralBobFrequency = 1.0f;
    public float bobHeight = 0.1f;
    public float bobWidth = 0.05f;
    public float resetSpeed = 2.0f;
    public float transitionSpeed = 5.0f;
    public float movingFOV = 75.0f;
    public float neutralFOV = 70.0f;

    private Rigidbody rb;
    private float bobbingTime = 0.0f;
    private Vector3 initialCameraLocalPosition;
    private float currentBobFrequency;
    private float currentBobHeight;
    private float currentFOV;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        initialCameraLocalPosition = cameraTransform.localPosition;
        currentBobFrequency = bobFrequency;
        currentBobHeight = bobHeight;
        currentFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forwardMovement = transform.forward * moveVertical;
        Vector3 rightMovement = transform.right * moveHorizontal;
        Vector3 movement = forwardMovement + rightMovement;

        Vector3 velocity = movement * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        float targetBobFrequency = (movement.magnitude > 0) ? bobFrequency : neutralBobFrequency;
        float targetBobHeight = (movement.magnitude > 0) ? bobHeight : bobHeight * 0.5f;
        float targetFOV = (movement.magnitude > 0) ? movingFOV : neutralFOV;

        currentBobFrequency = Mathf.Lerp(currentBobFrequency, targetBobFrequency, Time.deltaTime * transitionSpeed);
        currentBobHeight = Mathf.Lerp(currentBobHeight, targetBobHeight, Time.deltaTime * transitionSpeed);
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * 1);

        bobbingTime += Time.deltaTime * currentBobFrequency;

        float verticalBob = Mathf.Sin(bobbingTime) * currentBobHeight;
        float horizontalBob = Mathf.Sin(bobbingTime * 0.5f) * bobWidth;

        cameraTransform.localPosition = initialCameraLocalPosition + new Vector3(horizontalBob, verticalBob, 0);
        playerCamera.fieldOfView = currentFOV;
    }
}
