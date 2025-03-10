using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSuitUIAnim : MonoBehaviour
{
    public TextMeshProUGUI loadingText; // Assign in Inspector
    private Coroutine loadingCoroutine;

    private void OnEnable()
    {
        loadingCoroutine = StartCoroutine(AnimateLoadingText());
    }

    private void OnDisable()
    {
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }
    }

    private IEnumerator AnimateLoadingText()
    {
        string baseText = "System Startup";
        int dotCount = 0;

        while (true)
        {
            loadingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Cycles between 0, 1, 2, 3 dots
            yield return new WaitForSeconds(0.5f);
        }
    }
}
