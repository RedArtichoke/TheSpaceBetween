using UnityEngine;

/// <summary>
/// Assigns the main camera to the canvas at runtime, allowing blur.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UIBlurCameraAssigner : MonoBehaviour
{
    void Start()
    {
        // Fetch and assign the main camera automatically
        Canvas canvas = GetComponent<Canvas>();
        Camera uiCamera = Camera.main;
        canvas.worldCamera = uiCamera;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 100;

        // Ensure the UI camera has a higher depth
        uiCamera.depth = 1; // Set higher than other cameras
    }
}