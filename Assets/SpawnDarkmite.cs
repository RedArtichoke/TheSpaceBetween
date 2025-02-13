using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DarkMiteSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject darkMitePrefab;      
    public Transform player;              
    public DarkController dark;            
    public HeartRateAnimator heartRateScript;

    [Header("Spawning Settings")]
    public float spawnInterval = 15f;     
    public float spawnDistance = 15f;     
    public float navMeshSampleDistance = 5f; 

    private List<GameObject> spawnedMites = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnMitesRoutine());
    }

    private void Update()
    {
        if (heartRateScript != null && heartRateScript.beatsPerMinute <= 60)
        {
            ClearSpawnedMites();
        }
    }

    private IEnumerator SpawnMitesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (dark != null && dark.inDark)
            {
                Vector3 spawnPosition;
                if (TryGetValidSpawnLocation(out spawnPosition))
                {
                    int numberOfMites = 0;

                    // Check heartbeat to determine number of mites to spawn
                    if (heartRateScript != null)
                    {
                        if (heartRateScript.beatsPerMinute > 100)
                        {
                            numberOfMites = 2;
                        }
                        else if (heartRateScript.beatsPerMinute > 80)
                        {
                            numberOfMites = 1;
                        }
                    }

                    // Spawn the determined number of mites at the location
                    for (int i = 0; i < numberOfMites; i++)
                    {
                        SpawnMite(spawnPosition);
                    }
                }
            }
        }
    }

    private bool TryGetValidSpawnLocation(out Vector3 spawnPos)
    {
        const int maxAttempts = 10;
        spawnPos = Vector3.zero;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            attempts++;

            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnDistance;
            Vector3 potentialPosition = player.position + offset;

            Ray ray = new Ray(potentialPosition + Vector3.up * 10f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, navMeshSampleDistance, NavMesh.AllAreas))
                {
                    spawnPos = navHit.position;
                    return true;
                }
            }
        }
        return false;
    }

    private void SpawnMite(Vector3 position)
    {
        GameObject mite = Instantiate(darkMitePrefab, position, Quaternion.identity);
        spawnedMites.Add(mite);
    }

    private void ClearSpawnedMites()
    {
        foreach (GameObject mite in spawnedMites)
        {
            if (mite != null)
            {
                Destroy(mite);
            }
        }
        spawnedMites.Clear();
    }
}
