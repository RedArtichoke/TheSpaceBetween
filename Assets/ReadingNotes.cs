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

    private KeyBindManager keyBindManager;

    void Start()
    {
        keyBindManager = FindObjectOfType<KeyBindManager>();
        reading = false;

        range = 5.0f;

        noteInterface.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyBindManager.interactKey))
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
                    "It doesn�t just exist here, it exists between. It�s always here but it�s not always here, it�s there too. " +
                    "It�s forcing its way into our reality and bringing all kinds of dangers with it. We're not safe here.";
                break;

            case ("note 2"):
                noteTitle.text = "HE'S ALWAYS WATCHING";
                noteText.text = "";
                //picture goes here
                break;

            case ("note 3"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Maintenance Department\r\n\r\n" +
                    "Somebody or.. Something unplugged the maintenance adaptor, cutting power to the research sector.\r\n\r\n" +
                    "Please help me.. I'm pinned and can't move, somebody needs to repair the relay before THE THING finds us.";
                break;

            case ("note 4"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Department\r\n\r\n" +
                    "note 4 content";
                break;

            case ("note 5"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Department\r\n\r\n" +
                    "note 5 content";
                break;

            case ("note 6"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Department\r\n\r\n" +
                    "note 6 content";
                break;

            case ("note 7"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Department\r\n\r\n" +
                    "note 7 content";
                break;

            case ("note 8"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Department\r\n\r\n" +
                    "note 8 content";
                break;

            case ("note 9"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Department\r\n\r\n" +
                    "note 9 content";
                break;

            case ("note 10"):
                noteTitle.text = "";
                noteText.text = "Intervallum - Department\r\n\r\n" +
                    "note 10 content";
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
