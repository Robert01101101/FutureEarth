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
            StartCoroutine(SpawnTrees());
        }
    }

    //Not really needed but since not all collisions were being registered & seed has short lifetime, this didn't hurt to add.
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 8 && !done)
        {
            StartCoroutine(SpawnTrees());
        }
    }

    IEnumerator SpawnTrees()
    {
        //spawn one tree where the seed hit & two more with offsets
        done = true;
        SpawnTree(new Vector3(transform.position.x, transform.position.y - .2f, transform.position.z));
        for (int i=0; i<2; i++)
        {
            yield return new WaitForSeconds(.6f);
            Vector2 offset = Random.insideUnitCircle * 1.5f;
            SpawnTree(new Vector3(transform.position.x + offset.x, transform.position.y - .2f, transform.position.z + offset.y));
        }
        Destroy(gameObject);
    }

    private void SpawnTree(Vector3 pos)
    {
        GameObject treeInstance = Instantiate(treePrefab, pos, Quaternion.identity);
        treeInstance.GetComponent<Tree>().Init();
        GameCtrl.gameCtrl.gameObject.GetComponent<lb_BirdController>().TreeSpawn();
    }
}
