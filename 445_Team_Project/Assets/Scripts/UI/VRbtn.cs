using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RawImage))]

public class VRbtn : MonoBehaviour
{
    Button thisButton;
    Color defaultCol, pressedCol;
    RawImage image;
    BoxCollider collider;
    RectTransform rectTransform;

    private void Start()
    {
        thisButton = GetComponent<Button>();
        image = GetComponent<RawImage>();
        collider = GetComponent<BoxCollider>();
        rectTransform = GetComponent<RectTransform>();

        defaultCol = thisButton.colors.normalColor;
        pressedCol = thisButton.colors.pressedColor;

        //Ensure collider has correct size
        float rw = rectTransform.rect.width;
        float rh = rectTransform.rect.height;
        collider.size = new Vector3(rw, rh, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("VRbtn -> OnTriggerEnter");
        thisButton.onClick.Invoke();
        image.color = pressedCol;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("VRbtn -> OnTriggerExit");
        image.color = defaultCol;
    }

}
