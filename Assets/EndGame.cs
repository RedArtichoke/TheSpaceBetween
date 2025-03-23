using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public CanvasGroup blackScreen;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        yield return new WaitForSeconds (1f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        blackScreen.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            blackScreen.alpha = alpha;
            yield return null;  
        }
    }
}
