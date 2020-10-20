using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Clippy Controller Class. Responsible for:
/// - Init correctly (facing player)
/// - Control wrist buttons (open / close clippyUI)
/// 
/// Note: Keep in mind that there are TWO clippy instances - one on each arm.
/// </summary>

public class Clippy : MonoBehaviour
{
    ///////////////// Public fields
    public GameObject clippyUiPrefab, canvas;
    public TextMeshProUGUI btnLabel;

    ///////////////// Private fields
    private GameObject player;
    protected GameObject uiInstance;
    private OVRGrabber grabberL, grabberR;
    private static bool clippyOpen = false;
    private static bool temporaryLock = false;
    private Clippy otherSideClippy;

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

                //position 50cm in front of player gaze
                uiInstance.transform.position += player.transform.forward * .5f;

                uiInstance.GetComponent<ClippyUI>().SetClippy(this);
                Debug.Log("opened Clippy");
                Debug.Log(uiInstance);
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
}
