using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCameraController : MonoBehaviour
{
    // Sensitivity of the mouse, because we all love a sensitive mouse
    public float mouseSensitivity = 100f;
    public Transform playerBody; // The body of the player, not to be confused with a real body
    public float lerpSpeed = 0.1f; // Speed of the lerp, not to be confused with a slurp

    private float xRotation = 0f; // X-axis rotation, because we like to spin
    private float currentXRotation = 0f; // Current X-axis rotation, for those who like to keep track
    private float yRotation = 0f; // Y-axis rotation, because spinning in two directions is better
    private float currentYRotation = 0f; // Current Y-axis rotation, for the organized spinners

    public AudioClip enemyAudioCue; // Audio clip to play when an enemy is seen
    public float audioCooldown = 60f; // Cooldown time in seconds
    private float lastAudioPlayTime = -Mathf.Infinity; // Track last play time

    private AudioSource audioSource;
    public SuitVoice suitVoice;

    void Start()
    {
        // Lock that cursor like it's a treasure chest
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Make the cursor disappear like magic

        // Add an AudioSource component if not already present
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = enemyAudioCue;

        // Adjust pitch to make the sound quicker and higher
        audioSource.pitch = 1.5f; // Increase pitch (1.0 is normal)
    }

    void Update()
    {
        // Get the mouse movement, because we need to know where it's going
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust the xRotation based on mouseY, because up is down and down is up
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Keep the head from spinning too far

        // Adjust the yRotation based on mouseX, because left is right and right is left
        yRotation += mouseX;

        // Smoothly transition to the new rotation, like a gentle breeze
        currentXRotation = Mathf.Lerp(currentXRotation, xRotation, lerpSpeed);
        currentYRotation = Mathf.Lerp(currentYRotation, yRotation, lerpSpeed);

        // Apply the rotations, because we want to see the world from a new angle
        transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);

        CheckForVisibleEnemies();
    }

    private void CheckForVisibleEnemies()
    {
        // Define the tags to search for
        string[] enemyTags = { "Mimic", "Thing" };
        List<GameObject> enemies = new List<GameObject>();

        // Find all game objects with the specified tags
        foreach (string tag in enemyTags)
        {
            enemies.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        foreach (GameObject enemy in enemies)
        {
            if (IsVisible(enemy) && Time.time >= lastAudioPlayTime + audioCooldown)
            {
                PlayEnemyAudioCue();
                lastAudioPlayTime = Time.time; // Update last play time
                break; // Exit loop after playing audio once
            }
        }
    }

    private bool IsVisible(GameObject obj)
    {
        // Find the Renderer component in the object or its children
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            return false; // No renderer found, so enemy is not visible
        }

        // Check if the enemy is within the camera's view
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
        {
            return false; // Enemy is not within the camera's view
        }

        // Perform a raycast to check for line of sight
        Vector3 directionToEnemy = renderer.bounds.center - Camera.main.transform.position;
        if (Physics.Raycast(Camera.main.transform.position, directionToEnemy, out RaycastHit hit))
        {
            if (hit.transform.gameObject == obj)
            {
                return true; // Enemy is visible and not blocked
            }
        }

        return false; // Line of sight is blocked
    }

    private void PlayEnemyAudioCue()
    {
        if (audioSource != null && enemyAudioCue != null)
        {
            audioSource.Play();
            suitVoice.PlayHostileDetected();
        }
    }

    // Public method to start screen shake with optional duration
    public void StartScreenShake(float duration = 0.4f)
    {
        StartCoroutine(ScreenShake(duration));
    }

    // Screen shake coroutine with configurable duration
    private IEnumerator ScreenShake(float duration)
    {
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
}
