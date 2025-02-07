using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sludge : MonoBehaviour
{
    public DarkController darkController;

    public GameObject particles;

    public MeshRenderer meshRenderer;

    public BoxCollider boxCollider;
    void Update()
    {
        if(darkController.inDark)
        {
            particles.SetActive(true);
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
        }
        else
        {
            particles.SetActive(false);
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
        }
    }
}
