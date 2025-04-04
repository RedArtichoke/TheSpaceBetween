using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class YogurtSplashPage : MonoBehaviour
{
    public Image Yogurt;
    public CanvasGroup canvasGroup;
    public GameObject menuCanvas;
    public AudioSource audioPlay;

    void Start()
    {
        StartCoroutine(YogurtLoad());
    }   

    public IEnumerator YogurtLoad()
    {
        yield return new WaitForSeconds(2f);

        Yogurt.gameObject.SetActive(true);
        
        StartCoroutine(FadeInImage(Yogurt, 2f));
        //computerSound.clip = computerclip2;
        
        yield return new WaitForSeconds(1f);

        audioPlay.Play();
        
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(FadeOutImage(Yogurt, 1f));

        yield return new WaitForSeconds(2f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        canvasGroup.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;  
        }

        menuCanvas.SetActive(true);

        gameObject.SetActive(false);
    }

    IEnumerator FadeInImage(Image image, float duration)
    {
        Color originalColor = image.color;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Start fully transparent

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Ensure fully visible
    }

    IEnumerator FadeOutImage(Image image, float duration)
    {
        Color originalColor = image.color;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Start fully visible

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure fully transparent
    }
}
