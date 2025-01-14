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
    private MeshRenderer thingRenderer; // Reference to the mesh renderer
    private Collider thingCollider; // Reference to the collider

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        thingRenderer = GetComponent<MeshRenderer>(); // Get the MeshRenderer component
        thingCollider = GetComponent<Collider>(); // Get the Collider component

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Find the player by tag

        if (playerObject != null)
        {
            player = playerObject.transform.parent.gameObject; // Get the parent of the player object
            darkController = player.GetComponent<DarkController>();
        }

        InvokeRepeating("RoamAround", Random.Range(5f, 10f), Random.Range(10f, 15f)); // Start roaming
    }

    void Update()
    {
        if (player != null && navAgent.isOnNavMesh)
        {
            if (darkController.inDark)
            {
                // Make visible and enable collisions
                thingRenderer.enabled = true;
                EnableCollisions(true);

                navAgent.SetDestination(player.transform.position); // Chase the player
            }
            else
            {
                // Make invisible and disable collisions with player and mimic
                thingRenderer.enabled = false;
                EnableCollisions(false);
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
}
