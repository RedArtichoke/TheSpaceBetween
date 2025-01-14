using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCutscene : MonoBehaviour
{
    //ADDITIONAL NOTES
    //ask yogurt about how to crosshair button press
    //figure out how to temporarily disable controls
    //lighting should be different when they see the skull to show that the elevator took them to a "different" area

    /*
    ** SCENE WALKTHROUGH **
    upon button press, the doors shut and the cutscene starts.
    player cannot exit the elevator and camera does movement things to show its going to "a different floor".
    once the camera stops, the door opens and the skull, as well as the key item, is seen just outside the doors.
    (prevent player from leaving the elevator?) 
    once the player grabs the item, the doors close again and the elevator takes them back to the room that they started in.
    
     */

    [SerializeField]
    bool elevatorDone;  //has cutscene played
    bool inMotion;      //is the elevator moving?

    Vector3 origPos;    //where the elevator is before "falling"

    //should not be able to see before the cutscene happens
    public GameObject skull;
    public GameObject keyItem;

    //to limit their controls during cutscene
    public GameObject player;

    //to open/close during cutscene
    public GameObject doorL; 
    public GameObject doorR;
    float slideDistance; //how far the doors open/close

    //Raycast button press stuff
    float range;                    //interaction range 
    public LayerMask buttonLayer;
    Ray crosshair;                  //this interacts with colliders


    private void Start()
    {
        origPos = gameObject.transform.position;

        range = 5.0f;
        slideDistance = 0.8f;

        skull.SetActive(false);
        keyItem.SetActive(false);
    }

    private void Update()
    {
        //if the cutscene has not happened yet
        if (Input.GetKeyDown(KeyCode.E) && !elevatorDone)
        {
            crosshair = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //centre of viewport

            if (Physics.Raycast(crosshair, out RaycastHit hit, range, buttonLayer)) //if they hit a button
            {
                //Debug.Log(hit.transform.name); //what was pressed?

                StartCoroutine(ElevatorStart());
            }
        }

        if (inMotion)
        {
            rumble();
        }
    }
    
    IEnumerator ElevatorStart()
    {

        //find a different way so they slide closed
        //one of the is flipped 180 degress so they move in the "same direction"
        doorL.transform.Translate(Vector3.left * slideDistance);
        doorR.transform.Translate(Vector3.left * slideDistance);

        yield return new WaitForSeconds(2.0f);

        //ELEVATOR DESCENDING SEQUENCE
        Debug.Log("and now you fall...");
        inMotion = true;
        yield return new WaitForSeconds(4.0f);
        inMotion = false;

        //DOORS OPEN TO SKULL AND ITEM
        skull.SetActive(true);
        keyItem.SetActive(true);
        gameObject.transform.position = origPos; //falling has stopped

        //A STATEMENT TO CHECK IF THE PLAYER GRABBED THE ITEM

        //ELEVATOR ASCENDING SEQUENCE
        skull.SetActive(false);

        //DOORS OPEN AND THEY GET ON WITH THE GAME
        doorL.transform.Translate(Vector3.right * slideDistance);
        doorR.transform.Translate(Vector3.right * slideDistance);
        //elevatorDone = true;
    }
    
    void rumble()
    {
        float magnitude = 0.1f;

        gameObject.transform.position = new Vector3(
            Random.Range(-magnitude, magnitude) + origPos.x,
            Random.Range(-magnitude, magnitude) + origPos.y,
            Random.Range(-magnitude, magnitude) + origPos.z);
    }
}
