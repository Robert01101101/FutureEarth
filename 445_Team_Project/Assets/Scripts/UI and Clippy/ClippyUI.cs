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
    public GameObject intro, intro2, intro3, intro4, mainPanel, intro2Picture, intro2Btn, tabMission, tabStats, tabBuild;
    public Button intro3Btn, intro4Btn, waterFilterBtn;
    public TextMeshProUGUI statsValues;

    ////////////////////////////////////////////////////////////////////////// Init
    private void Start()
    {
        target = GameObject.Find("CenterEyeAnchor").transform;
        offset = transform.position - target.position;
        oVRGrabbable = GetComponent<OVRGrabbable>();
        rigidbody = GetComponent<Rigidbody>();

        if (!PlayerCtrl.clippyIntroDone) PlayerCtrl.playerCtrl.PlayClippyAudio(6);
    }

    public void SkipIntro()
    {
        intro.SetActive(false);
        mainPanel.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////////// Follow Player
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

        //intro: wait for player to grip screen before proceeding
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


    ////////////////////////////////////////////////////////////////////////// Coroutines
    IEnumerator ThrowAway()
    {
        rigidbody.isKinematic = false;
        yield return new WaitForSeconds(1);
        BtnClose();
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

    IEnumerator DelayedAutoClose()
    {
        yield return new WaitForSeconds(1);
        BtnClose();
    }

    ////////////////////////////////////////////////////////////////////////// Public Interface
    public void SetClippy(Clippy input)
    {
        clippy = input;
    }


    ////////////////////////////////////////////////////////////////////////// UI METHODS /////////////////////////////////////////////////////////
    //Call by setting onClick() on the Buttons (make sure you are editing the ClippyUI prefab)
    public void BtnClose()
    {
        clippy.ClippyClosed();
        Destroy(gameObject);
    }

    public void BtnTabMission()
    {
        tabMission.SetActive(true);
        tabStats.SetActive(false);
        tabBuild.SetActive(false);
    }

    public void BtnTabStats()
    {
        tabMission.SetActive(false);
        tabStats.SetActive(true);
        tabBuild.SetActive(false);

        statsValues.text = GameCtrl.GetPartCount() + "\n<size=50%>(enough for " + (int) (GameCtrl.GetPartCount()/5) + " water filters)</size>\n\n" + 
            GameCtrl.GetWaterFilterCount() + "\n<size=50%>(enough for " + GameCtrl.GetWaterFilterCount()*10 + " trees)</size>\n\n" + 
            GameCtrl.GetTreeCount() + "\n<size=50%>(plant 100)</size>";
    }

    public void BtnTabBuild()
    {
        tabMission.SetActive(false);
        tabStats.SetActive(false);
        tabBuild.SetActive(true);
        if (GameCtrl.GetPartCount() >= 5)
        {
            waterFilterBtn.interactable = true;
        } else
        {
            waterFilterBtn.interactable = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////// INTRO /////////////////////////////////////////////////////////
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
        mainPanel.SetActive(true);
        clippy.IntroDone();
    }

    ////////////////////////////////////////////////////////////////////////// TAB - SPAWN /////////////////////////////////////////////////////////
    public void BtnSpawnSeed()
    {
        clippy.SpawnSeed();
        StartCoroutine(DelayedAutoClose());
    }

    public void BtnSpawnWaterFilter()
    {
        clippy.SpawnWaterFiler();
        StartCoroutine(DelayedAutoClose());
        GameCtrl.RemovePartsFromList();
        if (GameCtrl.GetPartCount() >= 5)
        {
            waterFilterBtn.interactable = true;
        }
        else
        {
            waterFilterBtn.interactable = false;
        }
    }

}
