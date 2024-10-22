using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MimicBehaviour : MonoBehaviour
{
    public float roamRadius = 10f;
    public float neighborRadius = 10f;
    public float cohesionDistance = 7f;
    public float detectionRange = 10f;
    public LayerMask playerLayer;
    public GameObject footprintPrefab; // Reference to the footprint prefab
    private NavMeshAgent agent;
    private Vector3 groupDestination;
    private bool isLeader = false;
    private bool isAttacking = false;
    private GameObject player;
    private bool leftFoot = true;
    private float lastFootprintTime = 0f;
    private float footprintCooldown = 0.5f;

    // Public list to store found neighbors
    public List<GameObject> foundNeighbors = new List<GameObject>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("Roam", Random.Range(5f, 10f), Random.Range(10f, 15f)); // Random delay
        InvokeRepeating("LeaveFootprint", 0.5f, 0.5f); // Check for footprints every 0.5 seconds
    }

    void Roam()
    {
        if (isAttacking) return;

        if (foundNeighbors.Count > 0)
        {
            if (!isLeader)
            {
                isLeader = foundNeighbors[0].GetComponent<MimicBehaviour>().isLeader == false;
            }

            if (isLeader)
            {
                // Leader generates a new group destination
                Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
                randomDirection.y = 0; // Keep movement on
                groupDestination = transform.position + randomDirection;
            }
            else
            {
                // Followers use the leader's destination with a slight offset
                groupDestination = foundNeighbors[0].GetComponent<MimicBehaviour>().groupDestination;
                groupDestination += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            }
        }
        else
        {
            isLeader = false;
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection.y = 0; // Keep movement on
            groupDestination = transform.position + randomDirection;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(groupDestination, out hit, roamRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    bool DetectPlayer()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        foreach (var player in players)
        {
            if (player.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        // Real-time ally detection
        foundNeighbors.Clear();
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.CompareTag("Mimic") && neighbor.gameObject != this.gameObject)
            {
                foundNeighbors.Add(neighbor.gameObject);
            }
        }

        // Real-time player detection
        if (foundNeighbors.Count > 0 && DetectPlayer())
        {
            isAttacking = true;
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (isAttacking && player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > detectionRange)
            {
                isAttacking = false; // Stop pursuing if out of range
            }
            else
            {
                agent.SetDestination(player.transform.position);
            }
        }
    }

    void LeaveFootprint()
    {
        if (footprintPrefab != null && agent.velocity.magnitude > 0.1f && Time.time - lastFootprintTime > footprintCooldown) // Check if moving and cooldown passed
        {
            Vector3 offset = leftFoot ? new Vector3(-0.2f, 0, 0) : new Vector3(0.2f, 0, 0);
            Vector3 position = transform.position + transform.TransformDirection(offset) + new Vector3(0, 0.1f, 0); // Slightly above ground

            // Check for existing footprints within 1 meter
            Collider[] nearbyFootprints = Physics.OverlapSphere(position, 1f, LayerMask.GetMask("Footprints"));
            if (nearbyFootprints.Length == 0)
            {
                Quaternion rotation = Quaternion.LookRotation(agent.velocity.normalized);
                float angleOffset = leftFoot ? -15f : 15f; // Rotate left or right by 15 degrees
                rotation *= Quaternion.Euler(0, angleOffset + 180f, 0); // Flip 180 degrees

                GameObject footprint = Instantiate(footprintPrefab, position, rotation);
                footprint.transform.localScale = new Vector3(0.02f, 0.1f, 0.02f); // Small scale
                Destroy(footprint, 15f); // Destroy after 15 seconds
                leftFoot = !leftFoot; // Alternate foot
                lastFootprintTime = Time.time; // Update last footprint time
            }
        }
    }
}
