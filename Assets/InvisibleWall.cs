using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    public GameObject player;

    private Collider barrierCollider;

    private void Start()
    {
        barrierCollider = GetComponent<Collider>();
        if (barrierCollider == null)
        {
            Debug.LogError("No Collider found on " + gameObject.name);
        }

        if (player == null)
        {
            Debug.LogError("Player GameObject is not assigned on " + gameObject.name);
        }
    }

    private bool PlayerHasPlug()
    {
        if (player == null)
            return false;

        Transform mainCamera = player.transform.Find("Main Camera");
        if (mainCamera != null)
        {
            Transform plug = mainCamera.Find("Plug");
            return (plug != null);
        }
        return false;
    }

    private void Update()
    {
        if (player == null || barrierCollider == null)
            return;

        Collider[] playerColliders = player.GetComponentsInChildren<Collider>();


        bool ignoreCollision = !PlayerHasPlug();

        foreach (Collider playerCollider in playerColliders)
        {
            Physics.IgnoreCollision(playerCollider, barrierCollider, ignoreCollision);
        }
    }
}
