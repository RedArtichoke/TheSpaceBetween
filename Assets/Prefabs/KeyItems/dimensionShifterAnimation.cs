using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dimensionShifterAnimation : MonoBehaviour
{
    public float bobSpeed = 2f;  // Speed of the bobbing motion
    public float bobAmount = 0.5f;  // How much it moves up and down
    public float rotationSpeed = 20f;  // How fast it rotates

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Bobbing motion (up and down)
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotating motion (horizontal rotation)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
