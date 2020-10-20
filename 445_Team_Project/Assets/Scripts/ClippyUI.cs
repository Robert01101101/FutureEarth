using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClippyUI : MonoBehaviour
{
    public TextMeshProUGUI label;
    private Clippy clippy;
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

    public void BtnClose()
    {
        Destroy(gameObject);
        clippy.ClippyClosed();
    }

    public void SetClippy(Clippy input)
    {
        clippy = input;
    }
}
