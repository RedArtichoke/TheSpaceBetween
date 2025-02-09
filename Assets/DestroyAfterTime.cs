using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{   
    void Start()
    {
        StartCoroutine(Countdown());
    }

    public IEnumerator Countdown()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
