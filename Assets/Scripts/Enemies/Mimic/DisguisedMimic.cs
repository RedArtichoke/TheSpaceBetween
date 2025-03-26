using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisguisedMimic : MonoBehaviour
{
    // Reference to the original mimic
    public GameObject originalMimic;
    //public bool isTutorialMimic;

    private void OnEnable()
    {
        StartCoroutine(ShakeEffect());
    }

    private IEnumerator ShakeEffect()
    {
        while (true)
        {
            bool isBouncing = Random.value > 0.5f;

            if (isBouncing)
            {
                // Bounce effect
                Vector3 originalPosition = transform.position;
                float initialBounceHeight = 0.2f; 
                float gravity = -9.8f; // Simulate gravity
                int bounces = 3; 

                for (int i = 0; i < bounces; i++)
                {
                    float bounceHeight = initialBounceHeight * Mathf.Pow(0.5f, i); // Reduce height each bounce
                    float velocity = Mathf.Sqrt(-2 * gravity * bounceHeight); // Initial velocity for the bounce
                    float elapsedTime = 0f;

                    while (velocity > 0 || transform.position.y > originalPosition.y)
                    {
                        float displacement = velocity * Time.deltaTime + 0.5f * gravity * Mathf.Pow(Time.deltaTime, 2);
                        transform.position += Vector3.up * displacement;
                        velocity += gravity * Time.deltaTime;
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    // Ensure it returns to the original position
                    transform.position = new Vector3(transform.position.x, originalPosition.y, transform.position.z);
                }
            }
            else
            {
                // Shake effect
                Vector3 originalPosition = transform.position;
                float shakeAmount = 0.05f;
                float shakeDuration = 0.5f;

                float elapsedTime = 0f;
                while (elapsedTime < shakeDuration)
                {
                    transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Return to original position
                transform.position = originalPosition;
            }

            // Wait for a random delay before repeating
            yield return new WaitForSeconds(Random.Range(10f, 20f));
        }
    }
}
