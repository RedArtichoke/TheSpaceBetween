using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextFadeIn : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }   

    private void OnEnable()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        Color startColor = text.color;
        startColor.a = 0f;
        text.color = startColor;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            Color newColor = text.color;
            newColor.a = Mathf.Lerp(0f, 1f, t);
            text.color = newColor;

            yield return null;
        }

        Color finalColor = text.color;
        finalColor.a = 1f;
        text.color = finalColor;
    }

    public void DisableText()
    {
        StartCoroutine(FadeOut());
    }
    
    public IEnumerator FadeOut()
    {
        Color startColor = text.color;
        startColor.a = 1f;
        text.color = startColor;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            Color newColor = text.color;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            text.color = newColor;

            yield return null;
        }

        Color finalColor = text.color;
        finalColor.a = 0f;
        text.color = finalColor;
    }
}
