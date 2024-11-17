using System.Collections;
using System.Collections.Generic;
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
    private bool isAttacking = false;
    private GameObject player;
    private bool leftFoot = true;
    private float lastFootprintTime = 0f;
    private float footprintCooldown = 0.5f;

    // A list to keep track of our mimic buddies
    public List<GameObject> foundNeighbours = new List<GameObject>();

    private LayerMask playerLayer; // Layer for detecting the player

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerLayer = LayerMask.GetMask("Player"); // Set player layer
        InvokeRepeating("RoamAround", Random.Range(5f, 10f), Random.Range(10f, 15f)); // Roaming with style
        InvokeRepeating("LeaveFootprint", 0.5f, 0.5f); // Footprint party every 0.5 seconds
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

        if (isLeader || Vector3.Distance(transform.position, groupDestination) > 2f)
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
        // Let's find our mimic friends!
        foundNeighbours.Clear();
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.CompareTag("Mimic") && neighbor.gameObject != this.gameObject)
            {
                foundNeighbours.Add(neighbor.gameObject);
            }
        }

        // Time to play tag with the player!
        if (foundNeighbours.Count > 0 && IsPlayerDetected())
        {
            isAttacking = true;
            player = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;
        }

        if (isAttacking && player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > detectionRange)
            {
                isAttacking = false; // Player ran away, let's chill.
            }
            else
            {
                navAgent.SetDestination(player.transform.position);
            }
        }

        // Check if player is within 1.5 meters and mimic is chasing player
        if (isAttacking && player != null && Vector3.Distance(transform.position, player.transform.position) <= 1.5f)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    void LeaveFootprint()
    {
        if (footprintPrefab != null && navAgent.velocity.magnitude > 1f && Time.time - lastFootprintTime > footprintCooldown)
        {
            Vector3 offset = leftFoot ? new Vector3(-0.2f, 0, 0) : new Vector3(0.2f, 0, 0);
            Vector3 position = transform.position + transform.TransformDirection(offset) + new Vector3(0, 0.1f, 0);

            // Check for existing footprints within 1 meter
            Collider[] nearbyFootprints = Physics.OverlapSphere(position, 1f, LayerMask.GetMask("Footprints"));
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
            }
        }
    }

    // Handles visibility and movement control
    IEnumerator AttackPlayer()
    {        
        // Damage the player
        GameObject player = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;
        if (player != null)
        {
            HealthManager healthManager = player.GetComponent<HealthManager>();
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
            }
        }
    }
}
