using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float referenceDistance = 10.0f; // Distance at which the marker is at normal scale
    [SerializeField] private float screenSize = 2.0f; // Desired size on screen
    [SerializeField] private Transform designatedParent; // Optional parent to follow
    private Vector3 originalLocalScale;
    private bool hasBeenParented = false;

    void Start()
    {
        // Store initial values
        mainCamera = Camera.main;
        originalLocalScale = transform.localScale;
        
        // Apply optional parenting after initial values are recorded
        if (designatedParent != null && !hasBeenParented)
        {
            transform.SetParent(designatedParent, true); // Keep world position
            hasBeenParented = true;
        }
    }

    void Update()
    {
        // Billboard effect - make the object face the camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
            
            // Calculate distance from camera to this object
            float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
            
            // Scale based on distance - objects further away get proportionally larger
            float scaleFactor = distanceToCamera / referenceDistance * screenSize;
            
            // Apply the scale
            transform.localScale = originalLocalScale * scaleFactor;
        }
    }
}
