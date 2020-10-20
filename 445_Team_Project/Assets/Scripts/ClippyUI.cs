using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// Clippy UI (the canvas floating in front of player). Responsible for:
/// - Follow the Player & check whether thrown away
/// - Main Purpose: Control the UI elements -> respond to buttons, etc.
/// - TODO: Add menu functionality (possibly: different screens, etc.)
/// </summary> 

public class ClippyUI : MonoBehaviour
{
    ///////////////// Public fields
    public TextMeshProUGUI label;

    ///////////////// Private fields
    private OVRGrabbable oVRGrabbable;
    private Clippy clippy;
    private Rigidbody rigidbody;
    int count = 10;

    //Follow player variables. With help from https://docs.unity3d.com/ScriptReference/Vector3.SmoothDamp.html
    private Transform target;
    private Vector3 offset;
    private float smoothTime = 3f;
    private Vector3 velocity = Vector3.zero;
    private bool grabbed = false;
    private bool thrown = false;

    ////////////////////////////////////////////////////////////////////////// Follow Player
    private void Start()
    {
        target = GameObject.Find("CenterEyeAnchor").transform;
        offset = transform.position - target.position;
        oVRGrabbable = GetComponent<OVRGrabbable>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Smoothly move the camera towards that target position
        if (!thrown)
        {
            if (!oVRGrabbable.isGrabbed)
            {
                if (grabbed)
                {
                    //Player just let go -> Check velocity, delete if thrown
                    grabbed = false;
                    if (rigidbody.velocity.magnitude > 1f)
                    {
                        thrown = true;
                        StartCoroutine(ThrowAway());
                    }
                }
                //adjust smoothing depending on whether player is physically moving or using the thumbstick
                smoothTime = Util.mapVal(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).magnitude, 0, 0.5f, 3f, 0.04f, true);
                transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
            }
            else
            {
                if (!grabbed) grabbed = true;
                offset = transform.position - target.position;
            }
        }
    }

    IEnumerator ThrowAway()
    {
        rigidbody.isKinematic = false;
        yield return new WaitForSeconds(1);
        BtnClose();
    }

    ////////////////////////////////////////////////////////////////////////// Public Interface
    public void SetClippy(Clippy input)
    {
        clippy = input;
    }

    //Call by setting onClick() on the Buttons (make sure it's the Prefab)
    ////////////////////////////////////////////////////////////////////////// UI METHODS /////////////////////////////////////////////////////////
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
        clippy.ClippyClosed();
        Destroy(gameObject);
    }
}
