using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To prevent setting up the GameObject wrong. It's still reccommended to attach and configure the other components, but this prevents fatal errors if forgotten.
[RequireComponent(typeof(Collider))]

/// <summary>
/// This class is attached to every tree grown. It handles:
/// -  Tree growth
/// -  Tree rotation
/// -  Tree health
/// 
/// TODO: Add tree health status, change appearance when health poor, etc.
/// </summary> 

public class Tree : MonoBehaviour
{
    //Public fields
    public GameObject[] treeTypes;
    public Material dryLeavesMaterial;
    public GameObject grassPrefab;

    //Private fields
    Transform treeTransform;
    int turnDirection;
    CapsuleCollider treeCollider;
    private GameObject grassParentInstance;

    bool dead = false;
    bool growing = false;


    ///////////////////////////////////////////////////////// TREE GROWTH /////////////////////////////////////////
    //Init method: define tree type, start tree growth (grow in size, rotate)
    public void Init()
    {
        //Add to GameCtrl List
        GameCtrl.AddTreeToList(this);

        //Pick tree type
        GameObject pick = treeTypes[Random.Range(0, treeTypes.Length)];
        GameObject treeInstance = Instantiate(pick, transform.position, Quaternion.identity, transform);
        treeInstance.transform.SetParent(transform);

        //assign transform variable & set size to 0
        treeTransform = gameObject.transform;
        gameObject.transform.localScale = Vector3.zero;

        //Randomize: turn either left or right & select random mesh
        turnDirection = (Random.value < 0.5) ? -1 : 1;

        //Start growth
        treeCollider = GetComponent<CapsuleCollider>();
        StartCoroutine(TreeLerpSize(5, false));
    }


    ///////////////////////////////////////////////////////// TREE DEATH /////////////////////////////////////////
    public void StartDeath()
    {
        if (!growing) StartCoroutine(TreeDeath());
        dead = true;

        
    }

    IEnumerator TreeDeath()
    {
        //Remove grass
        Destroy(grassParentInstance);

        //TODO: make birds fly off
        //remove bird perch targets from birdController
        SphereCollider[] perchTargets;
        perchTargets = GetComponentsInChildren<SphereCollider>();
        foreach (SphereCollider collider in perchTargets)
        {
            collider.gameObject.tag = "Untagged";
            GameCtrl.birdCtrl.RemovePerchTarget(collider.gameObject);
        }

        //Leaves turn yellow
        yield return new WaitForSeconds(2);

        //If this line causes a bug, it's because a Tree has perchTargets as the first child. To fix, swap it with the tree model in the hierarchy.
        GameObject leaves = gameObject.transform.GetChild(0).GetChild(0).Find("leaves").gameObject;
        leaves.GetComponent<MeshRenderer>().material = dryLeavesMaterial;

        //Leaves disappear
        yield return new WaitForSeconds(5);
        leaves.SetActive(false);

        //Base disappears
        yield return new WaitForSeconds(10);
        StartCoroutine(TreeLerpSize(20, true));
    }

    ///////////////////////////////////////////////////////// Util /////////////////////////////////////////
    IEnumerator TreeLerpSize (float time, bool death)
    {
        ////////////// before main loop
        float startSize, endSize;
        float elapsedTime = 0;
        if (death)
        {
            startSize = 1;
            endSize = 0;
        } else
        {
            growing = true;
            startSize = 0;
            endSize = 1;

            //Activate collider
            treeCollider.enabled = true;
        }

        ////////////// Main loop: Handle growth / shrinking
        while (elapsedTime < time)
        {
            //Lerp is a function to interpolate (transition) between two values over time. Transition between start and end size (0 to  1)
            float curSize = Mathf.SmoothStep(startSize, endSize, (elapsedTime / time));
            //Rotations work different. It's best to define the rate of rotation, the sine function is handy for this (between 0, 1, back to 0)
            float curRotationRate = Mathf.Sin(Util.mapVal((elapsedTime / time), 0, 1, 0, Mathf.PI));

            //Apply size & rotation
            treeTransform.localScale = new Vector3(curSize, curSize, curSize);
            treeTransform.RotateAround(treeTransform.position, Vector3.up, curRotationRate * turnDirection);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ////////////// after main loop
        if (death)
        {
            Destroy(gameObject);
        } else
        {
            //Activate perch targets once growth complete
            Util.FindInactiveChild(gameObject, "perchParent").SetActive(true);
            growing = false;
            if (dead)
            {
                StartDeath();
            } else
            {
                ///////Add Grass
                int grassCount = (int)Random.Range(20f, 25f);
                //Add Grass Parent
                GameObject grassParent = new GameObject("grassParent");
                grassParentInstance = Instantiate(grassParent, transform.position, Quaternion.identity);
                grassParentInstance.transform.parent = gameObject.transform;
                grassParentInstance.transform.localScale = new Vector3(1, 0, 1);
                //Add Grass Children
                for (int i = 0; i < grassCount; i++)
                {
                    //Define Position & spawn
                    Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
                    Vector3 spawnPosition = new Vector3(transform.position.x + randomOffset.x,
                                                        transform.position.y - .2f,
                                                        transform.position.z + randomOffset.y);
                    GameObject grassInstance = Instantiate(grassPrefab, spawnPosition, Quaternion.identity);
                    //Add random rotation
                    grassInstance.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
                    grassInstance.transform.parent = grassParentInstance.transform;
                    grassInstance.transform.localScale = Vector3.one;
                }
                //Make gras grow vertically
                StartCoroutine(GrassGrowth());
            }
        }
    }

    IEnumerator GrassGrowth()
    {
        float time = 3;
        float startSize = 0;
        float endSize = 1;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            //Lerp is a function to interpolate (transition) between two values over time. Transition between start and end size (0 to  1)
            float curSize = Mathf.SmoothStep(startSize, endSize, (elapsedTime / time));

            //Apply size & rotation
            grassParentInstance.transform.localScale = new Vector3(1, curSize, 1);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
