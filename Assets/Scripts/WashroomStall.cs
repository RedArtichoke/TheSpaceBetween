using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashroomStall : MonoBehaviour
{
    public bool isClosed = true;
    public void useDoor(Transform stallDoor)
    {
        Animator hinge = stallDoor.GetComponent<Animator>();
        AudioSource[] doorAudio = stallDoor.GetComponents<AudioSource>();

        if (hinge == null)
        {
            hinge = stallDoor.parent.GetComponent<Animator>();
        }
        //Debug.Log("stall 2");

        if (hinge.GetBool("closed"))
        {
            //Debug.Log("stall 3");
            hinge.SetBool("closed", false);
            isClosed = false;

            if (doorAudio[0] != null)
            {
                doorAudio[0].Play(); //open audio
            }
        }
        else
        {
            //Debug.Log("stall 4");
            hinge.SetBool("closed", true);
            isClosed = true;

            if (doorAudio[1] != null)
            {
                doorAudio[1].Play(); //close audio
            }
        }

    }
}
