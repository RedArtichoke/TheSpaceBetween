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
    private FpsCameraController cameraController;
    public GameObject gameOverUI;
    public GameObject UIBlur;

    public AudioClip[] damageSounds; // Array of audio clips for damage
    private AudioSource audioSource; // AudioSource component

    public SuitVoice suitVoice;

    private HeartRateSimulator heartRateSimulator;

    void Start()
    {
        heartRateSimulator = GameObject.FindWithTag("HeartRateSimulator").GetComponent<HeartRateSimulator>();

        damageOverlay.color = new Color(1f, 0f, 0f, 0f); // Start transparent
        cameraController = FindObjectOfType<FpsCameraController>();

        // Create and configure the AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Set health to 100 with the 0 key
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            health = 100;
            Debug.Log("Health reset to 100.");
        }
    }

    public void DamagePlayer()
    {
        if (!isDamaged)
        {
            // Play random damage sound with pitch variation
            if (damageSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, damageSounds.Length);
                audioSource.clip = damageSounds[randomIndex];
                audioSource.pitch = Random.Range(0.9f, 1.1f); // Slight pitch variation
                audioSource.Play();
            }

            health -= 70;
            if (health < 0)
            {
                health = 0;
            }
            isDamaged = true;
            StartCoroutine(DamageCooldown());

            cameraController.StartScreenShake();

            heartRateSimulator.BumpUp();

            if (health < 50)
            {
                if (strobeCoroutine != null)
                {
                    StopCoroutine(strobeCoroutine);
                }
                strobeCoroutine = StartCoroutine(StrobeEffect());
            }

            if(health > 0)
            {
                suitVoice.playDamageAudio();
            }

            if (health <= 0)
            {
                RevealGameOverUI();
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
            suitVoice.playRestoreAudio();
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

        suitVoice.PlayConditionStabilizedAudio();
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

    private void RevealGameOverUI()
    {
        if (gameOverUI != null)
        {
            UIBlur.SetActive(true);
            gameOverUI.SetActive(true);
        }

        // Make the cursor visible and unlock it
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Freeze the game by setting time scale to 0
        Time.timeScale = 0f;
    }
}
