using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public float health = 100;
    public bool isDamaged = false;
    public Image damageOverlay;
    private Coroutine strobeCoroutine;
    private Coroutine regenCoroutine;
    private HeartRateAnimator heartRateAnimator;

    void Start()
    {
        damageOverlay.color = new Color(1f, 0f, 0f, 0f);
        heartRateAnimator = GameObject.Find("heartrate").GetComponent<HeartRateAnimator>();
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

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
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
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        float targetHealth = 100f;
        float smoothTime = 2f;
        float velocity = 0f;

        while (health < targetHealth)
        {
            health = Mathf.SmoothDamp(health, targetHealth, ref velocity, smoothTime);

            if (health > targetHealth)
            {
                health = targetHealth;
            }

            yield return null;
        }
    }

    private IEnumerator StrobeEffect()
    {
        while (health < 100)
        {
            float beatsPerMinute = heartRateAnimator.beatsPerMinute;
            float speed = beatsPerMinute / 60f;
            float maxAlpha = (100f - health) / 100f;
            float strobeAlpha = Mathf.SmoothStep(0f, maxAlpha, Mathf.PingPong(Time.time * speed, 1f));
            
            damageOverlay.color = new Color(1f, 0f, 0f, strobeAlpha);

            yield return null;
        }
    }

    private IEnumerator ScreenShake()
    {
        // Screen shake logic here
        yield return null;
    }
}
