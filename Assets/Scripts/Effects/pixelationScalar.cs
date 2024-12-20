using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicScaler : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateScale();
    }

    public void UpdateScale()
    {
        // Calculate the aspect ratio
        float targetAspect = 1920f / 1080f;
        float screenAspect = (float)Screen.width / Screen.height;

        // Determine the scale factor to cover the screen
        float scaleFactor = screenAspect > targetAspect 
            ? (float)Screen.width / 1920f 
            : (float)Screen.height / 1080f;

        // Set the size to maintain the aspect ratio and cover the screen
        rectTransform.sizeDelta = new Vector2(1920f * scaleFactor, 1080f * scaleFactor);

        // Centre the content
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
