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
        foreach (GameObject footprint in footprintPrefabs)
        {
            if (footprint != null)
            {
                footprint.SetActive(true);
                yield return new WaitForSeconds(stepInterval);
            }
        }

        yield return new WaitForSeconds(1f);

        DestroyDoor();
    }

    void DestroyDoor()
    {
        fixedDoor.SetActive(false);
        brokenDoor.SetActive(true);
    }
}
