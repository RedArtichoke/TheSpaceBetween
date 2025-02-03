using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShaftBaker : MonoBehaviour
{
    public GameObject cylinderPrefab; // Prefab of the cylinder mesh
    public Material coneMaterial; // Material with 5% opacity and pale yellow tint

    // Start is called before the first frame update
    void Start()
    {
        // Find all spotlights tagged "Hall Light"
        GameObject[] hallLights = GameObject.FindGameObjectsWithTag("Hall Light");
        
        foreach (GameObject light in hallLights)
        {
            Light spotlight = light.GetComponent<Light>();
            if (spotlight != null && spotlight.type == LightType.Spot)
            {
                // Instantiate cylinder
                GameObject cylinder = Instantiate(cylinderPrefab, light.transform.position, light.transform.rotation);
                cylinder.transform.SetParent(light.transform);

                // Adjust rotation to point downwards
                cylinder.transform.localRotation = Quaternion.Euler(-90, 0, 0);

                // Set fixed length for the cone
                float fixedLength = 15f;
                float angle = spotlight.spotAngle;
                float radius = fixedLength * Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad);
                cylinder.transform.localScale = new Vector3(radius * 2, fixedLength, radius * 2);

                // Offset the cone downward by half its height
                cylinder.transform.localPosition = new Vector3(0, 0, fixedLength -5);

                // Adjust the top vertices to form a cone
                MeshFilter meshFilter = cylinder.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh mesh = meshFilter.mesh;
                    Vector3[] vertices = mesh.vertices;

                    for (int i = 0; i < vertices.Length; i++)
                    {
                        if (vertices[i].y > 0) // Assuming top vertices have positive y
                        {
                            vertices[i].x *= 0; // Scale x to zero
                            vertices[i].z *= 0; // Scale z to zero
                        }
                    }

                    mesh.vertices = vertices;
                    mesh.RecalculateNormals();
                }

                // Apply material
                Renderer coneRenderer = cylinder.GetComponent<Renderer>();
                coneRenderer.material = coneMaterial;
            }
        }
    }
}
