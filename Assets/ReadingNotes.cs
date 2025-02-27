using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactions : MonoBehaviour
{
    [SerializeField] GameObject noteInterface;
    [SerializeField] TextMeshProUGUI noteText;

    Ray crosshair;
    float range;
    public LayerMask noteLayer;

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
            crosshair = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //centre of viewport

            if (Physics.Raycast(crosshair, out RaycastHit hit, range, noteLayer)) //if the ray finds something
            {
                Debug.Log(hit.transform.name);

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
                        if (noteInterface.activeInHierarchy == true)
                        {
                            Debug.Log("exiting note");
                            reading = false;

                            noteInterface.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("reading note");
                            reading = true;

                            noteInterface.SetActive(true);

                            noteText.text = findNote(hit.transform.name);
                        }
                        break;
                }
            }
        }
    }

    string findNote(string noteTitle)
    {
        Debug.Log("AHHH " + noteTitle[noteTitle.Length - 1]);

        int noteNum = noteTitle[noteTitle.Length - 1];

        //Debug.Log(noteNum);

        switch (noteNum)
        {
            case (0):
                return "idk bro";
            case (1):
                return "this is note one";
            case (2):
                return "this is note two";
        }


        return "it broke... what note is this supposed to be?";
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
