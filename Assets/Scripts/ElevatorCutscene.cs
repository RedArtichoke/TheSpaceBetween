using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

    [SerializeField] bool elevatorDone;  //player has already went "down"

    [SerializeField] bool inMotion;

    Transform toShake;
    Vector3 origPos;    //where the elevator is before "falling"

    //should not be able to see before the cutscene happens
    public GameObject skull;
    public GameObject doorBlock;

    //checking against "heldObject" to see if they picked it up
    [SerializeField] PlayerMovementController player;
    [SerializeField] Transform keyItem;
    public LayerMask itemLayer;

    //to open/close during cutscene
    Animator doorAnimator;
    [SerializeField] Light buttonLight;

    //Raycast button press stuff
    float range;                    //interaction range 
    public LayerMask buttonLayer;
    Ray crosshair;                  //this interacts with colliders

    [SerializeField] AudioSource vatorOpen;
    [SerializeField] AudioSource vatorClose;
    [SerializeField] AudioSource vatorNoise;
    [SerializeField] AudioSource vatorShake;
    [SerializeField] AudioSource vatorArrival;

    [SerializeField] AudioSource approach;
    [SerializeField] AudioSource cough;

    public GameObject door;
    public GameObject veil;

    private KeyBindManager keyBindManager;

    void Start()
    {
        keyBindManager = FindObjectOfType<KeyBindManager>();
        doorAnimator = transform.parent.GetComponent<Animator>();
        //skull = transform.parent.GetChild(3).gameObject; //the skull is the 4th child of "Elevator Room";

        skull.SetActive(false);
        doorBlock.SetActive(false);
        keyItem.gameObject.SetActive(false);

        Debug.Log("engine: " + keyItem.gameObject.layer);
        itemLayer = (int)Mathf.Pow(2,keyItem.gameObject.layer); //get the or-ginal object layer
        keyItem.gameObject.layer = 0; //cant pick it up before it gets thrown into the elevator

        toShake = transform.parent;
        origPos = toShake.position;

        range = 5.0f;

        //vatorOpen = transform.parent.GetComponents<AudioSource>()[0];
        //vatorClose = transform.parent.GetComponents<AudioSource>()[1];
        //vatorNoise = transform.parent.GetComponents<AudioSource>()[2];
        //vatorShake = transform.parent.GetComponents<AudioSource>()[3];
        //vatorArrival = transform.parent.GetComponents<AudioSource>()[4];
    }

    private void Update()
    {
        //if the cutscene has not happened yet
        if (Input.GetKeyDown(keyBindManager.interactKey) && !elevatorDone)
        {
            crosshair = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //centre of viewport

            //if they hit an elevator button (are now highlighted)
            if (Physics.Raycast(crosshair, out RaycastHit hit, range, buttonLayer) && hit.transform.name.Contains("geo_elevator_panel")) 
            {
                //Debug.Log(hit.transform.name); //what was pressed?
                
                StartCoroutine(ElevatorSequence());
            }
        }
        /*
        if((keyItem == player.GetHeldObject()) && !elevatorDone)
        {
            StartCoroutine(ElevatorUp());
        }*/
        
        if (inMotion)
        {
            rumble();
        }
    }

    public IEnumerator ElevatorSequence()
    {
        buttonLight.enabled = false;
        buttonLight.transform.parent.parent.gameObject.layer = 0;
        //START SEQUENCE
        vatorClose.Play();
        doorAnimator.SetBool("MotionStart", true);

        yield return new WaitForSeconds(2.0f);

        //ELEVATOR DESCENDING SEQUENCE
        vatorShake.Play();
        vatorNoise.Play();

        Debug.Log("and now you fall...");
        inMotion = true;
        yield return new WaitForSeconds(4.0f);
        inMotion = false;

        vatorShake.Stop();
        vatorNoise.Stop();

        //turn on fog, door light should not be seen
        RenderSettings.fog = true;  //consider adding reference to if in the Dark
        RenderSettings.fogStartDistance = 3f;
        RenderSettings.fogEndDistance = 7f;

        //REACHING THE BOTTOM FLOOR
        vatorArrival.Play();

        skull.SetActive(true);
        keyItem.gameObject.SetActive(true);
        doorBlock.SetActive(true);

        toShake.position = origPos; //falling has stopped
        yield return new WaitForSeconds(1.0f);

        //DOORS OPENING
        vatorOpen.Play();
        doorAnimator.SetBool("MotionStart", false);
        doorAnimator.SetBool("Arrived", true);

        yield return new WaitForSeconds(4.0f);
        Debug.Log("The skull moves forward with the engine");

        //skull.GetComponent<Rigidbody>().velocity = Vector3.back;
        //keyItem.GetComponent<Rigidbody>().velocity = Vector3.back;
        //approach.Play();

        yield return new WaitForSeconds(3.0f);

        keyItem.GetComponent<Rigidbody>().velocity = 10 * Vector3.back;
        keyItem.gameObject.layer = (int)Mathf.Log(itemLayer,2); //let them pick it up

        yield return new WaitForSeconds(2.0f);
        //cough.Play();
        yield return new WaitForSeconds(1.0f);

        elevatorDone = true; //only trigger scene once because you get the item
        skull.GetComponent<Rigidbody>().velocity = Vector3.forward;
        yield return new WaitForSeconds(4.0f);

        //ITEM IN ELEVATOR
        vatorClose.Play();

        doorAnimator.SetBool("Arrived", false);
        doorAnimator.SetBool("MotionStart", true);
        yield return new WaitForSeconds(1.0f);

        //ELEVATOR ASCENDING SEQUENCE
        vatorShake.Play();
        vatorNoise.Play();
        //Debug.Log("and now you rise...");

        inMotion = true;
        yield return new WaitForSeconds(4.0f);
        inMotion = false;
        toShake.position = origPos; //rising has stopped

        skull.SetActive(false);
        doorBlock.SetActive(false);

        vatorShake.Stop();
        vatorNoise.Stop();


        //ELEVATOR ARRIVES AT TOP FLOOR
        yield return new WaitForSeconds(1.0f);
        vatorArrival.Play();

        //DOORS OPEN AND THEY GET ON WITH THE GAME
        yield return new WaitForSeconds(1.0f);
        vatorOpen.Play();

        doorAnimator.SetBool("MotionStart", false);
        doorAnimator.SetBool("Arrived", true);

        door.SetActive(true);
        veil.SetActive(false);

        RenderSettings.fog = false;
    }

    void rumble()
    {
        float magnitude = 0.1f;

        toShake.position = new Vector3(
            Random.Range(-magnitude, magnitude) + origPos.x,
            Random.Range(-magnitude, magnitude) + origPos.y,
            Random.Range(-magnitude, magnitude) + origPos.z
        );
    }
}