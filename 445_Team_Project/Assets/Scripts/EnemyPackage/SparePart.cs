using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    defaultType
}

public class SparePart : MonoBehaviour
{
    Rigidbody rb;
    bool inRange = false;
    [HideInInspector] public PartType thisPartType = PartType.defaultType;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

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
            Vector3 offset = PlayerCtrl.playerCtrl.transform.position - transform.position;

            //Move towards player
            if (rb.velocity.magnitude < 6f)
            {
                rb.AddForce(offset.normalized * 4, ForceMode.Acceleration);
            }
            //Player pickup when close enough (40cm)
            if (offset.magnitude < .4f)
            {
                Debug.Log("Picked up part: " + thisPartType);
                GameCtrl.AddPartToList(thisPartType);
                Destroy(gameObject);
            }
        }
    }

    
}
