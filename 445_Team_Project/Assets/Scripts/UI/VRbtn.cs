using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(Rigidbody))]

//In order to track where the player is pointing we need to use colliders.
//Instead  of relying on Standalone Input we manually trigger the button on collision with finger.
//This class ensures the Button is initialized correctly & then triggers onClick() when detecting a collision.
//Check the Button Component's onClick properties to check what methods it calls.

public class VRbtn : MonoBehaviour
{
    Button thisButton;
    Color defaultCol, pressedCol;
    RawImage image;
    BoxCollider collider;
    RectTransform rectTransform;
    Rigidbody rigidbody;

    private void Start()
    {
        thisButton = GetComponent<Button>();
        image = GetComponent<RawImage>();
        collider = GetComponent<BoxCollider>();
        rectTransform = GetComponent<RectTransform>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        defaultCol = thisButton.colors.normalColor;
        pressedCol = thisButton.colors.pressedColor;

        //Ensure collider has correct size
        float rw = rectTransform.rect.width;
        float rh = rectTransform.rect.height;
        collider.size = new Vector3(rw, rh, 0.02f);
        collider.center = new Vector3(0,0, 0.01f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "hands:b_r_index_ignore" || other.gameObject.name == "hands:b_l_index_ignore")
        {
            Debug.Log("VRbtn -> pressed");
            thisButton.onClick.Invoke();
            image.color = pressedCol;
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "hands:b_r_index_ignore" || other.gameObject.name == "hands:b_l_index_ignore")
        {
            Debug.Log("VRbtn -> released");
            image.color = defaultCol;
        }
    }

}
