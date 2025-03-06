using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactions : MonoBehaviour
{
    [SerializeField] GameObject noteInterface;
    [SerializeField] TextMeshProUGUI noteText;
    [SerializeField] TextMeshProUGUI noteTitle;

    Ray crosshair;
    float range;
    public LayerMask interactableLayer;

    bool reading;

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
                        case ("maintenanceComputer_grp"):
                            if (hit.transform.GetChild(0).gameObject.activeInHierarchy == true)
                            {
                                hit.transform.GetChild(0).gameObject.SetActive(false);
                            }
                            else
                            {
                                hit.transform.GetChild(0).gameObject.SetActive(true);
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
                noteTitle.text = "Research Log - Entry 1";
                noteText.text = "theres a big scary monster in the other dimension :(";
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
