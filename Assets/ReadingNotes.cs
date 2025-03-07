using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UIElements;

public class Interactions : MonoBehaviour
{
    [SerializeField] GameObject noteInterface;
    [SerializeField] TextMeshProUGUI noteText;
    [SerializeField] TextMeshProUGUI noteTitle;

    Ray crosshair;
    float range;
    public LayerMask interactableLayer;

    bool reading;

    [SerializeField]  Material screenOff;
    [SerializeField]  Material screenOn;

    // Start is called before the first frame update
    void Start()
    {
        reading = false;

        range = 5.0f;

        noteInterface.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //if the note is on screen, turn it off
            if (noteInterface.activeInHierarchy == true)
            {
                Debug.Log("exiting note");
                reading = false;

                noteInterface.SetActive(false);
            }
            else 
            { 
                crosshair = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //centre of viewport

                if (Physics.Raycast(crosshair, out RaycastHit hit, range, interactableLayer)) //if the ray finds something
                {
                    switch (hit.transform.name)
                    {
                        case ("Quad"):
                            if (hit.transform.gameObject.GetComponent<MeshRenderer>().material == screenOff)
                            {
                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = screenOn;
                            }
                            else
                            {
                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = screenOff;
                            }
                            break;

                        case ("toilet"):
                            //play audio
                            break;

                        default:
                            if (hit.transform.name.StartsWith('n'))
                            {
                                Debug.Log("reading note");
                                reading = true;

                                noteInterface.SetActive(true);

                                editNote(hit.transform.name);
                            }
                            else
                            {
                                Debug.Log("BROKEN");
                            }
                            break;
                    }
                }      
            }
        }
    }

    void editNote(string noteName)
    {
        switch (noteName)
        {
            case ("note 1"):
                noteTitle.text = "Dr. Edwin";
                noteText.text = "Intervallum - Research Department\r\n\r\n" +
                    "We were wrong to think The Thing was a physical entity. " +
                    "It doesn’t just exist here, it exists between. It’s always here but it’s not always here, it’s there too. " +
                    "It’s forcing its way into our reality and bringing all kinds of dangers with it. We're not safe here.";
                break;

            case ("note 2"):
                noteTitle.text = "Captain's Log - Entry 5";
                noteText.text = "The Thing raided the cafeteria. We cant get near the kitchen and we are running out of food supplies.";
                break;

            case ("note 3"):
                noteTitle.text = "Captain's Log - Entry 5";
                noteText.text = "this is note three";
                break;

            default:
                noteTitle.text = "You lost buddy?";
                noteText.text = "I don't think you coded this properly...";
                break;
        }


        //noteText.text = "it broke... what note is this supposed to be?";
/*
        if(noteTitle == "maintenancePaper(1)")
        {
            return "this page is beside the dimension shifter";
        }
        else if (noteTitle == "paper_geo")
        {
            return "this page is not beside the dimension shifter";
        }
        return "broke";
*/
    }
}
