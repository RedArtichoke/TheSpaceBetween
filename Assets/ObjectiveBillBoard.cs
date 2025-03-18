using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float worldScale = 1.0f; // Desired world scale
    private Vector3 originalLocalScale;
    private Transform parentTransform;

    void Start()
    {
        mainCamera = Camera.main;
        originalLocalScale = transform.localScale;
        parentTransform = transform.parent;
    }

    void Update()
    {
        // Billboard effect - make the object face the camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }

        // Adjust scale to maintain consistent world scale
        if (parentTransform != null)
        {
            Vector3 parentWorldScale = new Vector3(
                parentTransform.lossyScale.x,
                parentTransform.lossyScale.y,
                parentTransform.lossyScale.z
            );

            transform.localScale = new Vector3(
                originalLocalScale.x * worldScale / parentWorldScale.x,
                originalLocalScale.y * worldScale / parentWorldScale.y,
                originalLocalScale.z * worldScale / parentWorldScale.z
            );
        }
    }
}
