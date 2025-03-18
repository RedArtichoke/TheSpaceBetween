using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class ReadingNotes : MonoBehaviour
{
    [SerializeField] GameObject UIBlur;
    [SerializeField] GameObject noteInterface;
    [SerializeField] MenuController menu;
    
    [SerializeField] TextMeshProUGUI noteText;
    [SerializeField] TextMeshProUGUI noteTitle;

    Ray crosshair;
    float range;
    public LayerMask interactableLayer;

    public bool reading;

    [SerializeField] GameObject foundNotes;
    [SerializeField] List<Transform> noteButtons = new(10);

    [SerializeField] AudioSource pageTurn;

    [SerializeField]  Material screenOff;
    [SerializeField]  Material screenOn;

    private KeyBindManager keyBindManager;

    public GameObject interactCanvas;

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
        if (Input.GetKeyDown(KeyCode.Escape) && noteInterface.activeInHierarchy)
        {
            reading = false;
            noteInterface.SetActive(false); //turn off canvas if game is paused
        }
        if (Input.GetKeyDown(keyBindManager.interactKey) && !menu.isPaused)
        {
            //if the note is on screen, turn it off
            if (noteInterface.activeInHierarchy == true)
            {
                reading = false;
                Time.timeScale = 1f;
                Debug.Log("exiting note");
                
                UIBlur.SetActive(false);
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
                                interactCanvas = hit.transform.GetChild(0).GetChild(1).gameObject; // object: "note/InteractPrompt"
                                if (interactCanvas != null)
                                {
                                    Debug.Log(interactCanvas.name);
                                    Destroy(interactCanvas);
                                }

                                Time.timeScale = 0f;
                                string noteIndex = hit.transform.name;
                                reading = true;

                                noteInterface.SetActive(true);
                                UIBlur.SetActive(true);

                                editNote(hit.transform.name, noteTitle, noteText);

                                Transform selectedNote = noteButtons[noteIndex[noteIndex.Length - 1] - '0' - 1];

                                //index is last number of note
                                if (selectedNote != null)
                                {
                                    if (noteIndex[noteIndex.Length - 1] - '0' < 0)
                                    {
                                        noteButtons[9].GetComponent<Button>().interactable = true; //for note 10
                                    }
                                    else if(!selectedNote.GetComponent<Button>().interactable)
                                    {
                                        //Debug.Log("reached here");
                                        selectedNote.GetComponent<Button>().interactable = true;
                                        selectedNote.GetChild(0).GetComponent<TextMeshProUGUI>().text = noteTitle.text;
                                    }
                                }
                                else
                                {
                                    Debug.Log("broken...");
                                }   
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
                title.text = "Dr. James";
                body.text = "Intervallum - Research Department\r\n\r\n" +
                    "We were wrong to think The Thing was a physical entity. " +
                    "It doesn't just exist here, it exists between. It's always here but it's not always here, it's there too. " +
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
                body.text = "Intervallum - Maintenance Washroom\r\n\r\n" +
                    "They are above us now...";
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
