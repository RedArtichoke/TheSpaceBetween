using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpen : MonoBehaviour
{
    private Animator animator;
    public bool IsLocked = false;  
    public Light doorLight;  
    public Light doorLight2;  
    private bool isOpen = false;  
    private float closeDelay = 5f;  

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateLightColor(); 
    }

    public void OpenDoor()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
            StartCoroutine(CloseDoorAfterDelay());
        }
    }

    public void SetLockState(bool locked)
    {
        IsLocked = locked;
        UpdateLightColor();
    }

    private void UpdateLightColor()
    {
        if (doorLight != null)
        {
            doorLight.color = IsLocked ? Color.red : Color.green;
            doorLight2.color = IsLocked ? Color.red : Color.green;
        }
    }

    private IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        if (animator != null && isOpen)
        {
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }
}
