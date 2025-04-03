using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    Image Thing;

    Ray crosshair;
    float range;
    public LayerMask interactableLayer;

    public bool reading;
    public int notesCollected;

    [SerializeField] GameObject foundNotes;
    [SerializeField] List<Transform> noteButtons = new(10);

    [SerializeField] AudioSource pageTurn;

    [SerializeField]  Material screenOff;
    [SerializeField]  Material screenOn;

    private KeyBindManager keyBindManager;

    private GameObject interactCanvas;

    [SerializeField] ControlPromptAnimator notePrompt;

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
        notesCollected = 0;

        Thing = noteText.transform.parent.GetChild(2).GetComponent<Image>();
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

                if (notesCollected == 1) //after collecting the first note
                {
                    notePrompt.RevealWithAnimation();
                }
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
                                int indexNum = noteIndex[noteIndex.Length - 1] - '0' - 1;
                                if (indexNum < 0){ indexNum = 10; }
                                reading = true;

                                noteInterface.SetActive(true);
                                UIBlur.SetActive(true);

                                editNote(hit.transform.name, noteTitle, noteText, Thing);

                                Transform selectedNote = noteButtons[indexNum];

                                //index is last number of note
                                if (selectedNote != null)
                                {
                                    if(!selectedNote.GetComponent<Button>().interactable)
                                    {
                                        //Debug.Log("reached here");
                                        selectedNote.GetComponent<Button>().interactable = true;
                                        selectedNote.GetChild(0).GetComponent<TextMeshProUGUI>().text = noteTitle.text;
                                    }

                                    pageTurn.Play();
                                    notesCollected++;
                                    Destroy(hit.transform.parent.gameObject);
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

    public void editNote(string noteName, TextMeshProUGUI title, TextMeshProUGUI body, Image thing)
    {
        if(thing.gameObject.activeInHierarchy)
        {
            thing.gameObject.SetActive(false);
        }

        switch (noteName)
        {
            case ("note 1"):
                title.text = "Dr. James";
                body.text = "Intervallum - Research Department\n\n" +
                    "We were wrong to think The Thing was a physical entity. " +
                    "It doesn't just exist here, it exists between. It's always here but it's not always here, it's there too. " +
                    "It's forcing its way into our reality and bringing all kinds of dangers with it. We're not safe here.";
                break;

            case ("note 2"):
                title.text = "HE'S ALWAYS WATCHING";
                body.text = "";
                //picture goes here
                thing.gameObject.SetActive(true);
                break;

            case ("note 3"):
                title.text = "Unknown Threat";
                body.text = "Intervallum - Maintenance Department\r\n\r\n" +
                    "Somebody or.. Something unplugged the maintenance adaptor, cutting power to the research sector.\r\n\r\n" +
                    "Please help me.. I'm pinned and can't move, somebody needs to repair the relay before THE THING finds us.";
                break;

            case ("note 4"):
                title.text = "A Moments Rest";
                body.text = "The Bathrooms are the only place I feel safe here. No weird noises, nothing moves around. It is quiet and peaceful," +
                    " and it is the only place on this ship where it feels like I am alone.\r\nWhere I am not being watched." +
                    " By anyone or anything. \r\nThey leave me alone in here.";
                break;

            case ("note 5"):
                title.text = "";
                body.text = "Intervallum - Canteen\r\n\r\n" +
                    "They are above us now...";
                break;

            case ("note 6"):
                title.text = "Dark Mites";
                body.text = "These creatures are emerging from the dark. As THE THING hunts us down, they cover us with this black goo " +
                    "and leave puddles all over the ship.\n\n If we're not careful, it could be the end of us.";
                break;

            case ("note 7"):
                title.text = "In The Dark";
                body.text = "The dark is supposed to be empty. But the silence isn’t quiet. If you listen too long it starts to sound like words. " +
                    "But not from any language I know. It’s speaking. I don’t know how, and I don’t know why. " +
                    "\r\nAnd everytime I go back, the silence starts to sound more like us.";
                break;

            case ("note 8"):
                title.text = "Personal Log 0X8";
                body.text = "The furniture keeps moving. It feels like I am going insane. Everytime I tell someone, it has already moved again. " +
                    "Or maybe something else is out of place. But I am starting to doubt myself as well. It started off small and happens " +
                    "very rarely, but it feels like its becoming more and more frequent and I don’t know what to do.";
                break;

            case ("note 9"):
                title.text = "Personal Log 0X9";
                body.text = "It finally happened. Finally, FINALLY the Doctor saw something move. Someone OTHER THAN ME saw something move!" +
                    " I’m not insane! I’m really not. \n\nBut now the furniture seems to be moving more… “coordinated”…?\r\n\r\n" +
                    "What am I talking about? Maybe it's just me again.\r\n";
                break;

            case ("note 10"):
                title.text = "A new Dimension";
                body.text = "The stars look wrong. I think we’re lost. I can’t figure out where we are and I don’t know how to escape. " +
                    "Sometimes they flicker, and sometimes they disappear… it feels like they’ve been replaced, or like there’s " +
                    "something dark stuck between us. Between everything. \r\n\r\nI can’t tell what’s real anymore.";
                break;

            default:
                title.text = "You lost buddy?";
                body.text = "I don't think you coded this properly...";
                break;
        }

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
