using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThingController : MonoBehaviour
{
    private NavMeshAgent navAgent; // Handles navigation
    private GameObject player; // Reference to the player
    private DarkController darkController;
    public float roamRadius = 10f; // How far can the thing wander?
    private Collider thingCollider; // Reference to the collider
    private AudioSource audioSource; // Audio source for playing sounds
    public AudioClip[] soundClips; // Array of sound clips
    public AudioClip[] footstepClips; // Array of footstep sound clips
    private GameObject body; // Reference to the Body GameObject
    private Animator animator; // Reference to the Animator
    private HeartRateAnimator heartRateAnimator; // Reference to Heart Rate Animator

    public float crawlStepDistance = 0.25f;
    public float walkStepDistance = 1.5f;
    public float runStepDistance = 2.5f;

    public float crawlStepDelay = 0.3f;
    public float walkStepDelay = 0.35f;
    public float runStepDelay = 0.25f;

    public AudioClip[] crawlingFootstepClips; // Array of crawling footstep sound clips

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        thingCollider = GetComponent<Collider>(); // Get the Collider component

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Find the player by tag

        if (playerObject != null)
        {
            player = playerObject.transform.parent.gameObject; // Get the parent of the player object
            darkController = player.GetComponent<DarkController>();
        }

        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        InvokeRepeating("RoamAround", Random.Range(5f, 10f), Random.Range(10f, 15f)); // Start roaming
        InvokeRepeating("PlayRandomSound", Random.Range(5f, 10f), Random.Range(10f, 25f)); // Schedule sound playing
        StartCoroutine(MoveInSteps());

        body = transform.Find("Body").gameObject; // Find the Body child object
        animator = body.GetComponent<Animator>(); // Get the Animator component

        GameObject heartRateObject = GameObject.FindGameObjectWithTag("HeartRateController");
        if (heartRateObject != null)
        {
            heartRateAnimator = heartRateObject.GetComponent<HeartRateAnimator>();
        }
    }

    void Update()
    {
        if (player != null && navAgent.isOnNavMesh)
        {
            if (darkController.inDark)
            {
                body.SetActive(true);
                EnableCollisions(true);
                navAgent.SetDestination(player.transform.position);

                // Set speed based on heart rate
                if (heartRateAnimator != null)
                {
                    float bpm = heartRateAnimator.beatsPerMinute;
                    if (bpm < 80) // Neutral heart rate
                        navAgent.speed = 2.0f; // Crawling
                    else if (bpm < 100) // Above neutral
                        navAgent.speed = 3.0f; // Walking
                    else // Notably above neutral
                        navAgent.speed = 5.0f; // Running
                }

                // Set animation based on NavMeshAgent speed
                animator.SetBool("walking", navAgent.speed == 2.0f);
                animator.SetBool("running", navAgent.speed == 3.0f);
                animator.SetBool("crawling", navAgent.speed == 5.0f);
                animator.speed = 1.0f;
            }
            else
            {
                body.SetActive(false);
                EnableCollisions(false);
                animator.speed = 0.0f;
            }
        }
    }

    void RoamAround()
    {
        if (!darkController.inDark && navAgent.isOnNavMesh)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection.y = 0; // Keep it on the ground
            Vector3 roamPosition = transform.position + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(roamPosition, out hit, roamRadius, 1))
            {
                navAgent.SetDestination(hit.position); // Set a random destination
            }
        }
    }

    void EnableCollisions(bool enable)
    {
        Collider[] playerColliders = GameObject.FindGameObjectWithTag("Player").GetComponentsInChildren<Collider>();
        foreach (var playerCollider in playerColliders)
        {
            Physics.IgnoreCollision(thingCollider, playerCollider, !enable);
        }

        GameObject[] mimics = GameObject.FindGameObjectsWithTag("Mimic");
        foreach (var mimic in mimics)
        {
            Collider mimicCollider = mimic.GetComponent<Collider>();
            if (mimicCollider != null)
            {
                Physics.IgnoreCollision(thingCollider, mimicCollider, !enable);
            }
        }
    }

    void PlayRandomSound()
    {
        if (soundClips.Length > 0 && darkController.inDark && player != null)
        {
            int randomIndex = Random.Range(0, soundClips.Length); // Select a random clip
            AudioClip selectedClip = soundClips[randomIndex];

            audioSource.pitch = Random.Range(0.5f, 1f); // Random pitch, lower emphasis
            audioSource.PlayOneShot(selectedClip); // Play the selected clip with calculated volume
        }
    }

    IEnumerator MoveInSteps()
    {
        while (true)
        {
            if (player != null && darkController.inDark)
            {
                float stepDistance = 0f;
                float stepDelay = 0f;

                // Determine step parameters based on speed
                if (navAgent.speed == 2.0f) // Walking
                {
                    stepDistance = walkStepDistance;
                    stepDelay = walkStepDelay;
                }
                else if (navAgent.speed == 3.0f) // Running
                {
                    stepDistance = runStepDistance;
                    stepDelay = runStepDelay;
                }
                else if (navAgent.speed == 5.0f) // Crawling
                {
                    stepDistance = crawlStepDistance;
                    stepDelay = crawlStepDelay;
                }

                Vector3 direction = (player.transform.position - transform.position).normalized;
                Vector3 startPosition = transform.position;
                Vector3 endPosition = startPosition + direction * stepDistance;

                float elapsedTime = 0f;

                navAgent.enabled = false; // Disable NavMeshAgent to manually move

                while (elapsedTime < stepDelay)
                {
                    float t = elapsedTime / stepDelay;
                    t = t * t * t * (t * (6f * t - 15f) + 10f); // Smootherstep easing
                    Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
                    transform.position = currentPosition;

                    elapsedTime += Time.deltaTime;
                    yield return null; // Wait for the next frame
                }

                transform.position = endPosition; // Ensure final position is exact
                navAgent.enabled = true; // Re-enable NavMeshAgent

                PlayFootstepSound(); // Play footstep sound with each step

                yield return new WaitForSeconds(stepDelay); // Wait before next step
            }
            else
            {
                yield return null; // Wait for the next frame
            }
        }
    }

    void PlayFootstepSound()
    {
        AudioClip[] currentFootstepClips = footstepClips;

        if (navAgent.speed == 5.0f) // Crawling
        {
            currentFootstepClips = crawlingFootstepClips;
        }

        if (currentFootstepClips.Length > 0)
        {
            int randomIndex = Random.Range(0, currentFootstepClips.Length); // Select a random footstep clip
            AudioClip selectedClip = currentFootstepClips[randomIndex];

            audioSource.pitch = Random.Range(0.8f, 1.2f); // Random pitch for footsteps
            audioSource.PlayOneShot(selectedClip, 0.3f); // Play the selected footstep clip with calculated volume
        }
    }
}
