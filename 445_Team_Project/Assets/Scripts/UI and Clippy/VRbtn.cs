using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// In order to track where the player is pointing we need to use colliders.
/// Instead  of relying on Standalone Input we manually trigger the button on collision with finger.
/// This class is attached to every tappable button, and it handles:
/// -  Initialize the Button correctly (resize collider, save color pallette)
/// -  Manually trigger onClick() when detecting a collision.
/// 
/// Check the Button Component's onClick properties to check what methods it calls.
/// </summary> 

public class VRbtn : MonoBehaviour
{
    Button thisButton;
    Color defaultCol, pressedCol;
    RawImage image;
    BoxCollider collider;
    RectTransform rectTransform;
    Rigidbody rigidbody;
    private bool pressed = false;

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
        collider.center = new Vector3(0, 0, 0.01f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "hands:b_r_index_ignore" || other.gameObject.name == "hands:b_l_index_ignore")
        {
            if (thisButton.interactable)
            {
                pressed = true;
                Debug.Log("VRbtn -> pressed");
                thisButton.onClick.Invoke();
                image.color = thisButton.colors.pressedColor;
                PlayerCtrl playerCtrl = GameObject.Find("OVRPlayerControllerCustom").GetComponent<PlayerCtrl>();
                playerCtrl.PlayAudioBtn();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "hands:b_r_index_ignore" || other.gameObject.name == "hands:b_l_index_ignore")
        {
            pressed = false;
            Debug.Log("VRbtn -> released");
            image.color = thisButton.colors.normalColor;
        }
    }

    ////////////////////////////////////////////////////////////////////////// Public Interface /////////////////////////////////////////////////////////
    public static int NORMALCOLOR = 0;
    public static int PRESSEDCOLOR = 1;
    public void SetColor(int type, Color col)
    {
        if (type == NORMALCOLOR)
        {
            ColorBlock newColors = thisButton.colors;
            newColors.normalColor = col;
            thisButton.colors = newColors;

        } else
        {
            ColorBlock newColors = thisButton.colors;
            newColors.pressedColor = col;
            thisButton.colors = newColors;
        }

        //update color
        image.color = pressed ? thisButton.colors.pressedColor : thisButton.colors.normalColor;
    }

    public void ResetColors()
    {
        ColorBlock newColors = thisButton.colors;
        newColors.normalColor = defaultCol;
        newColors.pressedColor = pressedCol;
        thisButton.colors = newColors;

        //update color
        image.color = pressed ? thisButton.colors.pressedColor : thisButton.colors.normalColor;
    }



}
