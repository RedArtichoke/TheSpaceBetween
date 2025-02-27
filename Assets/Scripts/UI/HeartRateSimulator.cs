using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartRateSimulator : MonoBehaviour
{
    [Header("Heart Rate References")]
    public HeartRateAnimator heartRateAnimator; // Reference to the HeartRateAnimator component
    
    [Header("Simulation Settings")]
    [Range(60, 130)]
    public float targetHeartRate = 80f; // Target BPM to set
    public float changeSpeed = 5f; // How quickly the heart rate changes
    [Range(60, 90)]
    public float baseHeartRate = 70f; // Normal resting heart rate
    public float recoveryRate = 0.5f; // How quickly heart rate returns to normal
    public float stressAccumulation = 0f; // Tracks how stressed the player is
    public float maxHeartRate = 130f; // Maximum possible heart rate

    [Header("Bump Up Settings")]
    public AnimationCurve bumpUpCurve = AnimationCurve.EaseInOut(60f, 15f, 130f, 1f); // Controls how much BPM increases based on current rate
    public float stressRecoveryDelay = 5f; // How much each bump up delays recovery
    public float maxStressAccumulation = 30f; // Maximum stress accumulation

    [Header("Controls")]
    public KeyCode increaseRateKey = KeyCode.RightBracket; // "]" key to increase heart rate
    public KeyCode decreaseRateKey = KeyCode.LeftBracket; // "[" key to decrease heart rate
    public KeyCode bumpUpKey = KeyCode.Equals; // "=" key to trigger BumpUp
    public float rateChangeAmount = 5f; // How much to change per key press

    // Start is called before the first frame update
    void Start()
    {
        // Verify that we have a reference to the heart rate animator
        if (heartRateAnimator == null)
        {
            Debug.LogWarning("HeartRateSimulator: No HeartRateAnimator reference assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (heartRateAnimator == null) return;
        
        // Handle key inputs for manual adjustment
        if (Input.GetKeyDown(increaseRateKey))
        {
            BumpUp();
        }
        else if (Input.GetKeyDown(decreaseRateKey))
        {
            targetHeartRate -= rateChangeAmount;
        }
        else if (Input.GetKeyDown(bumpUpKey))
        {
            BumpUp();
        }
        
        // Clamp the target heart rate to reasonable values
        targetHeartRate = Mathf.Clamp(targetHeartRate, 60f, maxHeartRate);
        
        // Gradually adjust the heart rate animator's BPM toward the target
        heartRateAnimator.beatsPerMinute = Mathf.Lerp(
            heartRateAnimator.beatsPerMinute, 
            targetHeartRate, 
            Time.deltaTime * changeSpeed
        );
        
        // Gradually reduce stress accumulation over time
        if (stressAccumulation > 0)
        {
            stressAccumulation -= Time.deltaTime * recoveryRate;
            stressAccumulation = Mathf.Max(0, stressAccumulation);
        }
        
        // Gradually return heart rate to base level when not stressed
        if (targetHeartRate > baseHeartRate)
        {
            // Calculate recovery speed - slower when stress is higher
            float recoverySpeed = recoveryRate / (1.0f + stressAccumulation * 0.5f);
            
            // Apply recovery
            targetHeartRate -= Time.deltaTime * recoverySpeed;
            targetHeartRate = Mathf.Max(baseHeartRate, targetHeartRate);
        }
    }
    
    // Method to increase heart rate in a realistic way when scared
    public void BumpUp()
    {
        // Calculate bump amount based on current heart rate (higher HR = smaller bump)
        float bumpAmount = bumpUpCurve.Evaluate(targetHeartRate);
        
        // Apply the bump
        targetHeartRate += bumpAmount;
        
        // Increase stress accumulation (delays recovery)
        stressAccumulation += stressRecoveryDelay;
        stressAccumulation = Mathf.Min(stressAccumulation, maxStressAccumulation);
        
        // Clamp to maximum heart rate
        targetHeartRate = Mathf.Clamp(targetHeartRate, baseHeartRate, maxHeartRate);
    }
    
    // Optional: Add methods to programmatically set heart rate from other scripts
    public void SetHeartRate(float newRate)
    {
        targetHeartRate = Mathf.Clamp(newRate, 60f, maxHeartRate);
    }
    
    public void IncreaseHeartRate(float amount)
    {
        targetHeartRate += amount;
        targetHeartRate = Mathf.Clamp(targetHeartRate, 60f, maxHeartRate);
    }
    
    public void DecreaseHeartRate(float amount)
    {
        targetHeartRate -= amount;
        targetHeartRate = Mathf.Clamp(targetHeartRate, 60f, maxHeartRate);
    }
}
