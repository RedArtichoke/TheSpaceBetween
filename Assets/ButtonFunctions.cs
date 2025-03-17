using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] AudioSource hover;
    //[SerializeField] AudioSource click;

    //[SerializeField] Button noteBtn;

    void Start()
    {
        //noteBtn = gameObject.GetComponent<Button>();
    }

    void OnMouseOver()
    {
        hover.Play();
    }
}
