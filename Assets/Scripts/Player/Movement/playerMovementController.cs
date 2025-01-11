using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Controls player movement, object interaction, and camera effects.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    // Public variables for player movement and camera settings
    public float movementSpeed = 5.0f;
    public Transform cameraTransform;
    public Camera playerCamera;
    public float bobFrequency = 15.0f;
    public float neutralBobFrequency = 1.0f;
    private float bobHeight = 0.075f;
    private float bobWidth = 0.05f;
    public float transitionSpeed = 5.0f;
    public float movingFOV = 75.0f;
    public float neutralFOV = 70.0f;
    public float pickupDistance = 2.0f;
    public float throwForce = 4.0f; // Force applied when throwing an object

    // Private variables for internal state management
    private Transform heldObject = null;
    private Rigidbody heldObjectRb = null;
    private Rigidbody playerRb;
    private float bobbingTime = 0.0f;
    private Vector3 initialCameraPosition;
    private float currentBobFrequency;
    private float currentBobHeight;
    private float currentFOV;
    private LayerMask interactableLayer;
    private Renderer lastHighlightedRenderer = null;
    private Material originalMaterial = null;
    private float currentCrouchOffset = 0f;
    public bool isCrouching = false;

    // UI and visual elements
    public GameObject hudElement;
    public GameObject crosshair;
    public Material glowMaterial;

    //Other scripts
    public PlayerAudio footstepAudio;
    private bool footstepPlayed = false;
    private bool isMoving;
    private PowerController powerController;

    // Public variables for audio
    public AudioClip pickupClip;
    private AudioSource audioSource;

    void Start()
    {
        // Initialize player components and settings
        playerRb = GetComponent<Rigidbody>();
        playerRb.useGravity = true;
        initialCameraPosition = cameraTransform.localPosition;
        currentBobFrequency = bobFrequency;
        currentBobHeight = bobHeight;
        currentFOV = playerCamera.fieldOfView;
        interactableLayer = LayerMask.GetMask("Pickup");
        powerController = GetComponent<PowerController>();
        // Initialize HUD elements
        if (hudElement != null)
        {
            Transform crosshairTransform = hudElement.transform.Find("Crosshair");
            if (crosshairTransform != null)
            {
                crosshair = crosshairTransform.gameObject;
            }
        }

        // Ensure an AudioSource is attached to the player
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Update crouch state based on input
        isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        // Adjust bob frequency and movement speed based on crouch state
        bobFrequency = isCrouching ? 10.0f : 15.0f;
        movementSpeed = isCrouching ? 3.0f : 5.0f;

        // Handle player movement and camera effects
        HandleMovement();
        HandleCameraEffects();

        // Handle object interaction
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject(false);
            }
        }

        // Handle throwing the held object
        if (Input.GetMouseButtonDown(0) && heldObject != null)
        {
            DropObject(true);
        }

        // Update UI and object glow effects
        UpdateCrosshairVisibility();
        UpdateObjectGlow();
    }

    void HandleMovement()
    {
        // Calculate movement based on input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput == 0 && verticalInput == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput) * movementSpeed;
        movement.y = playerRb.velocity.y;
        playerRb.velocity = movement;

        //Debug.Log(isMoving);
    }

    void HandleCameraEffects()
    {
        // Calculate the magnitude of the player's horizontal velocity
        Vector3 horizontalVelocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        float velocityMagnitude = horizontalVelocity.magnitude;

        // Define a threshold for movement
        float movementThreshold = 0.1f;

        // Adjust camera bobbing and field of view based on movement
        float targetBobFrequency = (velocityMagnitude > movementThreshold) ? bobFrequency : neutralBobFrequency;
        float targetBobHeight = (velocityMagnitude > movementThreshold) ? bobHeight : bobHeight * 0.5f;
        float targetFOV = (velocityMagnitude > movementThreshold) ? movingFOV : neutralFOV;

        currentBobFrequency = Mathf.Lerp(currentBobFrequency, targetBobFrequency, Time.deltaTime * transitionSpeed);
        currentBobHeight = Mathf.Lerp(currentBobHeight, targetBobHeight, Time.deltaTime * transitionSpeed);
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime);

        bobbingTime += Time.deltaTime * currentBobFrequency;
        float verticalBob = Mathf.Sin(bobbingTime) * currentBobHeight;

        //Plays the foostep audio
        if(isMoving)
        {
            if (Mathf.Sin(bobbingTime) <= -0.99f && !footstepPlayed)
            {
                footstepAudio.Footstep();
                footstepPlayed = true;

            }
            if (Mathf.Sin(bobbingTime) > 0f)
            {
                footstepPlayed = false;
            }
        }
        


        // Determine target crouch offset based on isCrouching
        float targetCrouchOffset = isCrouching ? -1f : 0f;

        // Smoothly interpolate current crouch offset towards target
        currentCrouchOffset = Mathf.Lerp(currentCrouchOffset, targetCrouchOffset, Time.deltaTime * transitionSpeed);

        float horizontalBob = Mathf.Sin(bobbingTime * 0.5f) * bobWidth;

        // Apply bobbing to the flashlight
        Transform flashlightTransform = cameraTransform.Find("flashlight");
        if (flashlightTransform != null)
        {
            flashlightTransform.localPosition = new Vector3(horizontalBob, verticalBob, flashlightTransform.localPosition.z);
        }

        // Apply bobbing to the held object with a slight delay
        if (heldObject != null)
        {
            float objectVerticalBob = Mathf.Sin(bobbingTime - 0.1f) * currentBobHeight;
            float objectHorizontalBob = Mathf.Sin((bobbingTime - 0.1f) * 0.5f) * bobWidth;
            heldObject.localPosition = new Vector3(objectHorizontalBob, objectVerticalBob, pickupDistance);
        }

        // Apply crouch offset to vertical bob
        verticalBob += currentCrouchOffset;

        cameraTransform.localPosition = initialCameraPosition + new Vector3(horizontalBob, verticalBob, 0);
        playerCamera.fieldOfView = currentFOV;
    }

    void TryPickupObject()
    {
        // Attempt to pick up an object within range
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, interactableLayer))
        {
            if (hit.transform.CompareTag("DarkDevice"))
            {
                // Get the DarkController component
                DarkController darkController = GetComponent<DarkController>();
                if (darkController != null)
                {
                    darkController.hasDevice = true; // Set hasDevice to true
                }

                Destroy(hit.transform.gameObject); // Delete the object
                return; // Exit the method
            }
            else if (hit.transform.CompareTag("Battery"))
            {
                powerController.AddPower(75);

                // Play the pickup sound using the audio clip
                if (pickupClip != null)
                {
                    audioSource.clip = pickupClip;
                    audioSource.Play();
                }

                Destroy(hit.transform.gameObject); // Delete the object
                return; // Exit the method
            }

            heldObject = hit.transform;
            heldObjectRb = heldObject.GetComponent<Rigidbody>();
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();

            if (heldObjectRb != null)
            {
                // Remove all forces from the object
                heldObjectRb.velocity = Vector3.zero;
                heldObjectRb.angularVelocity = Vector3.zero;

                heldObjectRb.useGravity = false;
                heldObjectRb.isKinematic = false;
            }

            if (heldObjectCollider != null)
            {
                IgnorePlayerCollisions(heldObjectCollider, true);
            }

            heldObject.SetParent(cameraTransform);
            heldObject.localPosition = new Vector3(0, 0, pickupDistance);
        }
    }

    void DropObject(bool applyThrowForce)
    {
        // Drop the currently held object, optionally applying a throw force
        if (heldObject != null)
        {
            if (heldObjectRb != null)
            {
                heldObjectRb.useGravity = true;
                heldObjectRb.isKinematic = false;

                if (applyThrowForce)
                {
                    // Apply a force in the direction of the player's crosshair
                    Vector3 throwDirection = cameraTransform.forward;
                    heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
                }
            }

            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                IgnorePlayerCollisions(heldObjectCollider, false);
            }

            heldObject.SetParent(null);
            heldObject = null;
            heldObjectRb = null;
        }
    }

    void IgnorePlayerCollisions(Collider objectCollider, bool ignore)
    {
        // Ignore or restore collisions between the player and the object
        Collider[] playerColliders = GameObject.FindGameObjectsWithTag("Player")
                                              .Select(go => go.GetComponent<Collider>())
                                              .Where(c => c != null)
                                              .ToArray();
        foreach (var playerCollider in playerColliders)
        {
            Physics.IgnoreCollision(objectCollider, playerCollider, ignore);
        }
    }

    void UpdateCrosshairVisibility()
    {
        // Update crosshair visibility based on object interaction
        if (crosshair != null)
        {
            CanvasGroup crosshairCanvasGroup = crosshair.GetComponent<CanvasGroup>();
            if (crosshairCanvasGroup == null)
            {
                crosshairCanvasGroup = crosshair.AddComponent<CanvasGroup>();
            }

            if (heldObject != null)
            {
                crosshairCanvasGroup.alpha = 0f; // Make crosshair invisible
                return;
            }

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, interactableLayer))
            {
                crosshairCanvasGroup.alpha = 0f; // Make crosshair invisible
            }
            else
            {
                crosshairCanvasGroup.alpha = 1f; // Make crosshair visible
            }
        }
    }

    void UpdateObjectGlow()
    {
        // Update the glow effect on objects being looked at
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, pickupDistance, interactableLayer);

        if (heldObject == null && isHit)
        {
            Renderer objectRenderer = hit.transform.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                if (lastHighlightedRenderer != null && lastHighlightedRenderer != objectRenderer)
                {
                    RestoreOriginalMaterial(lastHighlightedRenderer);
                }

                Material[] currentMaterials = objectRenderer.materials;
                if (currentMaterials.Length == 1 || !currentMaterials.Contains(glowMaterial))
                {
                    if (originalMaterial == null)
                    {
                        originalMaterial = currentMaterials[0];
                    }

                    Material[] newMaterials = new Material[2];
                    newMaterials[0] = originalMaterial;
                    newMaterials[1] = glowMaterial;
                    objectRenderer.materials = newMaterials;
                }

                lastHighlightedRenderer = objectRenderer;
            }
        }
        else if (heldObject == null && lastHighlightedRenderer != null)
        {
            RestoreOriginalMaterial(lastHighlightedRenderer);
            lastHighlightedRenderer = null;
        }
        else if (heldObject != null)
        {
            Renderer objectRenderer = heldObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                Material[] currentMaterials = objectRenderer.materials;
                if (currentMaterials.Length == 1 || !currentMaterials.Contains(glowMaterial))
                {
                    Material[] newMaterials = new Material[2];
                    newMaterials[0] = originalMaterial;
                    newMaterials[1] = glowMaterial;
                    objectRenderer.materials = newMaterials;
                }
            }
        }
    }

    void RestoreOriginalMaterial(Renderer renderer)
    {
        // Restore the original material of a renderer
        if (renderer != null && originalMaterial != null)
        {
            renderer.materials = new Material[] { originalMaterial };
            originalMaterial = null;
        }
    }

    void FixedUpdate()
    {
        // Update the position and rotation of the held object in physics updates
        if (heldObject != null && heldObjectRb != null)
        {
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * pickupDistance;

            // Directly set the position to the target position
            heldObject.position = targetPosition;

            // Lock the rotation to match the camera's rotation
            heldObject.rotation = cameraTransform.rotation;
        }
    }
}
