using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using TMPro; // Import TextMesh Pro namespace
using System.Text.RegularExpressions;

/// <summary>
/// Controls player movement, object interaction, and camera effects.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    // Public variables for player movement and camera settings
    public float currentMovementSpeed = 5.0f;
    public float speedModifier = 0.0f;   // <----- This variable is for adjusting both walk and crouch speeds
    private float walkingSpeed = 5.0f;
    private float crouchingSpeed = 3.0f;
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
    public CapsuleCollider hitbox;
    private float originalHeight;
    private Vector3 originalCenter;

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

    public GameObject QPrompt;
    public CanvasGroup QGroup;

    public SuitVoice suitVoice;

    public GameObject interactPromptPrefab; // Prefab to instantiate above glowing objects
    public GameObject dimensionPromptPrefab;

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

        originalHeight = hitbox.height;
        originalCenter = hitbox.center;

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

        // Dictate player move speed
        currentMovementSpeed = (isCrouching ? crouchingSpeed : walkingSpeed) + speedModifier;

        if (isCrouching)
        {
            // Set crouched height
            hitbox.height = 1.5f;
            hitbox.center = new Vector3(originalCenter.x, originalCenter.y - 0.20f, originalCenter.z);
        }
        else
        {
            // Restore original height
            hitbox.height = originalHeight;
            hitbox.center = originalCenter;
        }

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

    public Transform GetHeldObject()
    {
        return heldObject;
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

        Vector3 movement = (transform.forward * verticalInput + transform.right * horizontalInput) * currentMovementSpeed;
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
                    if (dimensionPromptPrefab != null)
                    {
                        dimensionPromptPrefab.SetActive(true);
                    }
                }
                suitVoice.PlaySuitInstallAudio();

                QPrompt.SetActive(true);
                StartCoroutine(DimensionPrompt());

                Destroy(hit.transform.gameObject); // Delete the object
                return; // Exit the method
            }
            else if (hit.transform.CompareTag("Battery"))
            {
                powerController.AddPower(100);
                suitVoice.PlayPowerRestoreAudio();
                
                // Play the pickup sound using the audio clip
                if (pickupClip != null)
                {
                    audioSource.clip = pickupClip;
                    audioSource.Play();
                }

                Destroy(hit.transform.gameObject); // Delete the object
                return; // Exit the method
            }
            else if (hit.transform.CompareTag("Suit"))
            {
                //powerController.AddPower(100);
                suitVoice.PlaySuitEquipAudio();

                crosshair.SetActive(true);

                powerController.enabled = true;
                
                // Play the pickup sound using the audio clip
                if (pickupClip != null)
                {
                    audioSource.clip = pickupClip;
                    audioSource.Play();
                }

                Debug.Log("Suit equipped");

                Destroy(hit.transform.gameObject); // Delete the object
                return; // Exit the method
            }
            else if (hit.transform.CompareTag("Mimic"))
            {
                // Handle picking up a disguised mimic
                DisguisedMimic disguisedMimic = hit.transform.GetComponent<DisguisedMimic>();
                if (disguisedMimic != null)
                {
                    DarkController darkController = GetComponent<DarkController>();
                    // If player in the dark, mimics will not exit disguise
                    if (darkController.inDark)
                    {
                        return;
                    }

                    GameObject originalMimic = disguisedMimic.originalMimic;
                    if (originalMimic != null)
                    {
                        MimicBehaviour mimicBehaviour = originalMimic.GetComponent<MimicBehaviour>();

                        // Enable the original mimic
                        originalMimic.SetActive(true);

                        // Raise the mimic 2 units above its current position
                        originalMimic.transform.position += Vector3.up * 2;

                        // Deparent the mimic from the disguised object
                        originalMimic.transform.SetParent(null);

                        // Make mimic attack the player
                        mimicBehaviour.foundNeighbours.Clear();
                        mimicBehaviour.isAttacking = true;
                    }

                    // Destroy the disguised mimic object
                    Destroy(hit.transform.gameObject);
                }
                return;
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
            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, interactableLayer | LayerMask.GetMask("button") | LayerMask.GetMask("Door")))
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
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, pickupDistance, 
                      interactableLayer | LayerMask.GetMask("button") | LayerMask.GetMask("Door"));

        if (heldObject == null && isHit)
        {
            // Use the collider's gameobject to avoid returning the parent in default cases
            GameObject targetObject = hit.collider.gameObject;
            
            Renderer objectRenderer = targetObject.GetComponent<Renderer>();

            // Look for a child with matching name criteria
            foreach (Transform child in targetObject.GetComponentsInChildren<Transform>())
            {
                if (child != targetObject.transform && 
                   (child.name.ToLower().Contains("door") || child.name == "x" ||child.name.Contains("geo")))
                {
                    objectRenderer = child.GetComponent<Renderer>();
                    break;
                }
            }

            if (objectRenderer == null)
            {
                Debug.LogWarning("No renderer found for the object or its children.");
                return;
            }

            if (lastHighlightedRenderer != null && lastHighlightedRenderer != objectRenderer)
            {
                RestoreOriginalMaterial(lastHighlightedRenderer);
                Destroy(lastHighlightedRenderer.transform.Find("InteractPrompt")?.gameObject);
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

            // Instantiate the interact prompt prefab above the object
            if (objectRenderer.transform.Find("InteractPrompt") == null)
            {
                GameObject instance = Instantiate(interactPromptPrefab, objectRenderer.transform);
                instance.name = "InteractPrompt";
                instance.transform.localPosition = Vector3.zero; // centre the prefab

                // Adjust scale so it isn't affected by the parent's scale
                Vector3 parentScale = objectRenderer.transform.lossyScale;
                instance.transform.localScale = new Vector3(1 / parentScale.x, 1 / parentScale.y, 1 / parentScale.z);

                TMP_Text instructionsText = instance.transform.Find("Canvas/Instructions").GetComponent<TMP_Text>();
                TMP_Text nameText = instance.transform.Find("Canvas/Name").GetComponent<TMP_Text>();

                // Compare using the target object and its name (ignoring parent name if necessary)
                if (targetObject.name.ToLower().Contains("doors"))
                {
                    doorOpen doorOpenComponent = targetObject.GetComponent<doorOpen>();
                    if (doorOpenComponent != null && doorOpenComponent.IsLocked)
                    {
                        if (nameText != null)
                            nameText.text = "Locked Door";

                        if (instructionsText != null)
                            instructionsText.text = "Cannot Open";
                    }
                    else
                    {
                        if (instructionsText != null)
                            instructionsText.text = "Press E to Open";

                        if (nameText != null)
                            nameText.text = "Door";
                    }
                }
                else if (targetObject.name == "x")
                {
                    if (instructionsText != null)
                        instructionsText.text = "Initiate Landing Sequence";
                    if (nameText != null)
                        nameText.text = "The Intervallum";
                }
                else
                {
                    if (nameText != null)
                        nameText.text = FormatObjectName(objectRenderer.transform.name);
                }
            }

            lastHighlightedRenderer = objectRenderer;
        }
        else if (heldObject == null && lastHighlightedRenderer != null)
        {
            RestoreOriginalMaterial(lastHighlightedRenderer);
            Destroy(lastHighlightedRenderer.transform.Find("InteractPrompt")?.gameObject);
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

    private IEnumerator DimensionPrompt()
    {
        yield return new WaitForSeconds (10f);
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        QGroup.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            QGroup.alpha = alpha;
            yield return null;  
        }
        yield return new WaitForSeconds (1f);

        QPrompt.SetActive(false);
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

    private string FormatObjectName(string originalName)
    {
        // Remove unwanted words
        string[] unwantedWords = { "geo", "geometry", "open", "closed", "clone", "right", "left", "main" };
        foreach (var word in unwantedWords)
        {
            originalName = originalName.Replace(word, "", System.StringComparison.OrdinalIgnoreCase);
        }

        // Remove numbers and special characters
        originalName = Regex.Replace(originalName, @"[^a-zA-Z]", "");

        // Insert spaces before capital letters
        System.Text.StringBuilder formattedName = new System.Text.StringBuilder();
        foreach (char c in originalName)
        {
            if (char.IsUpper(c) && formattedName.Length > 0)
            {
                formattedName.Append(' ');
            }
            formattedName.Append(c);
        }

        // Capitalise the first letter
        if (formattedName.Length > 0)
        {
            formattedName[0] = char.ToUpper(formattedName[0]);
        }

        return formattedName.ToString();
    }
}
