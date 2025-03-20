using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] MenuController menu;
    [SerializeField] Transform noteHolder;
    List<Transform> notes; //list of notes

    [SerializeField] ReadingNotes noteInfo;

    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] GameObject inventoryPaper;
    [SerializeField] GameObject UIComponents;
    [SerializeField] GameObject UIBlur;
    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject noteInterface;

    [SerializeField] AudioSource click;
    [SerializeField] AudioSource page;

    private KeyBindManager keyBindManager;

    // Start is called before the first frame update
    void Start()
    {
        notes = new List<Transform>(10);

        for(int i=0; i < noteHolder.childCount; i++)
        {
            //no notes have been found at the start of the game
            notes.Add(noteHolder.GetChild(i));
            notes[i].GetComponent<Button>().interactable = false;
        }

        //inventoryCanvas = transform.GetChild(0).gameObject;
        inventoryCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !menu.isPaused)
        {
            toggleNoteUI(); //if game is not paused, open notes
        }

        if(Input.GetKeyDown(KeyCode.Escape) && inventoryCanvas.activeInHierarchy)
        {
            inventoryCanvas.SetActive(false); //turn off canvas if game is paused
        }
    }

    public void displayButtonInfo(int num)
    {
        //create variables for finding the note
        string noteName = "note " + num;
        List<TextMeshProUGUI> invPaper = new();

        //add the title and body from the paper
        invPaper.Add(inventoryPaper.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        invPaper.Add(inventoryPaper.transform.GetChild(1).GetComponent<TextMeshProUGUI>());

        page.Play();
        noteInfo.editNote(noteName, invPaper[1], invPaper[0]);
    }

    void toggleNoteUI()
    {
        if (!Cursor.visible)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inventoryCanvas.SetActive(true);
            UIComponents.SetActive(false);
            UIBlur.SetActive(true);
            noteInterface.SetActive(false);
            crosshair.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inventoryCanvas.SetActive(false);
            UIComponents.SetActive(true);
            UIBlur.SetActive(false);
            crosshair.SetActive(true);
        }
        if (noteInfo.notesCollected <= 0) //if no notes have been collected
        {
            inventoryPaper.SetActive(false);
        }
        else if (!inventoryPaper.activeInHierarchy)
        {
            inventoryPaper.SetActive(true);
        }
    }
}
