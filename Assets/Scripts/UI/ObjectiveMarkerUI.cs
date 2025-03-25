using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Projects world-space objective markers onto a UI canvas as 2D images
/// Dynamically creates and manages UI elements that track the position of 
/// all game objects tagged with "ObjectiveMarker"
/// </summary>
public class ObjectiveMarkerUI : MonoBehaviour 
{
    [Header("Marker Settings")]
    [Tooltip("The sprite to use for the marker UI elements")]
    [SerializeField] private Sprite markerSprite;
    
    [Tooltip("The colour to apply to the marker UI elements")]
    [SerializeField] private Color markerColor = Color.white;
    
    [Tooltip("Scale factor for on-screen UI markers")]
    [SerializeField] private float markerScale = 1f;
    
    [Tooltip("Scale factor for off-screen UI markers (at screen edges)")]
    [SerializeField] private float offScreenMarkerScale = 1.2f;
    
    [Header("Movement Settings")]
    [Tooltip("Offset from screen edges for off-screen markers")]
    [SerializeField] private float screenEdgeOffset = 30f;

    [Tooltip("If true, disables smoothing for more precise positioning")]
    [SerializeField] private bool precisePositioning = true;
    
    [Header("Position Adjustment")]
    [Tooltip("Fine-tune X position if markers appear offset")]
    [SerializeField] private float adjustmentOffsetX = 0f;
    
    [Tooltip("Fine-tune Y position if markers appear offset")]
    [SerializeField] private float adjustmentOffsetY = 0f;
    
    [Tooltip("Distance-based correction factor - try values between 0-1 if markers are offset")]
    [SerializeField] private float distanceCorrectionFactor = 0f;
    
    [Header("References")]
    [Tooltip("Reference to the main camera (will find automatically if null)")]
    [SerializeField] private Camera mainCamera;
    
    // Dictionary to track active markers and their corresponding UI elements
    private Dictionary<GameObject, GameObject> markerToUIElements = new Dictionary<GameObject, GameObject>();
    
    // Cache transform for efficiency
    private RectTransform canvasRectTransform;
    
    // Reference to Canvas component for proper scaling calculation
    private Canvas canvas;
    
    // Whether initialization has completed
    private bool isInitialized = false;
    
    // Whether markers are currently visible
    private bool markersVisible = true;
    
    private void Start()
    {
        InitializeSystem();
    }
    
    private void InitializeSystem()
    {
        // Get canvas rect transform
        canvasRectTransform = GetComponent<RectTransform>();
        if (canvasRectTransform == null)
        {
            Debug.LogError("ObjectiveMarkerUI must be attached to a GameObject with a RectTransform component (like a Canvas)!");
            enabled = false;
            return;
        }
        
        // Get Canvas component
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("ObjectiveMarkerUI must be attached to a GameObject with a Canvas component!");
            enabled = false;
            return;
        }
        
        // Find main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No main camera found in the scene!");
                enabled = false;
                return;
            }
        }
        
        // Find all objective markers in the scene and set up UI elements for them
        GameObject[] objectiveMarkers = GameObject.FindGameObjectsWithTag("ObjectiveMarker");
        foreach (GameObject marker in objectiveMarkers)
        {
            CreateUIElementForMarker(marker);
        }
        
        // Start checking for new markers
        StartCoroutine(CheckForMarkerChanges());
        
        isInitialized = true;
    }
    
    private void CreateUIElementForMarker(GameObject marker)
    {
        // Don't create duplicates or handle null markers
        if (marker == null || markerToUIElements.ContainsKey(marker))
            return;
            
        // Create a new UI GameObject
        GameObject uiElement = new GameObject("MarkerUI_" + marker.name);
        
        // Add as child of the canvas (this is important for UI elements to work properly)
        uiElement.transform.SetParent(transform, false);
        
        // Add a RectTransform component (required for UI positioning)
        RectTransform rectTransform = uiElement.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(32, 32);  // Default size
        rectTransform.localScale = Vector3.one * markerScale;
        
        // Add an Image component
        Image image = uiElement.AddComponent<Image>();
        image.color = markerColor;
        
        // Set the sprite if provided
        if (markerSprite != null)
        {
            image.sprite = markerSprite;
        }
        else
        {
            // Try to load a built-in sprite
            Sprite builtInSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            if (builtInSprite != null)
            {
                image.sprite = builtInSprite;
            }
        }
        
        // Set visibility based on current state
        uiElement.SetActive(markersVisible);
        
        // Add to tracking dictionary
        markerToUIElements.Add(marker, uiElement);
        
        // Position immediately
        UpdateMarkerPosition(marker, uiElement);
    }
    
    // Using FixedUpdate for more consistent results with animated cameras
    private void FixedUpdate()
    {
        if (!isInitialized)
            return;
            
        // Process all markers
        UpdateAllMarkers();
    }
    
    private void UpdateAllMarkers()
    {
        // Update each UI element's position based on its corresponding world marker
        List<GameObject> markersToRemove = new List<GameObject>();
        
        foreach (var kvp in markerToUIElements)
        {
            GameObject marker = kvp.Key;
            GameObject uiElement = kvp.Value;
            
            // If the marker or UI element is destroyed or inactive, mark it for removal
            if (marker == null || uiElement == null || !marker.activeInHierarchy)
            {
                markersToRemove.Add(marker);
                continue;
            }
            
            // Update this marker's position
            UpdateMarkerPosition(marker, uiElement);
        }
        
        // Remove any destroyed or inactive markers
        foreach (GameObject marker in markersToRemove)
        {
            if (markerToUIElements.TryGetValue(marker, out GameObject uiElement) && uiElement != null)
            {
                Destroy(uiElement);
            }
            markerToUIElements.Remove(marker);
        }
    }
    
    private void UpdateMarkerPosition(GameObject marker, GameObject uiElement)
    {
        if (marker == null || uiElement == null || !marker.activeInHierarchy)
            return;
            
        // Get the RectTransform (required for UI positioning)
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;
            
        // Get marker position in world space
        Vector3 worldPosition = marker.transform.position;
        
        // Calculate distance from camera to marker
        float distanceToCamera = Vector3.Distance(mainCamera.transform.position, worldPosition);
        
        // Project directly to screen space with proper z-coordinate handling
        Vector3 screenPosition;
        
        // Alternative method for projection
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(worldPosition);
        screenPosition = new Vector3(
            viewportPosition.x * Screen.width,
            viewportPosition.y * Screen.height,
            viewportPosition.z
        );
        
        bool isBehind = screenPosition.z < 0;
        
        // Handle marker behind camera
        if (isBehind)
        {
            // If behind camera, invert the position to get the right direction
            screenPosition.x = Screen.width - screenPosition.x;
            screenPosition.y = Screen.height - screenPosition.y;
            screenPosition.z = 0;
        }
        
        // Define screen bounds with edge offset
        float minX = screenEdgeOffset;
        float maxX = Screen.width - screenEdgeOffset;
        float minY = screenEdgeOffset;
        float maxY = Screen.height - screenEdgeOffset;
        
        // Determine if marker is on-screen
        bool isOffScreen = isBehind || 
                          screenPosition.x < minX || 
                          screenPosition.x > maxX || 
                          screenPosition.y < minY || 
                          screenPosition.y > maxY;
        
        if (isOffScreen)
        {
            // Calculate screen center
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            
            // Get direction from center to the marker
            Vector2 directionFromCenter = new Vector2(screenPosition.x, screenPosition.y) - screenCenter;
            
            // If direction is too small, default to up direction
            if (directionFromCenter.sqrMagnitude < 0.01f)
            {
                directionFromCenter = Vector2.up;
            }
            
            // Normalize the direction
            directionFromCenter.Normalize();
            
            // Calculate the intersection with screen edges
            float angle = Mathf.Atan2(directionFromCenter.y, directionFromCenter.x);
            
            // Calculate actual screen bounds accounting for the edge offset
            float boundsWidth = Screen.width - (2 * screenEdgeOffset);
            float boundsHeight = Screen.height - (2 * screenEdgeOffset);
            
            // Find intersection point with screen edge
            float halfBoundsWidth = boundsWidth * 0.5f;
            float halfBoundsHeight = boundsHeight * 0.5f;
            
            float xAtEdge, yAtEdge;
            
            // Check for horizontal edges (top/bottom)
            if (Mathf.Abs(Mathf.Tan(angle)) > halfBoundsHeight / halfBoundsWidth)
            {
                // Will intersect with top or bottom edge
                yAtEdge = (directionFromCenter.y > 0) ? halfBoundsHeight : -halfBoundsHeight;
                xAtEdge = yAtEdge / Mathf.Tan(angle);
            }
            else
            {
                // Will intersect with left or right edge
                xAtEdge = (directionFromCenter.x > 0) ? halfBoundsWidth : -halfBoundsWidth;
                yAtEdge = Mathf.Tan(angle) * xAtEdge;
            }
            
            // Adjust position to the screen edge
            screenPosition.x = screenCenter.x + xAtEdge;
            screenPosition.y = screenCenter.y + yAtEdge;
        }
        
        // Convert screen position to canvas position using a direct approach for precise control
        Vector2 canvasPosition;
        
        // Direct calculation for overlay mode (most common)
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Convert directly to canvas space
            float canvasSizeX = canvasRectTransform.rect.width;
            float canvasSizeY = canvasRectTransform.rect.height;
            
            canvasPosition = new Vector2(
                ((screenPosition.x / Screen.width) * canvasSizeX) - (canvasSizeX * 0.5f),
                ((screenPosition.y / Screen.height) * canvasSizeY) - (canvasSizeY * 0.5f)
            );
        }
        else
        {
            // For other modes, use Unity's utility
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                screenPosition,
                canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null,
                out canvasPosition))
            {
                canvasPosition = Vector2.zero;
            }
        }
        
        // Apply distance-based correction if enabled
        if (distanceCorrectionFactor != 0f)
        {
            // Calculate correction based on distance
            float correctionMultiplier = distanceToCamera * distanceCorrectionFactor;
            
            // Direction from center of screen to the marker
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 direction = new Vector2(screenPosition.x, screenPosition.y) - screenCenter;
            direction.Normalize();
            
            // Apply correction in the direction of the marker
            canvasPosition.x += direction.x * correctionMultiplier;
            canvasPosition.y += direction.y * correctionMultiplier;
        }
        
        // Apply position immediately without smoothing for precise positioning with animated cameras
        if (precisePositioning)
        {
            // Apply any adjustment offsets
            canvasPosition.x += adjustmentOffsetX;
            canvasPosition.y += adjustmentOffsetY;
            rectTransform.anchoredPosition = canvasPosition;
        }
        else
        {
            // Apply any adjustment offsets
            canvasPosition.x += adjustmentOffsetX;
            canvasPosition.y += adjustmentOffsetY;
            
            // Simple smoothing if precise positioning is not needed
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                canvasPosition,
                Time.deltaTime * 10f
            );
        }
        
        // Apply static scaling based on whether marker is on-screen or off-screen
        if (!isOffScreen)
        {
            // Use regular scale for on-screen markers
            rectTransform.localScale = Vector3.one * markerScale;
        }
        else
        {
            // Use a fixed scale for off-screen markers
            rectTransform.localScale = Vector3.one * offScreenMarkerScale;
        }
    }
    
    // Periodically check for new markers that might be added during gameplay
    private IEnumerator CheckForMarkerChanges()
    {
        while (true)
        {
            // Wait for a few seconds between checks
            yield return new WaitForSeconds(2f);
            
            // Find all current objective markers
            GameObject[] objectiveMarkers = GameObject.FindGameObjectsWithTag("ObjectiveMarker");
            
            // Check for new markers
            foreach (GameObject marker in objectiveMarkers)
            {
                if (!markerToUIElements.ContainsKey(marker))
                {
                    CreateUIElementForMarker(marker);
                }
            }
        }
    }
    
    /// <summary>
    /// Set visibility of all marker UI elements
    /// </summary>
    /// <param name="visible">Whether markers should be visible</param>
    public void SetMarkersVisibility(bool visible)
    {
        markersVisible = visible;
        
        foreach (var kvp in markerToUIElements)
        {
            GameObject uiElement = kvp.Value;
            
            if (uiElement != null)
            {
                uiElement.SetActive(visible);
            }
        }
    }
    
    /// <summary>
    /// Toggle visibility of all marker UI elements
    /// </summary>
    public void ToggleMarkersVisibility()
    {
        SetMarkersVisibility(!markersVisible);
    }
} 