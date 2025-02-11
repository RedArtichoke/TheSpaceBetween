using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineGrowth : MonoBehaviour
{
    public GameObject vinePrefab; // Public vine prefab to be set in the Unity editor
    public int maxDepth = 10; // Maximum depth of vine branching
    public int maxBranches = 10; // Maximum number of branches per vine

    // Start is called before the first frame update
    void Start()
    {
        FindHallway(); // Uses object's position by default
    }

    /// <summary>
    /// Finds the nearest prefab with "hallway" in its name and covers its children containing "geo" in their name with vines. If none are found, use the hallway object.
    /// </summary>
    /// <param name="searchPosition">Optional position to search from. Defaults to object's position.</param>
    void FindHallway(Vector3? searchPosition = null)
    {
        Vector3 positionToSearchFrom = searchPosition ?? transform.position; // Use provided position or default to object's position
        Vector3 direction = Vector3.left; // Define the direction to shoot the ray

        RaycastHit hit;
        float rayDistance = 10f; // Define a reasonable ray distance

        if (Physics.Raycast(positionToSearchFrom, direction, out hit, rayDistance))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.name.ToLower().Contains("hallway"))
            {
                Transform targetTransform = obj.transform; // Default to hallway

                foreach (Transform child in obj.transform)
                {
                    if (child.name.ToLower().Contains("geo"))
                    {
                        targetTransform = child; // Use child if it contains "geo"
                        break; // Stop after finding the first match
                    }
                }

                if (vinePrefab != null)
                {
                    ApplyVinesToWalls(targetTransform); // Apply vines to walls
                }
            }
        }
    }

    /// <summary>
    /// Casts rays in six directions from the target transform and places vines on hit surfaces.
    /// </summary>
    /// <param name="targetTransform">The transform from which to cast rays.</param>
    void ApplyVinesToWalls(Transform seedTransform)
    {
        Vector3[] directions = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        float rayDistance = 5f; // Define how far the ray should check for walls

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(seedTransform.position, direction, out hit, rayDistance))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.name.ToLower().Contains("hallway"))
                {
                    // Calculate the rotation to align the vine's up direction with the wall's normal
                    Quaternion vineRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    
                    // Instantiate vine at the exact hit point on the wall
                    GameObject newVine = Instantiate(vinePrefab, hit.point, vineRotation);
                    GrowVine(newVine.transform, 0); // Start growing from the new vine
                }
            }
        }
    }

    /// <summary>
    /// Recursively grows vines from the given transform using anchor points.
    /// </summary>
    /// <param name="vineTransform">The transform from which to grow new vines.</param>
    /// <param name="currentDepth">The current depth of recursion.</param>
    void GrowVine(Transform vineTransform, int currentDepth)
    {
        if (currentDepth >= maxDepth) return; // Stop if max depth is reached

        // Find the first anchor point in the current vine
        foreach (Transform anchor in vineTransform)
        {
            if (anchor.CompareTag("VineTag")) // Check if the transform is an anchor point
            {
                Vector3 baseDirection = vineTransform.forward; // Use the current vine's forward direction

                RaycastHit hit;
                if (Physics.Raycast(anchor.position, baseDirection, out hit, 5f)) // Increased distance
                {
                    // Calculate a new direction with a slight angle variation in the plane of the wall
                    Vector3 right = Vector3.Cross(hit.normal, Vector3.up).normalized;
                    if (right == Vector3.zero)
                    {
                        right = Vector3.Cross(hit.normal, Vector3.forward).normalized;
                    }

                    int branches = Random.Range(1, maxBranches + 1); // Random number of branches
                    for (int i = 0; i < branches; i++)
                    {
                        Quaternion angleOffset = Quaternion.AngleAxis(Random.Range(-20f, 20f), right);
                        Vector3 variedDirection = angleOffset * baseDirection;
                        // Debugging: Draw the ray in the editor
                        Debug.DrawRay(anchor.position, variedDirection * 5f, Color.red, 1f); // Increased length for visibility

                        // Calculate a rotation that aligns the new vine with the varied direction
                        Quaternion vineRotation = Quaternion.LookRotation(variedDirection, hit.normal);

                        // Instantiate new vine at the anchor's position with the calculated rotation
                        GameObject newVine = Instantiate(vinePrefab, anchor.position, vineRotation);

                        // Move the vine forward 
                        newVine.transform.position += variedDirection.normalized *0.45f;

                        // Recursive call to grow the new vine
                        GrowVine(newVine.transform, currentDepth + 1);
                    }

                    // Chance to keep the anchor for branching
                    if (Random.Range(0, maxBranches) != 0)
                    {
                        Destroy(anchor.gameObject);
                    }
                }
                else
                {
                    Debug.Log("No suitable surface found for vine growth.");
                }

                break; // Stop after processing the first anchor
            }
        }
    }
}
