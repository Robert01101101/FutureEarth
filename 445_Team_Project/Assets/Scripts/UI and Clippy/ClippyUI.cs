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
/// - TODO: Find a way to store state.
/// </summary> 

public class ClippyUI : MonoBehaviour
{
    ///////////////// Private fields
    private OVRGrabbable oVRGrabbable;
    private Clippy clippy;
    private Rigidbody rigidbody;

    ///////////////// UI variables
    private int count = 10;

    //Follow player variables. With help from https://docs.unity3d.com/ScriptReference/Vector3.SmoothDamp.html
    private Transform target;
    private Vector3 offset;
    private float smoothTime = 3f;
    private Vector3 velocity = Vector3.zero;
    private bool grabbed = false;
    private bool thrown = false;

    ///////////////// A3 specific (sequence intro screens & spawn seeds)
    private bool waitingForGrip = false;
    public GameObject intro, intro2, intro3, intro4, spawnSeed, intro2Picture, intro2Btn;
    public Button intro3Btn, intro4Btn;
    public TextMeshProUGUI treeCountLabel;

    ////////////////////////////////////////////////////////////////////////// Follow Player
    private void Start()
    {
        target = GameObject.Find("CenterEyeAnchor").transform;
        offset = transform.position - target.position;
        oVRGrabbable = GetComponent<OVRGrabbable>();
        rigidbody = GetComponent<Rigidbody>();

        if (!PlayerCtrl.clippyIntroDone) PlayerCtrl.playerCtrl.PlayClippyAudio(6);
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

        //TMP for A3
        if (waitingForGrip)
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0 || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0)
            {
                waitingForGrip = false;
                intro2Picture.SetActive(false);
                intro2Btn.SetActive(true);
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

    //Call by setting onClick() on the Buttons (make sure you are editing the ClippyUI prefab)
    ////////////////////////////////////////////////////////////////////////// UI METHODS /////////////////////////////////////////////////////////
    public void BtnTest()
    {
        count--;
        //label.text = "2020 is over in: " + count;
    }

    public void BtnTest2()
    {
        count++;
        //label.text = "2020 is over in: " + count;
    }

    public void BtnClose()
    {
        clippy.ClippyClosed();
        Destroy(gameObject);
    }

    ////////////////////////////////////////////////////////////////////////// A3 Specific /////////////////////////////////////////////////////////
    public void BtnA3Intro()
    {
        intro.SetActive(false);
        intro2.SetActive(true);
        waitingForGrip = true;
        PlayerCtrl.playerCtrl.PlayClippyAudio(7);
    }

    public void BtnA3Intro2()
    {
        intro2.SetActive(false);
        intro3.SetActive(true);
        StartCoroutine(UnlockBtn(3));
        PlayerCtrl.playerCtrl.PlayClippyAudio(8);
    }
   
    public void BtnA3Intro3()
    {
        intro3.SetActive(false);
        intro4.SetActive(true);
        StartCoroutine(UnlockBtn(4));
        PlayerCtrl.playerCtrl.PlayClippyAudio(9);
    }  

    public void BtnA3Intro4()
    {
        intro4.SetActive(false);
        spawnSeed.SetActive(true);
        clippy.IntroDone();
    }

    IEnumerator UnlockBtn(int btnNum)
    {
        yield return new WaitForSeconds(2);
        switch (btnNum)
        {
            case 3:
                intro3Btn.interactable = true;
                break;
            case 4:
                intro4Btn.interactable = true;
                break;
            default:
                intro3Btn.interactable = true;
                intro4Btn.interactable = true;
                break;
        }
    }

    //Seed btns

    public void BtnA3Seed1()
    {
        clippy.SpawnSeed(0);
        StartCoroutine(DelayedAutoClose());
    }

    public void BtnA3Seed2()
    {
        clippy.SpawnSeed(1);
        StartCoroutine(DelayedAutoClose());
    }

    public void BtnA3Seed3()
    {
        clippy.SpawnSeed(2);
        StartCoroutine(DelayedAutoClose());
    }

    public void BtnA3SeedRandom()
    {
        clippy.SpawnSeed(3);
        StartCoroutine(DelayedAutoClose());
    }

    IEnumerator DelayedAutoClose()
    {
        yield return new WaitForSeconds(1);
        BtnClose();
    }

    public void Init(int treeCount)
    {
        intro.SetActive(false);
        spawnSeed.SetActive(true);
        treeCountLabel.text = "Trees spawned: " + treeCount;
    }
}
