using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableRenderer : MonoBehaviour
{
    public Transform[] cableSegments; // Array of transforms for cable segments
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = cableSegments.Length;
    }

    void Update()
    {
        for (int i = 0; i < cableSegments.Length; i++)
        {
            lineRenderer.SetPosition(i, cableSegments[i].position);
        }
    }
}
