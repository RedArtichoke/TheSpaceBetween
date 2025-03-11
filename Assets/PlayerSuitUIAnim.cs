using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSuitUIAnim : MonoBehaviour
{
    public TextMeshProUGUI loadingText; 
    public TextMeshProUGUI statusText1; 
    public TextMeshProUGUI statusText2; 
    public string[] loadingMessages = {
        @"C:\RAMP\system\addons\v1950\deosse.exe",
        @"C:\RAMP\system\sensors\testing.exe",
        @"C:\RAMP\security\firewall\scan.exe",
        @"C:\RAMP\diagnostics\sys_check.exe",
        @"C:\RAMP\system\drivers\update.exe",
        @"C:\RAMP\core\boot\loader.exe",
        @"C:\RAMP\hardware\gpu\benchmark.exe",
        @"C:\RAMP\logs\error_report.exe",
        @"C:\RAMP\system\updates\patch_04.exe",
        @"C:\RAMP\network\connection_check.exe",
        @"C:\RAMP\monitoring\status_logger.exe",
        @"C:\RAMP\system\backup\restore.exe",
        @"C:\RAMP\scripts\cleanup.bat",
        @"C:\RAMP\debug\crash_handler.exe",
        @"C:\RAMP\utilities\file_scan.exe",
        @"C:\RAMP\database\indexer.exe",
        @"C:\RAMP\security\malware_scan.exe",
        @"C:\RAMP\system\diagnostics\cpu_test.exe",
        @"C:\RAMP\user\profile_sync.exe",
        @"C:\RAMP\automation\task_scheduler.exe",
        @"C:\RAMP\tools\compression_tool.exe",
        @"C:\RAMP\emulation\virtual_env.exe",
        @"C:\RAMP\system\addons\v1950\deosse.exe",
        @"C:\RAMP\system\sensors\testing.exe",
        @"C:\RAMP\security\firewall\scan.exe",
        @"C:\RAMP\diagnostics\sys_check.exe",
        @"C:\RAMP\system\drivers\update.exe",
        @"C:\RAMP\core\boot\loader.exe",
        @"C:\RAMP\hardware\gpu\benchmark.exe",
        @"C:\RAMP\logs\error_report.exe"
    };


    private Coroutine loadingCoroutine;
    private Coroutine listCoroutine;
    private int dotCount = 0;
    private int messageIndex = 0;
    public CanvasGroup canvasGroup;

    private void OnEnable()
    {
        loadingCoroutine = StartCoroutine(AnimateLoadingText());
        listCoroutine = StartCoroutine(AnimateStatusList());
        StartCoroutine(LoadText());
    }

    private void OnDisable()
    {
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }
        if (listCoroutine != null)
        {
            StopCoroutine(listCoroutine);
            listCoroutine = null;
        }
    }
    private IEnumerator LoadText()
    {
        float fadeDuration = 4f;
        float elapsedTime = 0f;
        
        canvasGroup.alpha = 0f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;  
        }
        
        canvasGroup.alpha = 1f;

    }
    private IEnumerator AnimateLoadingText()
    {
        string baseText = "System Startup";
        int dotCount = 0;

        while (true)
        {
            loadingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; 
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator AnimateStatusList()
    {
        while (true)
        {
            string newMessage = loadingMessages[messageIndex % loadingMessages.Length];
            messageIndex++;

            //statusText1.rectTransform.anchoredPosition = Vector2.zero;
            //statusText2.rectTransform.anchoredPosition = Vector2.zero;
            statusText2.text = newMessage;
            statusText2.color = new Color(1, 1, 1, 0); 
            StartCoroutine(FadeInText(statusText2));

            yield return new WaitForSeconds(0.5f);

            statusText1.text = statusText2.text;
            statusText1.color = new Color(1, 1, 1, 1); 
            statusText2.text = ""; 

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI text)
    {
        float duration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.color = new Color(1, 1, 1, elapsedTime / duration);
            yield return null;
        }
    }
}
