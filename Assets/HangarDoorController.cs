using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarDoorController : MonoBehaviour
{
    public GameObject leftHangarDoor;
    public GameObject rightHangarDoor;
    public float doorMoveDistance = 16.8589f;
    public float moveDuration = 6f; 

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            OpenHangarDoors();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            CloseHangarDoors();
        }
    }
    public void OpenHangarDoors()
    {
        StartCoroutine(MoveDoor(leftHangarDoor.transform, Vector3.right * doorMoveDistance, moveDuration));
        StartCoroutine(MoveDoor(rightHangarDoor.transform, Vector3.left * doorMoveDistance, moveDuration));
    }

    public void CloseHangarDoors()
    {
        StartCoroutine(MoveDoor(leftHangarDoor.transform, Vector3.left * doorMoveDistance, moveDuration));
        StartCoroutine(MoveDoor(rightHangarDoor.transform, Vector3.right * doorMoveDistance, moveDuration));
    }

    private IEnumerator MoveDoor(Transform door, Vector3 moveBy, float duration)
    {
        Vector3 startPosition = door.position;
        Vector3 targetPosition = startPosition + moveBy;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            door.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        door.position = targetPosition; 
    }
}