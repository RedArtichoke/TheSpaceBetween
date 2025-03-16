using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] Transform noteHolder;
    public List<Transform> notes; //list of notes

    [SerializeField] GameObject InventoryCanvas;

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

        InventoryCanvas = transform.GetChild(0).gameObject;
        InventoryCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown( /*Blake Key Binds*/ KeyCode.Tab))
        {
            toggleNoteUI();
        }
    }

    void toggleNoteUI()
    {
        if (!Cursor.visible)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            InventoryCanvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InventoryCanvas.SetActive(false);
        }
    }
}
