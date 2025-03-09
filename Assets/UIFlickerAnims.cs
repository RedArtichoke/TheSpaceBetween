using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFlickerAnims : MonoBehaviour
{
    public Animator anim;
    void OnEnable()
    {
        anim.SetTrigger("Flicker");
    }
}
