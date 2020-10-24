using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created for A3. Seed class, responsible for:
/// - Float (no gravity) until grabbed by player
/// - Spawn tree on ground collision.
/// - Store type of tree selected & pass it to Tree instance
/// </summary> 

public class Seed : MonoBehaviour
{
    //Public fields
    public GameObject treePrefab;

    //Private fields
    Rigidbody rb;
    OVRGrabbable grabbable;
    bool floating = true;
    bool thrown = false;
    bool done = false;
    int type;

    //Define what kind of tree it will spawn (TODO before project integration: change to enum)
    public void SetType(int newType)
    {
        type = newType;
    }

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        rb = GetComponent<Rigidbody>();
        GameObject clippyUI = GameObject.Find("ClippyUI");
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
        } else
        {
            if (!grabbable.isGrabbed && !thrown)
            {
                thrown = true;
                GetComponent<CapsuleCollider>().isTrigger = false;
                rb.useGravity = true;
            }
        }
    }

    //Spawn tree on collision with ground & destroy seed instance
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            done = true;
            GameObject treeInstance = Instantiate(treePrefab, new Vector3(transform.position.x, transform.position.y-.2f, transform.position.z),
                   Quaternion.identity);
            treeInstance.GetComponent<Tree>().SetTreeType(type);
            GameObject.Find("_livingBirdsController").GetComponent<lb_BirdController>().TreeSpawn();
            Destroy(gameObject);
        }
    }

    //Not really needed but since not all collisions were being registered & seed has short lifetime, this didn't hurt to add.
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 8 && !done)
        {
            done = true;
            GameObject treeInstance = Instantiate(treePrefab, new Vector3(transform.position.x, transform.position.y - .2f, transform.position.z),
                   Quaternion.identity);
            treeInstance.GetComponent<Tree>().SetTreeType(type);
            GameObject.Find("_livingBirdsController").GetComponent<lb_BirdController>().TreeSpawn();
            Destroy(gameObject);
        }
    }
}
