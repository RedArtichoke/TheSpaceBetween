using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFlickerAnims : MonoBehaviour
{
    public Animator anim;
    public AudioSource audioLoad;
    void OnEnable()
    {
        anim.SetTrigger("Flicker");
        audioLoad.Play();
    }
}
