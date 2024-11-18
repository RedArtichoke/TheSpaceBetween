using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This tutorial helped: https://www.youtube.com/watch?v=wUxAWNnO2qA

public class PlayerAudio : MonoBehaviour
{
    public AudioSource audio;
    
    public AudioClip concrete;
    public AudioClip metal1;
    //public AudioClip metal2;
    public AudioClip water;
    public AudioClip carpet;

    RaycastHit hit;
    public Transform RayStart;
    public float range;
    public LayerMask layerMask;
    
    public void Footstep() 
    {
        if(Physics.Raycast(RayStart.position, RayStart.transform.up * -1, out hit, range, layerMask)) 
        {
            if(hit.collider.CompareTag("concrete"))
            {
                PlayFootstepSound(concrete);
            }
            if(hit.collider.CompareTag("metal1"))
            {
                PlayFootstepSound(metal1);
            }
            if(hit.collider.CompareTag("water"))
            {
                PlayFootstepSound(water);
            }
            if(hit.collider.CompareTag("carpet"))
            {
                PlayFootstepSound(carpet);
            }
        }
    }

    void Update() 
    {
        //Footstep();
    }

    public void PlayFootstepSound(AudioClip audioClip) 
    {
        audio.pitch = Random.Range(0.8f, 1f);
        audio.PlayOneShot(audioClip);

        Debug.Log("AUDIO PLAYED");
    }

    private void OnDrawGizmos()
    {
        if (RayStart != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(RayStart.position, RayStart.position + RayStart.transform.up * range * -1);
        }
    }
}
