using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health = 100;
    public bool isDamaged = false;
    public Image damageOverlay;
    
    // Remove default overlay, keep only damage level overlays
    public Sprite damageOverlaySprite1; // First hit
    public Sprite damageOverlaySprite2; // Second hit
    public Sprite damageOverlaySprite3; // Third hit
    // Fourth hit is death
    
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

    // Current active overlay reference
    private Image currentOverlay;

    // Add healing variables
    public int healAmount = 25; // Amount to heal per healing action
    public AudioClip healSound; // Sound to play when healing

    void Start()
    {
        heartRateSimulator = GameObject.FindWithTag("HeartRateSimulator").GetComponent<HeartRateSimulator>();

        // Start with transparent overlay and no sprite
        damageOverlay.color = new Color(1f, 0f, 0f, 0f);
        damageOverlay.sprite = null; // No sprite by default at full health
        
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
            
            // Reset damage overlay when health is reset to full
            damageOverlay.sprite = null;
            damageOverlay.color = new Color(1f, 0f, 0f, 0f); // Fully transparent
            isDamaged = false;
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

            health -= 30;
            if (health < 0)
            {
                health = 0;
            }
            isDamaged = true;

            cameraController.StartScreenShake();

            heartRateSimulator.BumpUp();

            // Removed strobe effect code
            // Instead, just update the overlay
            UpdateCurrentOverlay();
            UpdateOverlayAlpha();

            if(health > 0)
            {
                suitVoice.playDamageAudio();
                StartCoroutine(DamageCooldown());
            }

            if (health <= 0)
            {
                RevealGameOverUI();
                StopCoroutine(DamageCooldown());
                UIComponents.SetActive(false);
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(2f);
        isDamaged = false;
        
        // Removed health regeneration code here
        // Just update the overlay based on current health
        UpdateCurrentOverlay();
        UpdateOverlayAlpha();
    }

    private IEnumerator DamageCooldownRespawn()
    {
        isDamaged = false;
        
        // Removed health regeneration code here
        // Just update the overlay based on current health
        UpdateCurrentOverlay();
        UpdateOverlayAlpha();
        
        yield return null;
    }

    // Replaced RegenerateHealth with this simpler method
    private void UpdateOverlayAlpha()
    {
        // Only update alpha if there's a sprite assigned
        if (damageOverlay.sprite != null)
        {
            // Calculate alpha based on health thresholds
            float alpha;
            
            if (health <= 25)
            {
                alpha = 0.5f; // Strong overlay for critical health
            } 
            else if (health <= 50)
            {
                alpha = 0.35f; // Medium overlay 
            }
            else if (health < 75)
            {
                alpha = 0.25f; // Light overlay
            }
            else
            {
                alpha = 0f; // No overlay at full health
            }
            
            // Apply alpha to the damage overlay
            damageOverlay.color = new Color(1f, 0f, 0f, alpha);
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

        //health = 30;

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

        bg.alpha = 1f;

        UIComponents.SetActive(true);

        gameOverUIButton1.SetActive(true);
        gameOverUIButton2.SetActive(true);
        gameOverUIButton3.SetActive(true);
        gameOverUIButton4.SetActive(true);
        gameOverUIButton5.SetActive(true);

        //StartCoroutine(DamageCooldownRespawn());
        isDamaged = false;
        powerController.power = 90;
    }

    // Selects the appropriate overlay sprite based on health
    private void UpdateCurrentOverlay()
    {
        // No overlay at full health (75-100)
        if (health <= 25 && damageOverlaySprite3 != null)
        {
            damageOverlay.sprite = damageOverlaySprite3; // Most severe overlay (3rd hit)
        }
        else if (health <= 50 && damageOverlaySprite2 != null)
        {
            damageOverlay.sprite = damageOverlaySprite2; // Medium severity overlay (2nd hit)
        }
        else if (health < 75 && damageOverlaySprite1 != null)
        {
            damageOverlay.sprite = damageOverlaySprite1; // Light severity overlay (1st hit)
        }
        else
        {
            damageOverlay.sprite = null; // No overlay at full health
            damageOverlay.color = new Color(1f, 0f, 0f, 0f); // Make fully transparent
        }
    }

    // New method to heal the player
    public void HealPlayer(int amount = 0)
    {
        // Use the default heal amount if no specific amount is provided
        int actualHealAmount = (amount > 0) ? amount : healAmount;
        
        // Only heal if not at full health
        if (health < 100)
        {
            // Play heal sound if available
            if (healSound != null)
            {
                audioSource.clip = healSound;
                audioSource.pitch = 1.0f;
                audioSource.Play();
            }
            
            // Increase health and clamp to max
            health += actualHealAmount;
            if (health > 100)
            {
                health = 100;
            }
            
            // Notify the player with suit voice
            suitVoice.playRestoreAudio();
            
            // Update the overlay to match new health state
            UpdateCurrentOverlay();
            UpdateOverlayAlpha();
            
            Debug.Log("Player healed for " + actualHealAmount + ". Current health: " + health);
        }
        else
        {
            Debug.Log("Player already at full health!");
        }
    }
}
