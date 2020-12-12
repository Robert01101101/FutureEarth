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

    /////////////////
    private bool waitingForGrip = false;
    public GameObject intro, intro2, intro3, intro4, introPanel, mainPanel, intro2Picture, tabMission, tabInventory, tabStats, tabBuild, imgChip, imgPump1, imgPump2, imgTube1, imgTube2, imgTube3;
    public VRbtn intro2Btn, intro3Btn, intro4Btn, waterFilterBtn, tabMissionBtn, tabInventoryBtn, tabStatsBtn, tabBuildBtn;

    public TextMeshProUGUI statsValues, inventoryValues, timeText;
    private string timeString = "Date:\n2720 | 03 | 23\n\nTime:\n";

    ////////////////////////////////////////////////////////////////////////// Init
    private void Start()
    {
        target = GameObject.Find("CenterEyeAnchor").transform;
        offset = transform.position - target.position;
        oVRGrabbable = GetComponent<OVRGrabbable>();
        rigidbody = GetComponent<Rigidbody>();

        if (!PlayerCtrl.clippyIntroDone)
        {
            PlayerCtrl.playerCtrl.PlayClippyAudio(7);
            introPanel.SetActive(true);
            mainPanel.SetActive(false);

            intro.SetActive(true);
            intro2.SetActive(false);
            intro3.SetActive(false);
            intro4.SetActive(false);
        } else
        {
            introPanel.SetActive(false);
            mainPanel.SetActive(true);

            BtnTabMission();
        }
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
                intro2Btn.gameObject.SetActive(true);
                intro2Btn.SetInteractable(true);
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
                intro3Btn.SetInteractable(true);
                break;
            case 4:
                intro4Btn.SetInteractable(true);
                break;
            default:
                intro3Btn.SetInteractable(true);
                intro4Btn.SetInteractable(true);
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
        tabInventory.SetActive(false);
        tabStats.SetActive(false);
        tabBuild.SetActive(false);
        tabMissionBtn.SetAltCol(false);
        tabInventoryBtn.SetAltCol(true);
        tabStatsBtn.SetAltCol(true);
        tabBuildBtn.SetAltCol(true);

        timeText.text = timeString + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
    }

    public void BtnTabInventory()
    {
        tabMission.SetActive(false);
        tabInventory.SetActive(true);
        tabStats.SetActive(false);
        tabBuild.SetActive(false);
        tabMissionBtn.SetAltCol(true);
        tabInventoryBtn.SetAltCol(false);
        tabStatsBtn.SetAltCol(true);
        tabBuildBtn.SetAltCol(true);

        int chips = GameCtrl.GetPartCount(PartType.chip);
        int pumps = GameCtrl.GetPartCount(PartType.pump);
        int tubes = GameCtrl.GetPartCount(PartType.tube);

        inventoryValues.text = chips + "\n<size=50%> \n </size>\n" +
            tubes + "\n<size=50%> \n </size>\n" +
            pumps;

        imgChip.SetActive(chips >= 1);
        imgPump1.SetActive(pumps >= 1);
        imgPump2.SetActive(pumps >= 2);
        imgTube1.SetActive(tubes >= 1);
        imgTube2.SetActive(tubes >= 2);
        imgTube3.SetActive(tubes >= 3);
    }

    public void BtnTabStats()
    {
        tabMission.SetActive(false);
        tabInventory.SetActive(false);
        tabStats.SetActive(true);
        tabBuild.SetActive(false);
        tabMissionBtn.SetAltCol(true);
        tabInventoryBtn.SetAltCol(true);
        tabStatsBtn.SetAltCol(false);
        tabBuildBtn.SetAltCol(true);

        statsValues.text = GameCtrl.GetWaterFilterCount() + "\n <size=50%> \n </size>\n" + 
            GameCtrl.GetTreeCount() + "\n<size=50%> </size>";
    }

    public void BtnTabBuild()
    {
        tabMission.SetActive(false);
        tabInventory.SetActive(false);
        tabStats.SetActive(false);
        tabBuild.SetActive(true);
        tabMissionBtn.SetAltCol(true);
        tabInventoryBtn.SetAltCol(true);
        tabStatsBtn.SetAltCol(true);
        tabBuildBtn.SetAltCol(false);
        StartCoroutine(WaterFilterBtnDelay());
    }

    IEnumerator WaterFilterBtnDelay()
    {
        yield return new WaitForEndOfFrame();
        if (GameCtrl.CheckIfEnoughParts())
        {
            waterFilterBtn.SetInteractable(true);
        }
        else
        {
            waterFilterBtn.SetInteractable(false);
        }
    }

    ////////////////////////////////////////////////////////////////////////// INTRO /////////////////////////////////////////////////////////
    public void BtnIntro()
    {
        intro.SetActive(false);
        intro2.SetActive(true);
        waitingForGrip = true;
        PlayerCtrl.playerCtrl.PlayClippyAudio(8);
    }

    public void BtnIntro2()
    {
        intro2.SetActive(false);
        intro3.SetActive(true);
        StartCoroutine(UnlockBtn(3));
        PlayerCtrl.playerCtrl.PlayClippyAudio(9);
    }
   
    public void BtnIntro3()
    {
        intro3.SetActive(false);
        intro4.SetActive(true);
        StartCoroutine(UnlockBtn(4));
        PlayerCtrl.playerCtrl.PlayClippyAudio(10);
    }  

    public void BtnIntro4()
    {
        intro4.SetActive(false);
        introPanel.SetActive(false);
        mainPanel.SetActive(true);
        clippy.IntroDone();
        BtnTabBuild();
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
        if (GameCtrl.CheckIfEnoughParts())
        {
            waterFilterBtn.SetInteractable(true);
        }
        else
        {
            waterFilterBtn.SetInteractable(false);
        }
    }

}
