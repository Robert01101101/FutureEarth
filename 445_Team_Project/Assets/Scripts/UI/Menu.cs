using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI label;
    int count = 10;

    public void BtnTest()
    {
        count--;
        label.text = "2020 is over in: " + count;
    }

    public void BtnTest2()
    {
        count++;
        label.text = "2020 is over in: " + count;
    }
}
