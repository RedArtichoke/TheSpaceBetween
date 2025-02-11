using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplatEffect : MonoBehaviour
{
    public GameObject splatImage;

    public AudioSource splatSound;

    public void ShowSplat()
    {
        StartCoroutine(SplatTimer());
    }

    public IEnumerator SplatTimer()
    {
        splatImage.SetActive(true);
        splatSound.Play();

        yield return new WaitForSeconds(3f);

        Image image = splatImage.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("No Image component found on splatImage!");
            yield break;
        }

        float fadeDuration = 1f; 
        float elapsedTime = 0f;
        Color originalColor = image.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null; 
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        splatImage.SetActive(false);
    }
}
