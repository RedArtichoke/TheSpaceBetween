using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpen : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
    }
}
