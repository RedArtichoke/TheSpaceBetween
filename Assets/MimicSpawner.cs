using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MimicSpawner : MonoBehaviour
{
    [Header("Mimic Settings")]
    public GameObject mimicPrefab;
    public float spawnInterval = 10f;
    public float spawnRandomVariation = 10f; // +/- seconds of randomisation
    public float spawnRadius = 20f;
    public int maxSpawnAttempts = 30;
    
    private float nextSpawnTime;
    private DarkController playerController;

    // Initialises spawner and starts spawn cycle
    void Start()
    {
        if (mimicPrefab == null)
        {
            Debug.LogError("Mimic prefab not assigned to spawner!");
            enabled = false;
            return;
        }
        
        // Get the DarkController component from parent (player)
        playerController = transform.parent.GetComponent<DarkController>();
        if (playerController == null)
        {
            Debug.LogError("MimicSpawner must be attached to a child of an object with DarkController component!");
            enabled = false;
            return;
        }
        
        SetNextSpawnTime();
    }

    // Checks if it's time to spawn a new mimic
    void Update()
    {
        // Only spawn mimics if player has the device
        if (playerController.hasDevice && Time.time >= nextSpawnTime)
        {
            SpawnMimic();
            SetNextSpawnTime();
        }
    }
    
    // Sets the next spawn time with randomised variation
    void SetNextSpawnTime()
    {
        // Add random variation between +/- spawnRandomVariation
        float randomVariation = Random.Range(-spawnRandomVariation, spawnRandomVariation);
        float actualInterval = Mathf.Max(1f, spawnInterval + randomVariation); // Ensure minimum 1 second
        
        nextSpawnTime = Time.time + actualInterval;
    }
    
    // Attempts to spawn a mimic at a valid position
    void SpawnMimic()
    {
        Vector3 spawnPosition = FindValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(mimicPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Could not find valid spawn position for mimic");
        }
    }
    
    // Searches for a valid spawn position with NavMesh access
    Vector3 FindValidSpawnPosition()
    {
        // Debug visualization to help troubleshoot
        Debug.DrawRay(transform.position, Vector3.up * 3f, Color.green, 0.5f);
        
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Generate random direction and distance within spawn radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            
            // Check if position is on NavMesh with increased search distance
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 5f, NavMesh.AllAreas))
            {
                // Make sure point isn't too close to player (reduced minimum distance)
                if (Vector3.Distance(hit.position, transform.position) > 3f)
                {
                    // Visualize successful hit points
                    Debug.DrawLine(transform.position, hit.position, Color.green, 1f);
                    return hit.position;
                }
            }
            else
            {
                // Visualize failed sample attempts
                Debug.DrawLine(transform.position, randomPosition, Color.red, 0.5f);
            }
        }
        
        // Fallback: Try directly above/below a valid NavMesh point
        NavMeshHit directHit;
        if (NavMesh.SamplePosition(transform.position + Vector3.forward * 3f, out directHit, 10f, NavMesh.AllAreas))
        {
            return directHit.position;
        }
        
        return Vector3.zero; // No valid position found
    }
}
