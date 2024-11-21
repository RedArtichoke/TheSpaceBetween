using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health = 100;
    public bool isDamaged = false;
    public Image damageOverlay;
    private Coroutine strobeCoroutine;
    private Coroutine regenCoroutine;

    void Start()
    {
        damageOverlay.color = new Color(1f, 0f, 0f, 0f); // Start transparent
    }

    public void DamagePlayer()
    {
        if (!isDamaged)
        {
            health -= 70;
            if (health < 0)
            {
                health = 0;
            }
            isDamaged = true;
            StartCoroutine(DamageCooldown());
            StartCoroutine(ScreenShake());

            if (health < 50)
            {
                if (strobeCoroutine != null)
                {
                    StopCoroutine(strobeCoroutine);
                }
                strobeCoroutine = StartCoroutine(StrobeEffect());
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(2f);
        isDamaged = false;
        yield return new WaitForSeconds(15f);

        if (health < 100)
        {
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (health < 100)
        {
            health += 1;
            float alpha = Mathf.Lerp(0.5f, 0f, health / 100f);
            damageOverlay.color = new Color(1f, 0f, 0f, alpha);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ScreenShake()
    {
        float duration = 0.4f;
        float initialMagnitude = 0.6f;
        float magnitude = initialMagnitude;
        Vector3 originalPosition = Camera.main.transform.position;

        float elapsed = 0.0f;
        float velocity = 0.0f;

        while (elapsed < duration)
        {
            magnitude = Mathf.SmoothDamp(magnitude, 0f, ref velocity, duration - elapsed);
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = originalPosition;
    }

    private IEnumerator StrobeEffect()
    {
        while (health < 50)
        {
            float speed = Mathf.Lerp(0.5f, 1.5f, (50f - health) / 50f); // Slower strobe across the board
            float alpha = Mathf.PingPong(Time.time * speed, 0.5f) * ((50f - health) / 50f);
            damageOverlay.color = new Color(1f, 0f, 0f, alpha);

            yield return null; // Update every frame for smooth strobing
        }
    }
}
