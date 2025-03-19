using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float worldScale = 1.0f; // Desired world scale
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
        }

        // Apply consistent world scale directly, ignoring parent's scale
        transform.localScale = originalLocalScale * worldScale;
    }
}
