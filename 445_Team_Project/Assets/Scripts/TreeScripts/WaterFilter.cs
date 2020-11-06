using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFilter : MonoBehaviour
{
    Collider myCollider;
    Rigidbody rb;
    OVRGrabbable grabbable;
    bool floating = true;
    bool thrown = false;
    bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        //Add to GameCtrl List
        GameCtrl.AddWaterFilterToList(this);

        //Prevent player collision
        grabbable = GetComponent<OVRGrabbable>();
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        /*
        Collider [] playerColliders = PlayerCtrl.playerCtrl.gameObject.GetComponentsInChildren<Collider>(true);
        foreach (Collider playerCol in playerColliders)
        {
            string goName = playerCol.gameObject.name;
            if (!(goName == "LeftHandAnchor" || goName == "RightHandAnchor")) Physics.IgnoreCollision(myCollider, playerCol, true);
        }*/
    }

    //To ensure OVRGrabbable doesn't interfere: continously check whether grabbed & enabled gravity once grabbed.
    //Also prevents collissions while floating in the air.
    private void Update()
    {
        if (floating)
        {
            if (grabbable.isGrabbed)
            {
                floating = false;
                rb.useGravity = true;
            }
        }
        else
        {
            if (!grabbable.isGrabbed && !thrown)
            {
                thrown = true;
                myCollider.isTrigger = false;
                rb.useGravity = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            GroundCollision();
        }
    }

    private void GroundCollision()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        RaycastHit hit;
        // Does the ray intersect any objects on the ground layer
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            //Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

            //Place at hit
            rb.isKinematic = true;
            transform.position = hit.point + Vector3.up * .4f;
            transform.rotation = Quaternion.identity;

        }
        else
        {
            //Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
}
