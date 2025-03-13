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

    public GameObject UIComponents;

    public GameObject gameOverUIButton1;
    public GameObject gameOverUIButton2;
    public GameObject gameOverUIButton3;
    public GameObject gameOverUIButton4;
    public GameObject gameOverUIButton5;

    public GameObject UIBlur;

    public AudioClip[] damageSounds; // Array of audio clips for damage
    private AudioSource audioSource; // AudioSource component

    public SuitVoice suitVoice;

    private HeartRateSimulator heartRateSimulator;

    public PowerController powerController;

    public CanvasGroup bg;

    public AudioSource reviveSound;
    public PlayGameButton playButton;

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

        if(health > 0)
        {
            playButton.isDead = false;
        }
        else
        {
            playButton.isDead = true;
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

                UIComponents.SetActive(false);

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

    public void RespawnPlayer()
    {
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        UIBlur.SetActive(false);

        health = 30;

        reviveSound.Play();

        UIComponents.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        gameOverUIButton1.SetActive(false);
        gameOverUIButton2.SetActive(false);
        gameOverUIButton3.SetActive(false);
        gameOverUIButton4.SetActive(false);
        gameOverUIButton5.SetActive(false);

        yield return new WaitForSeconds(6f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        bg.alpha = 1f; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            bg.alpha = alpha;
            yield return null;  
        }
        
        bg.alpha = 0f;

        gameOverUI.SetActive(false);

        UIComponents.SetActive(true);

        gameOverUIButton1.SetActive(true);
        gameOverUIButton2.SetActive(true);
        gameOverUIButton3.SetActive(true);
        gameOverUIButton4.SetActive(true);
        gameOverUIButton5.SetActive(true);

        StartCoroutine(RegenerateHealth());
        
        

        powerController.power = 90;
    }
}
