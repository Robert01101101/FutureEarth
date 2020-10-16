using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    Transform treeTransform;
    int turnDirection;

    void Start()
    {
        //assign transform variable & set size to 0
        treeTransform = gameObject.transform;
        gameObject.transform.localScale = Vector3.zero;

        //Coinflip: turn either left or right
        turnDirection = (Random.value < 0.5) ? -1 : 1;

        //Start growth
        StartCoroutine(TreeCoroutine(5));
    }


    //map s from range a1-a2 to b1-b2
    float mapVal(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }


    //This is a Coroutine. Coroutine are asynchronour processes - use whenever we need a timer or do something based on time
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
            float curRotationRate = Mathf.Sin(mapVal((elapsedTime / time), 0, 1, 0, Mathf.PI));

            //Apply size & rotation
            treeTransform.localScale = new Vector3(curSize, curSize, curSize);
            treeTransform.RotateAround(treeTransform.position, Vector3.up, curRotationRate * turnDirection);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
