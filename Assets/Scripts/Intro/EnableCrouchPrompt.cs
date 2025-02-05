using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCrouchPrompt : MonoBehaviour
{
    public GameObject CrouchPrompt;
    public CanvasGroup CrouchGroup;
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            CrouchPrompt.SetActive(true);

            StartCoroutine(ShowCrouchPrompt());

            GetComponent<Collider>().enabled = false;
        }
    }

        public IEnumerator ShowCrouchPrompt()
        {
            yield return new WaitForSeconds (8f);

            float fadeDuration = 1f;
            float elapsedTime = 0f;
        
            CrouchGroup.alpha = 1f; 

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                CrouchGroup.alpha = alpha;
                yield return null;  
            }
        
            CrouchGroup.alpha = 0f;

            CrouchPrompt.SetActive(false);


        }
}

