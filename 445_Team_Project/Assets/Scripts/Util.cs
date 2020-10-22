using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// STATIC UTILITY CLASS. Place any static methods here which we might need access to frequently.
/// </summary> 

public class Util : MonoBehaviour
{
    //map s from range a1-a2 to b1-b2
    //Modified, originally from: https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    public static float mapVal(float s, float a1, float a2, float b1, float b2, bool capped = false)
    {
        float output = b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        if (!capped)
        {
            return output;
        } else
        {
            //Option to limit output to target range.
            return ((output < b1 ? b1 : output) > b2 ? b2 : output);
        }
    }

    //From: https://answers.unity.com/questions/890636/find-an-inactive-game-object.html?_ga=2.231808671.1877443542.1603150124-1744466240.1597356417
    //Lets us find inactive GOs as long as they are children of a known GO. Is safer to use than:
    //We could use Resources.FindObjectsOfTypeAll() as an alternative. However, this is very risky, as it can modify prefabs and other resources in the project if not used correctly.
    public static GameObject FindInactiveChild(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
