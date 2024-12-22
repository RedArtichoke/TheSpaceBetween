using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup; 

    private void Start()
    {
        text.gameObject.SetActive(false);
        StartCoroutine(TextAndImageRoutine());
    }

    private IEnumerator TextAndImageRoutine()
    {
        yield return new WaitForSeconds(5f);

        text.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

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
        
        canvasGroup.alpha = 0f;
    }
}

