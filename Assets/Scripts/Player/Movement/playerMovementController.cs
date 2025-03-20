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
    public float throwForce = 8.0f; // Force applied when throwing an object

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
    public bool canStand = true;
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

    private endgameGameInfo yogurt;//yogurt collectible counter
    [SerializeField] ElevatorCutscene vator;
    [SerializeField] ReadingNotes notes;
    [SerializeField] WashroomStall stall;

    // Public variables for audio
    public AudioClip pickupClip;
    private AudioSource audioSource;

    public GameObject QPrompt;
    public CanvasGroup QGroup;

    public SuitVoice suitVoice;

    public GameObject interactPromptPrefab; // Prefab to instantiate above glowing objects
    public GameObject dimensionPromptPrefab;

    // Add these variables at the top of the class with other private variables
    private Transform heldObjectOriginalParent;
    private Vector3 heldObjectOriginalPosition;
    private Quaternion heldObjectOriginalRotation;
    private float originalDrag;
    private float originalAngularDrag;
    private float carryDistance = 1.5f; // Increased from 0.5f to 1.5f - How far in front of the camera to hold objects
    private float carryHeight = -0.3f;  // Vertical offset for held objects
    private float carrySmoothing = 10f; // How smoothly to move the held object
    private float collisionPushback = 0.1f; // How much to push back when colliding
    public GameObject suitUIAnim;

    // Add these variables to the private variables section
    private float objectCollisionRadius = 0.3f; // Adjustable radius for collision detection
    private float minFloorDistance = 0.2f; // Minimum distance to keep objects above the floor
    private float largeObjectThreshold = 1.0f; // Size threshold for considering an object "large"
    private float largeObjectDistance = 2.5f; // Increased from 1.5f to 2.5f - Hold large objects even further away
    private bool isLargeObject = false;

    public doorOpen door1;
    public doorOpen door2;

    // Add this new variable
    private LayerMask collisionCheckMask; // Mask for collision checks

    private KeyBindManager keyBindManager;

    void Start()
    {
        keyBindManager = FindObjectOfType<KeyBindManager>();

        // Initialize player components and settings
        playerRb = GetComponent<Rigidbody>();
        playerRb.useGravity = true;
        initialCameraPosition = cameraTransform.localPosition;
        currentBobFrequency = bobFrequency;
        currentBobHeight = bobHeight;
        currentFOV = playerCamera.fieldOfView;
        interactableLayer = LayerMask.GetMask("Pickup");
        powerController = GetComponent<PowerController>();
        yogurt = GetComponent<endgameGameInfo>();

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

        // Initialize the collision check mask to exclude trigger-only layers
        // This excludes layers that typically only have triggers like UI, effects, etc.
        collisionCheckMask = ~(LayerMask.GetMask("UI", "Trigger", "Ignore Raycast"));
    }

    void Update()
    {
        // Update crouch state based on input and canStand
        if (Input.GetKey(keyBindManager.crouchKey) || Input.GetKey(KeyCode.RightControl))
        {
            // Player wants to crouch
            isCrouching = true;
        }
        else if (canStand)
        {
            // Player wants to stand and can stand
            isCrouching = false;
        }
        // If canStand is false, isCrouching remains true even if the key is released

        // Adjust bob frequency and movement speed based on crouch state
        bobFrequency = isCrouching ? 10.0f : 15.0f;

        // Dictate player move speed
        currentMovementSpeed = (isCrouching ? crouchingSpeed : walkingSpeed) + speedModifier;

        // Apply the appropriate hitbox size based on crouch state
        if (isCrouching)
        {
            // Set crouched height
            hitbox.height = 1.5f;
            hitbox.center = new Vector3(originalCenter.x, originalCenter.y - 0.20f, originalCenter.z);
        }
        else
        {
            // Restore original height (we already checked canStand when setting isCrouching)
            hitbox.height = originalHeight;
            hitbox.center = originalCenter;
        }

        // Handle player movement and camera effects
        HandleMovement();
        HandleCameraEffects();

        // Handle object interaction
        if (Input.GetKeyDown(keyBindManager.interactKey) || Input.GetMouseButtonDown(1))
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
        if (Input.GetKeyDown(keyBindManager.throwKey)&& heldObject != null)
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
            // Reset the original material before handling consumable objects
            // This prevents material persistence when objects are destroyed
            originalMaterial = null;
            
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

                door1.SetLockState(false);
                door2.SetLockState(false);
                
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

                suitUIAnim.SetActive(true);
                
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
                StartCoroutine(HandleMimicPickupWithDelay(hit.transform));
                return;
            }
            else if (hit.transform.CompareTag("FireExtinguisher"))
            {
                Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = false;  
                    rb.useGravity = true;    
                }
            }
            else if(hit.transform.name.Contains("yogurtCup"))
            {
                //add 1 to the counter and destroy the cup
                yogurt.yogurtCollected++;
                Destroy(hit.transform.gameObject);
                return;
            }
            else if(hit.transform.name == "geo_elevator_panel")
            {
                StartCoroutine(vator.ElevatorSequence());
                return;
            }
            else if (hit.transform.name.StartsWith("note"))
            {
                notes.reading = true;
                return;
            }
            else if (hit.transform.name.StartsWith("stallDoor"))
            {
                Debug.Log("stall 1");
                stall.useDoor(hit.transform);
                return;
            }

            heldObject = hit.transform;
            heldObjectRb = heldObject.GetComponent<Rigidbody>();
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();

            // Determine if this is a large object by checking its bounds
            if (heldObjectCollider != null)
            {
                float objectSize = Mathf.Max(
                    heldObjectCollider.bounds.size.x,
                    heldObjectCollider.bounds.size.y,
                    heldObjectCollider.bounds.size.z
                );
                isLargeObject = objectSize > largeObjectThreshold;
                
                // Adjust collision radius based on object size
                objectCollisionRadius = Mathf.Max(0.3f, objectSize * 0.3f);
            }

            if (heldObjectRb != null)
            {
                // Remove all forces from the object
                heldObjectRb.velocity = Vector3.zero;
                heldObjectRb.angularVelocity = Vector3.zero;

                // Configure rigidbody for carrying
                heldObjectRb.useGravity = false;
                heldObjectRb.isKinematic = true; // Make kinematic while held to prevent physics interactions
                
                // Store original drag values to restore later
                originalDrag = heldObjectRb.drag;
                originalAngularDrag = heldObjectRb.angularDrag;
                
                // For large objects, increase the drag to make them feel heavier when thrown
                if (isLargeObject)
                {
                    heldObjectRb.drag = Mathf.Max(heldObjectRb.drag, 1.0f);
                    heldObjectRb.angularDrag = Mathf.Max(heldObjectRb.angularDrag, 1.0f);
                }
                
                // Play pickup sound
                if (pickupClip != null)
                {
                    audioSource.clip = pickupClip;
                    audioSource.Play();
                }
            }

            if (heldObjectCollider != null)
            {
                IgnorePlayerCollisions(heldObjectCollider, true);
            }

            // Don't parent to camera, just track the object
            heldObjectOriginalParent = heldObject.parent;
            heldObject.SetParent(null); // Detach from any parent
            
            // Store original position and rotation
            heldObjectOriginalPosition = heldObject.position;
            heldObjectOriginalRotation = heldObject.rotation;

            // Update the instruction text on the object's prompt if it exists
            Renderer objectRenderer = heldObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                Transform promptTransform = objectRenderer.transform.Find("InteractPrompt");   
                if (promptTransform != null)
                {
                    TMP_Text instructionsText = promptTransform.Find("Canvas/Instructions").GetComponent<TMP_Text>();
                    if (instructionsText != null)
                    {
                            instructionsText.text = "Press " + keyBindManager.interactKey + " to Drop";
                    } 
                }
            }
        }
    }

    private IEnumerator HandleMimicPickupWithDelay(Transform mimicTransform)
    {
        float shakeDuration = 3f; 
        float shakeIntensity = 0.05f; 

        AudioSource audioSource = mimicTransform.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource not found on mimicTransform!");
        }

        float elapsedTime = 0f;
        Vector3 originalLocalPosition = mimicTransform.localPosition;
        Quaternion originalLocalRotation = mimicTransform.localRotation;

        while (elapsedTime < shakeDuration)
        {
            if (mimicTransform == null) yield break; 

            mimicTransform.localPosition = originalLocalPosition + new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity)
            );

            mimicTransform.localRotation = originalLocalRotation * Quaternion.Euler(
                Random.Range(-shakeIntensity * 10, shakeIntensity * 10),
                Random.Range(-shakeIntensity * 10, shakeIntensity * 10),
                Random.Range(-shakeIntensity * 10, shakeIntensity * 10)
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (mimicTransform != null)
        {
            mimicTransform.localPosition = originalLocalPosition;
            mimicTransform.localRotation = originalLocalRotation;

            DisguisedMimic disguisedMimic = mimicTransform.GetComponent<DisguisedMimic>();
            if (disguisedMimic != null)
            {
                DarkController darkController = GetComponent<DarkController>();
                if (darkController.inDark)
                {
                    yield break; 
                }

                GameObject originalMimic = disguisedMimic.originalMimic;
                if (originalMimic != null)
                {
                    MimicBehaviour mimicBehaviour = originalMimic.GetComponent<MimicBehaviour>();

                    originalMimic.SetActive(true);
                    originalMimic.transform.position += Vector3.up * 2;
                    originalMimic.transform.SetParent(null);

                    mimicBehaviour.foundNeighbours.Clear();
                    mimicBehaviour.isAttacking = true;
                }

                Destroy(mimicTransform.gameObject);
            }
        }
    }



    void DropObject(bool applyThrowForce)
    {
        // Drop the currently held object, optionally applying a throw force
        if (heldObject != null)
        {
            // Store a reference to the object before clearing heldObject
            Transform droppedObject = heldObject;
            Renderer objectRenderer = droppedObject.GetComponent<Renderer>();
            
            // Remove glow material from the held object before dropping it
            if (objectRenderer != null)
            {
                Material[] currentMaterials = objectRenderer.materials;
                // Always restore to just the first material (original material)
                if (currentMaterials.Length > 0)
                {
                    objectRenderer.materials = new Material[] { currentMaterials[0] };
                }
            }
            
            if (heldObjectRb != null)
            {
                // Restore original physics properties
                heldObjectRb.useGravity = true;
                heldObjectRb.isKinematic = false;
                heldObjectRb.drag = originalDrag;
                heldObjectRb.angularDrag = originalAngularDrag;

                heldObjectRb.velocity = Vector3.zero;
                heldObjectRb.angularVelocity = Vector3.zero;

                if (applyThrowForce)
                {
                    // Apply a force in the direction of the player's crosshair
                    Vector3 throwDirection = cameraTransform.forward;
                    
                    // Calculate the angle between the throw direction and down vector
                    float downwardAngle = Vector3.Angle(throwDirection, Vector3.down);
                    
                    // If looking too far downward (less than 45 degrees from straight down)
                    if (downwardAngle < 45f)
                    {
                        // Create a horizontal forward direction (zero Y component)
                        Vector3 horizontalForward = cameraTransform.forward;
                        horizontalForward.y = 0;
                        
                        // If the horizontal component is too small, use player's transform.forward instead
                        if (horizontalForward.magnitude < 0.1f)
                        {
                            horizontalForward = transform.forward;
                            horizontalForward.y = 0;
                        }
                        
                        horizontalForward.Normalize();
                        
                        // Add a MODEST upward component - enough to clear the floor but not launch into sky
                        // The more downward the player looks, the less upward force we apply
                        float upwardForce = Mathf.Lerp(0.1f, 0.3f, downwardAngle / 45f);
                        throwDirection = horizontalForward + Vector3.up * upwardForce;
                        throwDirection.Normalize();
                    }
                    else
                    {
                        // For normal throws, add a slight upward component to prevent objects from going through the floor
                        throwDirection += Vector3.up * 0.2f;
                        throwDirection.Normalize();
                    }
                    
                    // Adjust throw force based on object size
                    float adjustedThrowForce = isLargeObject ? throwForce * 0.7f : throwForce;
                    
                    heldObjectRb.AddForce(throwDirection * adjustedThrowForce, ForceMode.Impulse);
                    
                    // Add a bit of random torque for more natural throwing
                    Vector3 randomTorque = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f)
                    );
                    heldObjectRb.AddTorque(randomTorque * (isLargeObject ? 0.5f : 1.0f), ForceMode.Impulse);
                }
            }

            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                IgnorePlayerCollisions(heldObjectCollider, false);
            }

            // Restore original parent if it had one
            heldObject.SetParent(heldObjectOriginalParent);
            
            // Clear references
            heldObject = null;
            heldObjectRb = null;
            heldObjectOriginalParent = null;
            
            // Reset large object flag
            isLargeObject = false;
            
            // Update the instruction text on the object's prompt if it exists
            if (objectRenderer != null)
            {
                Transform promptTransform = objectRenderer.transform.Find("InteractPrompt");
                if (promptTransform != null)
                {
                    TMP_Text instructionsText = promptTransform.Find("Canvas/Instructions").GetComponent<TMP_Text>();
                    if (instructionsText != null)
                    {
                        instructionsText.text = "Press " + keyBindManager.interactKey + " to Pickup";
                    }
                }
            }
        }
    }

    void IgnorePlayerCollisions(Collider objectCollider, bool ignore)
    {
        // Get all colliders on the player and its children
        Collider[] playerColliders = GetComponentsInChildren<Collider>();
        
        // Also get the player's camera collider if it exists
        if (cameraTransform != null)
        {
            Collider[] cameraColliders = cameraTransform.GetComponentsInChildren<Collider>();
            if (cameraColliders.Length > 0)
            {
                // Combine the arrays
                List<Collider> colliderList = new List<Collider>(playerColliders);
                colliderList.AddRange(cameraColliders);
                playerColliders = colliderList.ToArray();
            }
        }
        
        // If the object has multiple colliders, handle all of them
        Collider[] objectColliders;
        if (objectCollider.transform.childCount > 0)
        {
            objectColliders = objectCollider.transform.GetComponentsInChildren<Collider>();
        }
        else
        {
            objectColliders = new Collider[] { objectCollider };
        }
        
        // Ignore collisions between all player colliders and all object colliders
        foreach (var pCollider in playerColliders)
        {
            if (pCollider == null || pCollider.isTrigger) continue;
            
            foreach (var oCollider in objectColliders)
            {
                if (oCollider == null || oCollider.isTrigger) continue;
                
                Physics.IgnoreCollision(pCollider, oCollider, ignore);
            }
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
                // Always store the original material for this renderer
                originalMaterial = currentMaterials[0];

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
                            instructionsText.text = "Press " + keyBindManager.interactKey + " to Open";

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
                else if(targetObject.name.StartsWith("stall"))
                {
                    if (nameText != null)
                        nameText.text = FormatObjectName(objectRenderer.transform.name);

                    if (instructionsText != null)
                        instructionsText.text = stall.isClosed ? "Press " + keyBindManager.interactKey + " to Open" : "Press " + keyBindManager.interactKey + " to Close";
                }
                else
                {
                    if (nameText != null)
                        nameText.text = FormatObjectName(objectRenderer.transform.name);

                    // Check if we're holding an object and update instructions accordingly
                    if (instructionsText != null)
                        instructionsText.text = heldObject != null ? "Press " + keyBindManager.interactKey + " to Drop" : "Press " + keyBindManager.interactKey + " to Pickup"; 
                }
            }

            lastHighlightedRenderer = objectRenderer;
        }
        else if (heldObject == null && lastHighlightedRenderer != null)
        {
            RestoreOriginalMaterial(lastHighlightedRenderer);
            Destroy(lastHighlightedRenderer.transform.Find("InteractPrompt")?.gameObject);
            lastHighlightedRenderer = null;
            // Make sure to reset originalMaterial here too
            originalMaterial = null;
        }
        else if (heldObject != null)
        {
            Renderer objectRenderer = heldObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                Material[] currentMaterials = objectRenderer.materials;
                if (currentMaterials.Length == 1 || !currentMaterials.Contains(glowMaterial))
                {
                    // Always store the original material for the held object
                    Material heldObjectMaterial = currentMaterials[0];
                    
                    Material[] newMaterials = new Material[2];
                    newMaterials[0] = heldObjectMaterial;
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
        if (renderer != null)
        {
            Material[] currentMaterials = renderer.materials;
            
            // Always attempt to remove the glow material if we have more than one material
            if (currentMaterials.Length > 1)
            {
                // If we have the original material reference, use it
                if (originalMaterial != null)
                {
                    renderer.materials = new Material[] { originalMaterial };
                }
                // Otherwise just use the first material (which should be the original)
                else if (currentMaterials.Length > 0)
                {
                    renderer.materials = new Material[] { currentMaterials[0] };
                }
            }
        }
        // Always reset the originalMaterial to prevent persistence
        originalMaterial = null;
    }

    void FixedUpdate()
    {
        if (heldObject != null && heldObjectRb != null)
        {
            // Adjust pickup distance based on object size
            float effectivePickupDistance = isLargeObject ? largeObjectDistance : carryDistance;
            
            // Calculate target position in front of the camera
            Vector3 targetPosition = cameraTransform.position + 
                                    cameraTransform.forward * effectivePickupDistance +
                                    cameraTransform.up * carryHeight;
            
            // Check for floor collision
            RaycastHit floorHit;
            if (Physics.Raycast(targetPosition, Vector3.down, out floorHit, 10f, collisionCheckMask))
            {
                // Ensure the object stays above the floor
                float floorY = floorHit.point.y;
                if (targetPosition.y - objectCollisionRadius < floorY + minFloorDistance)
                {
                    targetPosition.y = floorY + minFloorDistance + objectCollisionRadius;
                }
            }
            
            // Create a ray from the camera to detect obstacles
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            float maxCarryDistance = effectivePickupDistance + 0.5f;
            
            // Use a larger radius for SphereCast based on object size, but only check against physical colliders
            if (Physics.SphereCast(ray, objectCollisionRadius, out RaycastHit hit, maxCarryDistance, collisionCheckMask))
            {
                // Skip triggers and non-physical colliders
                if (!hit.collider.isTrigger)
                {
                    // If the hit object isn't the held object or its children, adjust the target position
                    if (hit.transform != heldObject && !hit.transform.IsChildOf(heldObject) && 
                        !heldObject.IsChildOf(hit.transform))
                    {
                        // Place the object just before the obstacle
                        float adjustedDistance = hit.distance - objectCollisionRadius - 0.1f;
                        if (adjustedDistance < 0.5f) adjustedDistance = 0.5f;
                        
                        targetPosition = cameraTransform.position + 
                                        cameraTransform.forward * adjustedDistance +
                                        cameraTransform.up * carryHeight;
                    }
                }
            }
            
            // Additional collision check using overlaps for large objects
            if (isLargeObject)
            {
                Collider[] heldColliders = heldObject.GetComponentsInChildren<Collider>();
                foreach (Collider col in heldColliders)
                {
                    if (col.isTrigger) continue;
                    
                    // Get the bounds of the collider
                    Bounds bounds = col.bounds;
                    Vector3 extents = bounds.extents;
                    
                    // Check for overlaps at the target position, only with physical colliders
                    Collider[] overlaps = Physics.OverlapBox(
                        targetPosition + (bounds.center - heldObject.position), 
                        extents, 
                        Quaternion.identity,
                        collisionCheckMask
                    );
                    
                    foreach (Collider overlap in overlaps)
                    {
                        // Skip triggers and non-physical colliders
                        if (overlap.isTrigger) continue;
                        
                        // Skip if it's part of the held object or the player
                        if (overlap.transform == heldObject || 
                            overlap.transform.IsChildOf(heldObject) || 
                            overlap.transform.CompareTag("Player") ||
                            heldObject.IsChildOf(overlap.transform))
                            continue;
                        
                        // Move the target position away from the overlap
                        Vector3 direction = (targetPosition - overlap.bounds.center).normalized;
                        targetPosition += direction * 0.1f;
                    }
                }
            }
            
            // Apply bobbing effect to held object
            if (isMoving)
            {
                float bobScale = isLargeObject ? 0.5f : 1.0f; // Reduce bobbing for large objects
                float objectVerticalBob = Mathf.Sin(bobbingTime - 0.1f) * currentBobHeight * bobScale;
                float objectHorizontalBob = Mathf.Sin((bobbingTime - 0.1f) * 0.5f) * bobWidth * bobScale;
                
                targetPosition += cameraTransform.right * objectHorizontalBob + 
                                 cameraTransform.up * objectVerticalBob;
            }
            
            // Smoothly move the object to the target position
            float smoothingFactor = isLargeObject ? carrySmoothing * 0.7f : carrySmoothing;
            heldObject.position = Vector3.Lerp(heldObject.position, targetPosition, Time.fixedDeltaTime * smoothingFactor);
            
            // Smoothly rotate the object to match camera orientation
            Quaternion targetRotation = cameraTransform.rotation;
            heldObject.rotation = Quaternion.Slerp(heldObject.rotation, targetRotation, 
                                                Time.fixedDeltaTime * smoothingFactor);
            
            // Check if object is too far from player
            float distanceToTarget = Vector3.Distance(heldObject.position, targetPosition);
            if (distanceToTarget > maxCarryDistance * 2)
            {
            heldObject.position = targetPosition;
            }
        }
    }

    private string FormatObjectName(string originalName)
    {
        // Remove unwanted words
        string[] unwantedWords = { "geo", "geometry", "open", "closed", "clone", "right", "left", "main", "tenance", "handle" };
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
