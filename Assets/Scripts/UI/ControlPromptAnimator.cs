using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles animation of control prompts from screen center to their designated UI positions
/// </summary>
public class ControlPromptAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float delayBeforeAnimation = 4.0f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float initialScale = 1.2f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float verticalOffset = 100f; // Offset to raise elements
    [SerializeField] private float textHorizontalAdjustment = 0f;
    
    [Header("Debugging")]
    [SerializeField] private bool debugMode = true;

    private CanvasGroup canvasGroup;
    private Coroutine animationCoroutine;
    
    // Store data about each child
    private class ChildData
    {
        public RectTransform rectTransform;
        public Vector2 originalAnchoredPosition;
        public Vector3 originalScale;
        public Vector2 offsetFromCenter;
    }
    
    private List<ChildData> childrenData = new List<ChildData>();
    private bool isInitialized = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        StoreChildrenData();
        gameObject.SetActive(false);
    }
    
    private void StoreChildrenData()
    {
        childrenData.Clear();
        
        // Calculate bounds to find the center of all children
        Bounds bounds = new Bounds();
        bool boundsInitialized = false;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform childRect = transform.GetChild(i).GetComponent<RectTransform>();
            if (childRect != null)
            {
                // Convert anchored position to world position for bounds calculation
                Vector3 worldPos = childRect.TransformPoint(Vector3.zero);
                
                if (!boundsInitialized)
                {
                    bounds = new Bounds(worldPos, Vector3.zero);
                    boundsInitialized = true;
                }
                else
                {
                    bounds.Encapsulate(worldPos);
                }
            }
        }
        
        Vector3 centerPoint = bounds.center;
        if (debugMode) Debug.Log($"[{gameObject.name}] Group center: {centerPoint}");
        
        // Store data for each child
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            RectTransform childRect = child.GetComponent<RectTransform>();
            
            if (childRect != null)
            {
                Vector3 worldPos = childRect.TransformPoint(Vector3.zero);
                Vector3 offsetFromCenter = worldPos - centerPoint;
                
                ChildData data = new ChildData
                {
                    rectTransform = childRect,
                    originalAnchoredPosition = childRect.anchoredPosition,
                    originalScale = childRect.localScale,
                    offsetFromCenter = new Vector2(offsetFromCenter.x, offsetFromCenter.y)
                };
                
                childrenData.Add(data);
                
                if (debugMode)
                {
                    Debug.Log($"[{gameObject.name}] Child {child.name}: Original pos: {data.originalAnchoredPosition}, Offset from center: {data.offsetFromCenter}");
                }
            }
        }
        
        isInitialized = true;
    }

    /// <summary>
    /// Reveals the prompt with animation from screen center to target position
    /// </summary>
    public void RevealWithAnimation()
    {
        if (debugMode) Debug.Log($"[{gameObject.name}] RevealWithAnimation called");
        
        if (!isInitialized)
        {
            StoreChildrenData();
        }
        
        // Stop any ongoing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Show the canvas
        gameObject.SetActive(true);
        
        // Position all children at screen center while maintaining relative positioning
        PositionChildrenAtCenter();
        
        // Start the animation
        animationCoroutine = StartCoroutine(AnimateChildren());
    }
    
    private void PositionChildrenAtCenter()
    {
        foreach (ChildData data in childrenData)
        {
            // Calculate center position for this child based on its offset
            Vector2 centerPosition = new Vector2(
                data.offsetFromCenter.x, 
                data.offsetFromCenter.y + verticalOffset
            );
            
            // Special handling for text elements
            if (data.rectTransform.GetComponent<Text>() != null || 
                data.rectTransform.GetComponent<TMPro.TextMeshProUGUI>() != null)
            {
                centerPosition.x += textHorizontalAdjustment;
            }
            
            // Apply special positioning for elements by name
            if (data.rectTransform.name.Contains("Instructions") || data.rectTransform.name.Contains("Text"))
            {
                // Apply your custom adjustment
                centerPosition.x += 60f; // Adjust as needed
            }
            
            // Apply centered position and initial scale
            data.rectTransform.anchoredPosition = centerPosition;
            data.rectTransform.localScale = data.originalScale * initialScale;
            
            if (debugMode)
            {
                Debug.Log($"[{gameObject.name}] Child {data.rectTransform.name}: Centered position with offset: {centerPosition}");
            }
        }
    }
    
    private IEnumerator AnimateChildren()
    {
        if (debugMode) Debug.Log($"[{gameObject.name}] Animation started");
        
        // Fade in
        canvasGroup.alpha = 0;
        float elapsed = 0;
        while (elapsed < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        
        // Add delay before animation starts to allow reading
        if (debugMode) Debug.Log($"[{gameObject.name}] Pausing for {delayBeforeAnimation} seconds to allow reading");
        yield return new WaitForSeconds(delayBeforeAnimation);
        
        if (debugMode) Debug.Log($"[{gameObject.name}] Starting movement animation");
        
        // Store starting positions
        Vector2[] startingPositions = new Vector2[childrenData.Count];
        for (int i = 0; i < childrenData.Count; i++)
        {
            startingPositions[i] = childrenData[i].rectTransform.anchoredPosition;
        }
        
        // Animate position and scale of all children
        elapsed = 0;
        while (elapsed < animationDuration)
        {
            float t = animationCurve.Evaluate(elapsed / animationDuration);
            
            for (int i = 0; i < childrenData.Count; i++)
            {
                ChildData childData = childrenData[i];
                Vector2 startPos = startingPositions[i];
                
                // Animate from centered to original position
                childData.rectTransform.anchoredPosition = Vector2.Lerp(
                    startPos, 
                    childData.originalAnchoredPosition,
                    t
                );
                
                // Animate from enlarged scale to original scale
                childData.rectTransform.localScale = Vector3.Lerp(
                    childData.originalScale * initialScale,
                    childData.originalScale,
                    t
                );
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we end at exactly the target values
        foreach (ChildData childData in childrenData)
        {
            childData.rectTransform.anchoredPosition = childData.originalAnchoredPosition;
            childData.rectTransform.localScale = childData.originalScale;
        }
        
        if (debugMode) Debug.Log($"[{gameObject.name}] Animation completed");
        
        animationCoroutine = null;
    }

    /// <summary>
    /// Shows the prompt instantly at its target position without animation
    /// </summary>
    public void ShowInstantly()
    {
        if (debugMode) Debug.Log($"[{gameObject.name}] ShowInstantly called");
        
        if (!isInitialized)
        {
            StoreChildrenData();
        }
        
        // Stop any ongoing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Show the canvas
        gameObject.SetActive(true);
        
        // Reset all children to their original positions
        foreach (ChildData childData in childrenData)
        {
            childData.rectTransform.anchoredPosition = childData.originalAnchoredPosition;
            childData.rectTransform.localScale = childData.originalScale;
        }
        
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Hides the prompt
    /// </summary>
    public void Hide()
    {
        if (debugMode) Debug.Log($"[{gameObject.name}] Hide called");
        
        // Stop any ongoing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Hide the canvas
        gameObject.SetActive(false);
    }
} 