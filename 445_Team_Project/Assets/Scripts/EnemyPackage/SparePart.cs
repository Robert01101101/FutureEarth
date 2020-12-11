using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    chip,
    tube,
    pump
}

public class SparePart : MonoBehaviour
{
    bool inRange = false;
    bool pickup = false;
    public PartType thisPartType;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Player in range
            inRange = true;
        }
    }

    void Update()
    {
        if (inRange)
        {
            pickup = true;
            StartCoroutine(Pickup());
        }
    }

    IEnumerator Pickup()
    {
        float time = 3;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            
            float curSpeed = Mathf.SmoothStep(0,.1f, (elapsedTime / time));
            transform.position = Vector3.MoveTowards(transform.position, PlayerCtrl.playerCtrl.transform.position, curSpeed);

            if ((transform.position - PlayerCtrl.playerCtrl.transform.position).magnitude < .5f) elapsedTime = time;

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while ((transform.position - PlayerCtrl.playerCtrl.transform.position).magnitude > .5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerCtrl.playerCtrl.transform.position, .3f);
        }

        EndOfPickup();
    }

    private bool pickedUp = false;
    private void EndOfPickup()
    {
        if (!pickedUp)
        {
            pickedUp = true;
            Debug.Log("Picked up part: " + thisPartType);
            GameCtrl.AddPartToList(thisPartType);
            PlayerCtrl.playerCtrl.PlayPickupSound();
            Destroy(gameObject);
        }
    }

    
}
