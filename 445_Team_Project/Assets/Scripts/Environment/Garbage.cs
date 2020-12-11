using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    public float zeroPoint;
    private bool shrinking = false;
    private float shrinkRate = 0.003f;
    private List<EnemyController> bots = new List<EnemyController>();

    public void AddBot(EnemyController bot)
    {
        if (!bots.Contains(bot)) bots.Add(bot);
        UpdateShrinkingSpeed();
    }

    public void RemoveBot(EnemyController bot)
    {
        if (bots.Contains(bot)) bots.Remove(bot);
        UpdateShrinkingSpeed();
    }

    private void UpdateShrinkingSpeed()
    {
        shrinking = bots.Count > 0;
        if (shrinking)
        {
            if (!coroutineRunning) StartCoroutine(Shrinkage());
        } else
        {
            coroutineRunning = false;
            StopAllCoroutines();
        }
    }

    bool coroutineRunning = false;
    IEnumerator Shrinkage()
    {
        coroutineRunning = true;
        while (shrinking)
        {
            if (transform.position.y > zeroPoint)
            {
                //Lerp is a function to interpolate (transition) between two values over time. Transition between start and end size (0 to  1)
                gameObject.transform.position = gameObject.transform.position - new Vector3(0, shrinkRate * bots.Count, 0);
                yield return new WaitForEndOfFrame();
            } else
            {
                TerminatePile();
                shrinking = false;
            }
        }
        coroutineRunning = false;
    }

    private void TerminatePile()
    {
        Debug.Log("Pile done");
        GameCtrl.RemovePileFromList(gameObject);
        foreach(EnemyController bot in bots)
        {
            bot.FindNewPile();
        }
    }
}
