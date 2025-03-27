using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTheMaintenanceDoor : MonoBehaviour
{
    [Header("Footstep Prefabs in Sequence")]
    public List<GameObject> footprintPrefabs; 
    public float stepInterval = 0.5f; 

    public GameObject fixedDoor;
    public GameObject brokenDoor;

    void OnEnable()
    {
        StartCoroutine(FootstepSequence());
    }

    IEnumerator FootstepSequence()
    {
        int activeCount = 0;

        for (int i = 0; i < footprintPrefabs.Count; i++)
        {
            if (footprintPrefabs[i] != null)
            {
                footprintPrefabs[i].SetActive(true);
                activeCount++;

                if (activeCount > 2 && i - 2 >= 0 && footprintPrefabs[i - 2] != null)
                {
                    footprintPrefabs[i - 2].SetActive(false);
                }

                yield return new WaitForSeconds(stepInterval);
            }
        }

        foreach (var footprint in footprintPrefabs)
        {
            if (footprint != null) footprint.SetActive(false);
        }

        DestroyDoor();
    }

    void DestroyDoor()
    {
        fixedDoor.SetActive(false);
        brokenDoor.SetActive(true);
    }
}
