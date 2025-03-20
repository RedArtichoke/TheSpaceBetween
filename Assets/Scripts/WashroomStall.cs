using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashroomStall : MonoBehaviour
{
    public void useDoor(Transform stallDoor)
    {
        Animator hinge = stallDoor.parent.GetComponent<Animator>();
        if(hinge == null)
        {
            return; //do not continue if there is no animator
        }
        Debug.Log("stall 2");

        if (hinge.GetBool("closed"))
        {
            Debug.Log("stall 3");
            hinge.SetBool("closed", false);
        }
        else
        {
            Debug.Log("stall 4");
            hinge.SetBool("closed", true);
        }
        
    }
}
