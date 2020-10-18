using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI label;
    int count = 0;

    public void BtnTest()
    {
        count++;
        label.text = "Click Count: " + count;
    }
}
