using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatingDoorOpen : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;  
    private float closeDelay = 5f;  

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
    }

}

