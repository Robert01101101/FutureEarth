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

    //Private fields
    Transform treeTransform;
    int turnDirection;
    CapsuleCollider treeCollider;

    bool dead = false;
    bool growing = false;
    

    //A3 (was originally in start with always random pick)
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
        StartCoroutine(TreeGrowth(5));
    }


    //Tree Growth Coroutine
    IEnumerator TreeGrowth(float time)
    {
        growing = true;
        float startSize = 0;
        float endSize = 1;
        float elapsedTime = 0;

        //Activate collider
        treeCollider.enabled = true;

        while (elapsedTime < time)
        {   
            //Lerp is a function to interpolate (transition) between two values over time. Transition between start and end size (0 to  1)
            float curSize = Mathf.SmoothStep(startSize, endSize, (elapsedTime / time));
            //Rotations work different. It's best to define the rate of rotation, the sine function is handz for this (between 0, 1, back to 0)
            float curRotationRate = Mathf.Sin(Util.mapVal((elapsedTime / time), 0, 1, 0, Mathf.PI));

            //Apply size & rotation
            treeTransform.localScale = new Vector3(curSize, curSize, curSize);
            treeTransform.RotateAround(treeTransform.position, Vector3.up, curRotationRate * turnDirection);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Activate perch targets once growth complete
        Util.FindInactiveChild(gameObject, "perchParent").SetActive(true);
        growing = false;
        if (dead) StartDeath();
    }

    ///////////////////////////////////////////////////////// PUBLIC INTERFACE /////////////////////////////////////////
    public void StartDeath()
    {
        if (!growing) StartCoroutine(TreeDeath());
        dead = true;
    }

    IEnumerator TreeDeath()
    {
        yield return new WaitForSeconds(2);

        //tmp
        treeTransform.localScale *= .5f;

        yield return new WaitForSeconds(2);

        Destroy(gameObject);
    }


}
