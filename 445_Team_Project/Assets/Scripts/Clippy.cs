using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Clippy Controller Class. Responsible for:
/// - Init wristband UI correctly (facing player)
/// - Control wrist buttons (open / close clippyUI)
/// 
/// Note: Keep in mind that there are TWO clippy instances - one on each arm.
/// Problem: two instances store static values.
/// TODO - find a better approach. Ideas: singleton instance? Move methods to ClippyUI?
/// </summary>

public class Clippy : MonoBehaviour
{
    ///////////////// Public fields
    public GameObject clippyUiPrefab, canvas, seedPrefab;
    public TextMeshProUGUI btnLabel;

    ///////////////// Private fields
    private GameObject player;
    protected GameObject uiInstance;
    protected ClippyUI uiInstanceClippyUI;
    private OVRGrabber grabberL, grabberR;
    private static bool clippyOpen = false;
    private static bool temporaryLock = false;
    private Clippy otherSideClippy;

    //A3 - spawn UI on correct screen (skip intro once done)
    private static bool introDone = false;
    private static int treeCount = 0;

    private void Start()
    {
        player = GameObject.Find("CenterEyeAnchor");
        grabberL = GameObject.Find("LeftHandAnchor").GetComponent<OVRGrabber>();
        grabberR = GameObject.Find("RightHandAnchor").GetComponent<OVRGrabber>();

        //Flip canvas on right hand (to face the right way)
        if (gameObject.name == "clippyR") canvas.transform.rotation = canvas.transform.rotation * Quaternion.Euler(0f, 0f, 180f);
    }

    public void BtnClippyOpen()
    {
        if (!temporaryLock)
        {
            StartCoroutine(TemporaryLock());
            if (!clippyOpen)
            {
                //_________________________________________________ Open ClippyUI
                if (otherSideClippy == null) FindOtherClippy();

                clippyOpen = true;
                uiInstance = Instantiate(clippyUiPrefab, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
                   player.transform.rotation);
                otherSideClippy.uiInstance = uiInstance;

                //position 55cm in front of player gaze
                uiInstance.transform.position += player.transform.forward * .55f;

                uiInstanceClippyUI = uiInstance.GetComponent<ClippyUI>();
                uiInstanceClippyUI.SetClippy(this);
                if (introDone) uiInstanceClippyUI.Init(treeCount);

                Debug.Log("opened Clippy");
                SwapLabels();
            }
            else
            {
                //_________________________________________________ Close ClippyUI
                uiInstance.GetComponent<ClippyUI>().BtnClose();
            }
        }
    }

    //Prevent opening & immediately closing again by accident, by inducing half a second delay
    IEnumerator TemporaryLock()
    {
        temporaryLock = true;
        yield return new WaitForSeconds(.5f);
        temporaryLock = false;
    }

    public void ClippyClosed()
    {
        uiInstance = null;
        clippyOpen = false;
        //Reset OVRGrabbers (they can't tell that the Canvas instance was destroyed)
        grabberL.GrabVolumeEnable(false);
        grabberR.GrabVolumeEnable(false);
        grabberL.GrabVolumeEnable(true);
        grabberR.GrabVolumeEnable(true);
        SwapLabels();
    }

    private void SwapLabels()
    {
        if (clippyOpen)
        {
            btnLabel.text = "CLOSE";
            otherSideClippy.btnLabel.text = "CLOSE";
        } else
        {
            btnLabel.text = "CLIPPY";
            otherSideClippy.btnLabel.text = "CLIPPY";
        }
    }

    private void FindOtherClippy()
    {
        GameObject avatar = GameObject.Find("LocalAvatar");
        otherSideClippy = (gameObject.name == "clippyL") ? Util.FindInactiveChild(avatar, "clippyR").GetComponent<Clippy>() : Util.FindInactiveChild(avatar, "clippyL").GetComponent<Clippy>();
    }

    //////////////////////////////////////////////////////////////////////////// A3
    public void IntroDone()
    {
        //skip intro in the future
        introDone = true;
        if (!GetIntroDone()) otherSideClippy.IntroDone();
    }

    public bool GetIntroDone() { return introDone; }

    public void SpawnSeed(int type)
    {
        GameObject seed = Instantiate(seedPrefab, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
                   Quaternion.identity);
        seed.GetComponent<Seed>().SetType(type);

        //position 55cm in front of player gaze
        seed.transform.position += player.transform.forward * .4f;
    }

    public static void IncreaseTreeCount()
    {
        treeCount++;
    }
}
