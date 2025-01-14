using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFlight : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The transform you want this GameObject to move toward.")]
    public Transform target;

    [Tooltip("How fast you want the object to move.")]
    public float speed = 5f;

    private bool isMoving = false;

    public void MoveShip()
    {
        StartCoroutine(MoveToTarget());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SkipCutscene();
        }
    }

    IEnumerator MoveToTarget()
    {
        isMoving = true;
        
        while (Vector3.Distance(transform.position, target.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
            yield return null; // Wait a frame before looping again
        }
        
        transform.position = target.position;
        
        isMoving = false;
    }

    public void SkipCutscene()
    {
        Debug.Log("SKipped");

        transform.position = target.position;
    }
}

