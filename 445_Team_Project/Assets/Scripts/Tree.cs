using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To prevent setting up the GameObject wrong. It's still reccommended to attach and configure the other components, but this prevents fatal errors if forgotten.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]

public class Tree : MonoBehaviour
{
    //Public fields
    public Mesh[] treeMeshes;

    //Private fields
    Transform treeTransform;
    int turnDirection;
    MeshFilter meshFilter;
    


    void Start()
    {
        //assign transform variable & set size to 0
        treeTransform = gameObject.transform;
        gameObject.transform.localScale = Vector3.zero;

        //Randomize: turn either left or right & select random mesh
        turnDirection = (Random.value < 0.5) ? -1 : 1;

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = treeMeshes[Random.Range(0,treeMeshes.Length)];

        //Start growth
        StartCoroutine(TreeCoroutine(5));
    }


    


    //This is a Coroutine. Coroutines are asynchronous processes - use whenever we need a timer or do something based on time
    IEnumerator TreeCoroutine(float time)
    {
        float startSize = 0;
        float endSize = 1;
        float elapsedTime = 0;

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
    }
}
