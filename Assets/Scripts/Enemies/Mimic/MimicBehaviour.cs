using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MimicBehaviour : MonoBehaviour
{
    // Public variables for setting up the mimic's behavior
    public float roamRadius = 10f; // How far can the mimic wander?
    public float neighborRadius = 10f; // How close do we need to be friends?
    public float cohesionDistance = 7f; // How close is too close?
    public float detectionRange = 10f; // Can you see me now?
    public GameObject footprintPrefab; // Footprint factory!

    // Private variables for internal mimic shenanigans
    private NavMeshAgent navAgent;
    private Vector3 groupDestination;
    private bool isLeader = false;
    public bool isAttacking = false;
    // Crappy solution to a bizarre problem- made player public for permanent reference
    public GameObject player;
    private bool leftFoot = true;
    private float lastFootprintTime = 0f;
    private float footprintCooldown = 0.5f;

    // A list to keep track of our mimic buddies
    public List<GameObject> foundNeighbours = new List<GameObject>();

    private LayerMask playerLayer; // Layer for detecting the player

    public AudioClip[] mimicSounds; // Array to store mimic sounds
    private AudioSource mimicAudioSource; // AudioSource for playing sounds

    public AudioClip stepSound; // Add this line to declare the step sound

    public AudioClip[] stunSounds; // Array to store stun sounds

    public AudioClip shakingSound;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerLayer = LayerMask.GetMask("Player"); // Set player layer
        InvokeRepeating("RoamAround", Random.Range(5f, 10f), Random.Range(10f, 15f)); // Roaming with style
        InvokeRepeating("LeaveFootprint", 0.25f, 0.25f); // Footprint party every 0.5 seconds

        mimicAudioSource = gameObject.GetComponent<AudioSource>();
        mimicAudioSource.spatialBlend = 1.0f; // Make the sound 3D
        mimicAudioSource.maxDistance = 35f; // Set max distance for sound

        // Add reverb filter for haunting effect
        if (!GameObject.Find("MimicReverb")) {
            var reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
            reverbFilter.reverbPreset = AudioReverbPreset.Cave; // Choose a haunting reverb preset
        }

        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(PlayRandomSound());
    }

    void RoamAround()
    {
        if (isAttacking) return; // No roaming while attacking

        if (foundNeighbours.Count > 0)
        {
            // Ensure only one leader
            if (!isLeader)
            {
                isLeader = !foundNeighbours.Exists(neighbor => neighbor.GetComponent<MimicBehaviour>().isLeader);
            }

            if (isLeader)
            {
                // Leader decides the new destination
                Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
                randomDirection.y = 0; // No flying
                groupDestination = transform.position + randomDirection;
            }
            else
            {
                // Follow the leader's destination with an offset
                var leader = foundNeighbours.Find(neighbor => neighbor.GetComponent<MimicBehaviour>().isLeader);
                if (leader != null)
                {
                    Vector3 offset = (transform.position - leader.transform.position).normalized * 2f;
                    groupDestination = leader.GetComponent<MimicBehaviour>().groupDestination + offset;
                }
            }
        }
        else
        {
            isLeader = false;
            // Only generate a new destination if not in a group
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection.y = 0; // Stay grounded
            groupDestination = transform.position + randomDirection;
        }

        if ((isLeader || Vector3.Distance(transform.position, groupDestination) > 2f) && navAgent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(groupDestination, out hit, roamRadius, 1))
            {
                navAgent.SetDestination(hit.position);
            }
        }
    }

    bool IsPlayerDetected()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        foreach (var player in players)
        {
            if (player.CompareTag("Player"))
            {
                return true; // Player spotted! Engage!
            }
        }
        return false; // No players here, just us mimics.
    }

    void Update()
    {
        if (player == null)
        {
            // Debug.LogError("Player reference is null.");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // Debug.Log("Distance to player: " + distanceToPlayer);

        if (distanceToPlayer <= detectionRange)
        {
            isAttacking = true;
            // Debug.Log("Player detected, attacking!");
        }
        else
        {
            isAttacking = false;
            // Debug.Log("Player out of range.");
        }

        if (isAttacking)
        {
            if (navAgent.isOnNavMesh && navAgent.enabled)
            {
                navAgent.SetDestination(player.transform.position);
                // Debug.Log("Chasing player.");
            }

            if (distanceToPlayer <= 2f)
            {
                StartCoroutine(AttackPlayer());
                // Debug.Log("Attacking player!");
            }
        }
        else
        {
            // Only disguise if not attacking
            if (!isAttacking)
            {
                StartCoroutine(DelayedDisguise());
            }
        }
    }

    void LeaveFootprint()
    {
        if (footprintPrefab != null && navAgent.velocity.magnitude > 1f && Time.time - lastFootprintTime > footprintCooldown)
        {
            Vector3 offset = leftFoot ? new Vector3(-0.2f, 0, 0) : new Vector3(0.2f, 0, 0);
            Vector3 position = transform.position + transform.TransformDirection(offset) + new Vector3(0, 0.1f, 0);

            // Check for existing footprints within 1 meter
            Collider[] nearbyFootprints = Physics.OverlapSphere(position, 0.5f, LayerMask.GetMask("Footprints"));
            if (nearbyFootprints.Length == 0)
            {
                Quaternion rotation = Quaternion.LookRotation(navAgent.velocity.normalized);
                float angleOffset = leftFoot ? -15f : 15f; // Left or right foot?
                rotation *= Quaternion.Euler(0, angleOffset + 180f, 0); // Spin around!

                GameObject footprint = Instantiate(footprintPrefab, position, rotation);
                footprint.transform.localScale = new Vector3(0.02f, 0.1f, 0.02f); // Tiny footprints!
                Destroy(footprint, 15f); // Footprints vanish after 15 seconds
                leftFoot = !leftFoot; // Switch feet for the next step
                lastFootprintTime = Time.time; // Update the last footprint time

                // Play step sound with random pitch
                if (stepSound != null)
                {
                    mimicAudioSource.pitch = Random.Range(0.8f, 1.2f); // Random pitch variation
                    mimicAudioSource.PlayOneShot(stepSound);
                }
            }
        }
    }

    // Handles visibility and movement control
    IEnumerator AttackPlayer()
    {        
        // Damage the player
        if (player != null)
        {
            // Get the parent of the player object
            GameObject actualPlayer = player.transform.parent?.gameObject;

            if (actualPlayer != null)
            {

                HealthManager healthManager = actualPlayer.GetComponent<HealthManager>();
                if (healthManager != null)
                {
                    if (!healthManager.isDamaged)
                    {

                        healthManager.DamagePlayer();
                        MimicBody mimicBody = GetComponentInChildren<MimicBody>();
                        if (mimicBody != null)
                        {
                            mimicBody.isVisible = true;
                            mimicBody.UpdateVisibility();
                        }
                        navAgent.isStopped = true;
                        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
                        if (mimicBody != null)
                        {
                            mimicBody.isVisible = false;
                            mimicBody.UpdateVisibility();
                        }
                        yield return new WaitForSeconds(2f);

                        navAgent.isStopped = false;
                    }
                    else
                    {
                        Debug.Log("Player is already damaged.");
                    }
                }
                else
                {
                    Debug.Log("HealthManager not found on actual player: " + actualPlayer.name);
                }
            }
            else
            {
                Debug.Log("Actual player is null.");
            }
        }
        else
        {
            Debug.Log("Player is null.");
        }
    }

    // Handles visibility and movement control
    public IEnumerator StunMimic()
    {        
        // Play a random stun sound with varying pitch
        if (stunSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, stunSounds.Length); // Pick a random sound
            mimicAudioSource.pitch = Random.Range(0.8f, 1.2f); // Random pitch variation
            mimicAudioSource.PlayOneShot(stunSounds[randomIndex]); // Play the sound
        }

        MimicBody mimicBody = GetComponentInChildren<MimicBody>();
        if (mimicBody != null)
        {
            mimicBody.isVisible = true;
            mimicBody.UpdateVisibility();
        }
        
        // Check if NavMeshAgent is valid before stopping
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
        }
        
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f)); // Fixed inverted min/max values
        
        if (mimicBody != null)
        {
            mimicBody.isVisible = false;
            mimicBody.UpdateVisibility();
        }
        yield return new WaitForSeconds(5f);

        // Check if NavMeshAgent is valid before resuming
        if (navAgent != null && navAgent.isOnNavMesh && navAgent.enabled)
        {
            navAgent.isStopped = false;
        }
    }

    IEnumerator PlayRandomSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(10f, 20f)); // Random delay between sounds

            if (mimicSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, mimicSounds.Length);
                mimicAudioSource.clip = mimicSounds[randomIndex];

                // Ensure player reference is always available
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                }

                if (player != null)
                {
                    // Calculate volume based on proximity to player
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance <= 35f) // Check if within 35 meters
                    {
                        float volume = Mathf.Clamp01(1 - (distance / mimicAudioSource.maxDistance));
                        mimicAudioSource.volume = volume;

                        // Randomize pitch for uniqueness
                        mimicAudioSource.pitch = Random.Range(0.8f, 1.2f);

                        mimicAudioSource.Play();
                    }
                }
            }
        }
    }

    void DisguiseAsObject()
    {
        // Scan for objects in the "Pickup" layer
        Collider[] objects = Physics.OverlapSphere(transform.position, roamRadius, LayerMask.GetMask("Pickup"));
        if (objects.Length == 0) return; // No objects found, continue roaming

        // Choose a random object
        int randomIndex = Random.Range(0, objects.Length);
        GameObject chosenObject = objects[randomIndex].gameObject;

        // Duplicate the object above the mimic
        Vector3 positionAbove = transform.position + Vector3.up;
        GameObject duplicate = Instantiate(chosenObject, positionAbove, Quaternion.identity);

        // Set the tag of the duplicated object to "Mimic"
        duplicate.tag = "Mimic";

        // Add the DisguisedMimic component and store reference to the original mimic
        DisguisedMimic disguisedMimic = duplicate.AddComponent<DisguisedMimic>();
        disguisedMimic.originalMimic = this.gameObject;

        //Add audio component
        AudioSource audioSource = duplicate.AddComponent<AudioSource>();

        audioSource.playOnAwake = false; 
        audioSource.spatialBlend = 1.0f; 
        audioSource.volume = 0.5f; 
        audioSource.clip = shakingSound;

        // Make the mimic a child of the duplicated object
        transform.SetParent(duplicate.transform);

        // Disable the mimic
        gameObject.SetActive(false);
    }

    IEnumerator DelayedDisguise()
    {
        yield return new WaitForSeconds(10f); // Wait for 10 seconds

        // Check again if still not attacking
        if (!isAttacking)
        {
            DisguiseAsObject();
        }
    }
}
