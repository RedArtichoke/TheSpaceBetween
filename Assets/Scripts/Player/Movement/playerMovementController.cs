using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public float pickupDistance = 2.0f;
    private Transform pickedObject = null;
    private Rigidbody pickedObjectRb = null;
    private Collider playerCollider;

    private Rigidbody rb;
    private float bobbingTime = 0.0f;
    private Vector3 initialCameraLocalPosition;
    private float currentBobFrequency;
    private float currentBobHeight;
    private float currentFOV;
    private LayerMask interactableLayer;

    public GameObject hudElement; // Reference to the HUD element
    public GameObject crosshair; // Direct reference to the Crosshair
    public Material glowMaterial; // Reference to the glow material
    private Material originalMaterial; // To store the original material of the object

    private Renderer lastHighlightedRenderer = null;
    private Dictionary<Renderer, Material[]> originalMaterialsMap = new Dictionary<Renderer, Material[]>(); // Map to store original materials

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        initialCameraLocalPosition = cameraTransform.localPosition;
        currentBobFrequency = bobFrequency;
        currentBobHeight = bobHeight;
        currentFOV = playerCamera.fieldOfView;
        interactableLayer = LayerMask.GetMask("Pickup");
        playerCollider = GetComponent<Collider>();

        if (hudElement != null)
        {
            Transform crosshairTransform = hudElement.transform.Find("Crosshair");
            if (crosshairTransform != null)
            {
                crosshair = crosshairTransform.gameObject;
            }
            else
            {
                Debug.LogWarning("Crosshair object not found under HUD element.");
            }
        }
        else
        {
            Debug.LogWarning("HUD element is not assigned.");
        }
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (pickedObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }

        UpdateCrosshairVisibility();
        UpdateObjectGlow();
    }

    void TryPickupObject()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance, interactableLayer))
        {
            pickedObject = hit.transform;
            pickedObjectRb = pickedObject.GetComponent<Rigidbody>();
            Collider pickedObjectCollider = pickedObject.GetComponent<Collider>();

            if (pickedObjectRb != null)
            {
                pickedObjectRb.useGravity = false;
                pickedObjectRb.isKinematic = false;
            }

            if (pickedObjectCollider != null)
            {
                Collider[] playerColliders = GameObject.FindGameObjectsWithTag("Player")
                                                      .Select(go => go.GetComponent<Collider>())
                                                      .Where(c => c != null)
                                                      .ToArray();
                foreach (var playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(pickedObjectCollider, playerCollider, true);
                }
            }

            pickedObject.SetParent(cameraTransform);
            pickedObject.localPosition = new Vector3(0, 0, pickupDistance);
        }
    }

    void UpdateCrosshairVisibility()
    {
        if (crosshair != null)
        {
            if (pickedObject != null)
            {
                crosshair.SetActive(false);
                return;
            }

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupDistance, interactableLayer))
            {
                crosshair.SetActive(false);
            }
            else
            {
                crosshair.SetActive(true);
            }
        }
    }

    void DropObject()
    {
        if (pickedObject != null)
        {
            if (pickedObjectRb != null)
            {
                pickedObjectRb.useGravity = true;
                pickedObjectRb.isKinematic = false;
            }

            Collider pickedObjectCollider = pickedObject.GetComponent<Collider>();
            if (pickedObjectCollider != null)
            {
                Collider[] playerColliders = GameObject.FindGameObjectsWithTag("Player")
                                                      .Select(go => go.GetComponent<Collider>())
                                                      .Where(c => c != null)
                                                      .ToArray();
                foreach (var playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(pickedObjectCollider, playerCollider, false);
                }
            }

            pickedObject.SetParent(null);
            pickedObject = null;
            pickedObjectRb = null;
        }
    }

    void FixedUpdate()
    {
        if (pickedObject != null && pickedObjectRb != null)
        {
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * pickupDistance;
            Vector3 direction = targetPosition - pickedObject.position;
            pickedObjectRb.velocity = direction * 10f;
        }
    }

    void OnDrawGizmos()
    {
        if (cameraTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * pickupDistance);
        }
    }

    void UpdateObjectGlow()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, pickupDistance, interactableLayer);

        if (pickedObject == null && isHit)
        {
            Renderer objectRenderer = hit.transform.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                // If the object is different from the last highlighted one, restore the last one
                if (lastHighlightedRenderer != null && lastHighlightedRenderer != objectRenderer)
                {
                    RestoreOriginalMaterial(lastHighlightedRenderer);
                }

                // Get current materials
                Material[] currentMaterials = objectRenderer.materials;

                // Check if the glow material is already added
                if (currentMaterials.Length == 1 || !currentMaterials.Contains(glowMaterial))
                {
                    // Store the original material if not already stored
                    if (originalMaterial == null)
                    {
                        originalMaterial = currentMaterials[0];
                    }

                    // Create a new array with the original and glow material
                    Material[] newMaterials = new Material[2];
                    newMaterials[0] = originalMaterial;
                    newMaterials[1] = glowMaterial;

                    // Assign the new materials array back to the renderer
                    objectRenderer.materials = newMaterials;
                }

                // Update the last highlighted renderer
                lastHighlightedRenderer = objectRenderer;
            }
        }
        else if (pickedObject == null && lastHighlightedRenderer != null)
        {
            // Restore the original material of the last highlighted object
            RestoreOriginalMaterial(lastHighlightedRenderer);
            lastHighlightedRenderer = null;
        }
        // If an object is picked, ensure it retains the glow material
        else if (pickedObject != null)
        {
            Renderer objectRenderer = pickedObject.GetComponent<Renderer>();
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
        if (renderer != null && originalMaterial != null)
        {
            renderer.materials = new Material[] { originalMaterial };
            originalMaterial = null; // Clear the stored original material
        }
    }
}