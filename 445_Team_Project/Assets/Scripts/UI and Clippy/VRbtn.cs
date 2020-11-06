using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Button))]
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
    public Color defaultCol, pressedCol, disabledCol, altCol;
    RawImage image;
    BoxCollider collider;
    RectTransform rectTransform;
    Rigidbody rigidbody;
    private bool pressed = false;
    private bool altColOn = false;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        for (int i=0; i<transform.childCount; i++)
        {
            Transform curChild = transform.GetChild(i);
            if (curChild.gameObject.name == "Front") image = curChild.gameObject.GetComponent<RawImage>();
        }
        collider = GetComponent<BoxCollider>();
        rectTransform = GetComponent<RectTransform>();
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

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
                image.color = pressedCol;
                PlayerCtrl playerCtrl = GameObject.Find("OVRPlayerControllerCustom").GetComponent<PlayerCtrl>();
                playerCtrl.PlayAudioBtn();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "hands:b_r_index_ignore" || other.gameObject.name == "hands:b_l_index_ignore")
        {
            if (thisButton.interactable)
            {
                pressed = false;
                Debug.Log("VRbtn -> released");
                image.color = altColOn ? altCol : defaultCol;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////// Public Interface /////////////////////////////////////////////////////////
    public void SetInteractable(bool newState)
    {
        thisButton.interactable = newState;
        if (newState)
        {
            image.color = defaultCol;
        }
        else
        {
            image.color = disabledCol;
        }
    }

    public void SetAltCol(bool usingAlt)
    {
        image.color = usingAlt ? altCol : defaultCol;
        altColOn = usingAlt;
    }


    //prevent click if finger is inside button on start
    private void OnEnable()
    {
        StartCoroutine(TmpLock());
    }

    IEnumerator TmpLock()
    {
        collider.enabled = false;
        yield return new WaitForSeconds(.5f);
        collider.enabled = true;
    }
}
