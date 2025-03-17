using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class ReadingNotes : MonoBehaviour
{
    [SerializeField] GameObject noteInterface;
    
    [SerializeField] TextMeshProUGUI noteText;
    [SerializeField] TextMeshProUGUI noteTitle;

    Ray crosshair;
    float range;
    public LayerMask interactableLayer;

    bool reading;

    [SerializeField] GameObject foundNotes;
    [SerializeField] List<Transform> noteButtons = new(10);

    [SerializeField] AudioSource pageTurn;

    [SerializeField]  Material screenOff;
    [SerializeField]  Material screenOn;

    private KeyBindManager keyBindManager;

    void Start()
    {
        keyBindManager = FindObjectOfType<KeyBindManager>();
        reading = false;

        range = 5.0f;

        noteInterface.SetActive(false);

        for (int i = 0; i < 10; i++)
        { 
            noteButtons.Add(foundNotes.transform.GetChild(i));
        }
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
                                string noteIndex = hit.transform.name;
                                reading = true;

                                noteInterface.SetActive(true);

                                //Debug.Log(noteIndex[noteIndex.Length - 1]);

                                //index is last number of note
                                if (noteButtons[noteIndex[noteIndex.Length - 1] - '0' - 1] != null)
                                {
                                    if (noteIndex[noteIndex.Length - 1] - '0' < 0)
                                    {
                                        noteButtons[9].GetComponent<Button>().interactable = true; //for note 10
                                    }
                                    else
                                    {
                                        //Debug.Log("reached here");
                                        noteButtons[noteIndex[noteIndex.Length - 1] - '0' - 1].GetComponent<Button>().interactable = true;
                                    }
                                }
                                else
                                {
                                    Debug.Log("broken...");
                                }

                                editNote(hit.transform.name, noteTitle, noteText);
                            }
                            break;
                    }
                }      
            }
        }
    }

    public void editNote(string noteName, TextMeshProUGUI title, TextMeshProUGUI body)
    {
        pageTurn.Play();

        switch (noteName)
        {
            case ("note 1"):
                title.text = "Dr. Edwin";
                body.text = "Intervallum - Research Department\r\n\r\n" +
                    "We were wrong to think The Thing was a physical entity. " +
                    "It doesn�t just exist here, it exists between. It�s always here but it�s not always here, it�s there too. " +
                    "It's forcing its way into our reality and bringing all kinds of dangers with it. We're not safe here.";
                break;

            case ("note 2"):
                title.text = "HE'S ALWAYS WATCHING";
                body.text = "";
                //picture goes here
                break;

            case ("note 3"):
                title.text = "";
                body.text = "Intervallum - Maintenance Department\r\n\r\n" +
                    "Somebody or.. Something unplugged the maintenance adaptor, cutting power to the research sector.\r\n\r\n" +
                    "Please help me.. I'm pinned and can't move, somebody needs to repair the relay before THE THING finds us.";
                break;

            case ("note 4"):
                title.text = "";
                body.text = "Intervallum - Department\r\n\r\n" +
                    "note 4 content";
                break;

            case ("note 5"):
                title.text = "";
                body.text = "Intervallum - Department\r\n\r\n" +
                    "note 5 content";
                break;

            case ("note 6"):
                title.text = "";
                body.text = "Intervallum - Department\r\n\r\n" +
                    "note 6 content";
                break;

            case ("note 7"):
                title.text = "";
                body.text = "Intervallum - Department\r\n\r\n" +
                    "note 7 content";
                break;

            case ("note 8"):
                title.text = "";
                body.text = "Intervallum - Department\r\n\r\n" +
                    "note 8 content";
                break;

            case ("note 9"):
                title.text = "";
                body.text = "Intervallum - Department\r\n\r\n" +
                    "note 9 content";
                break;

            case ("note 10"):
                title.text = "";
                body.text = "Intervallum - Department\r\n\r\n" +
                    "note 10 content";
                break;

            default:
                title.text = "You lost buddy?";
                body.text = "I don't think you coded this properly...";
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

    void addToInventory(int noteNum)
    {
        TextMeshProUGUI noteInfo = foundNotes.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(foundNotes.transform.GetChild(0))
        {
            noteInfo.text = noteTitle.text;
        }
        
    }
}
