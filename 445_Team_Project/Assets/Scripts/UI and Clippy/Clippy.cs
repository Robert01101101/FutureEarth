using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Clippy Controller Class. Responsible for:
/// - Init wristband UI correctly (facing player)
/// - Control wrist buttons (open / close clippyUI)
/// 
/// Note: Keep in mind that there are TWO clippy instances - one on each arm.
/// </summary>

public class Clippy : MonoBehaviour
{
    ///////////////// Public fields
    public GameObject clippyUiPrefab, canvas, seedPrefab, waterFilterPrefab;
    public VRbtn vrBtn;
    public TextMeshProUGUI btnLabel;

    ///////////////// Private fields
    private GameObject player;
    protected GameObject uiInstance;
    protected ClippyUI uiInstanceClippyUI;
    private OVRGrabber grabberL, grabberR;
    private Color colBtn, colBtnPressed;
    
    private static bool temporaryLock = false;
    private Clippy otherSideClippy;

    

    private void Start()
    {
        player = GameObject.Find("CenterEyeAnchor");
        grabberL = GameObject.Find("LeftHandAnchor").GetComponent<OVRGrabber>();
        grabberR = GameObject.Find("RightHandAnchor").GetComponent<OVRGrabber>();

        //Flip canvas on right hand (to face the right way)
        if (gameObject.name == "clippyR") canvas.transform.rotation = canvas.transform.rotation * Quaternion.Euler(0f, 0f, 180f);

        //start disabled
        temporaryLock = true;
        vrBtn.SetInteractable(false);
    }

    

    public void BtnClippyOpen()
    {
        if (!temporaryLock)
        {
            if (otherSideClippy == null) FindOtherClippy();
            StartCoroutine(TemporaryLock());
            if (!PlayerCtrl.clippyOpen)
            {
                //_________________________________________________ Open ClippyUI
                PlayerCtrl.clippyOpen = true;
                uiInstance = Instantiate(clippyUiPrefab, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
                   player.transform.rotation);
                otherSideClippy.uiInstance = uiInstance;

                //position 55cm in front of player gaze
                uiInstance.transform.position += player.transform.forward * .55f;

                uiInstanceClippyUI = uiInstance.GetComponent<ClippyUI>();
                uiInstanceClippyUI.SetClippy(this);
                if (PlayerCtrl.clippyIntroDone) uiInstanceClippyUI.SkipIntro();

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
        vrBtn.SetInteractable(false);
        otherSideClippy.vrBtn.SetInteractable(false);
        yield return new WaitForSeconds(.5f);
        temporaryLock = false;
        vrBtn.SetInteractable(true);
        otherSideClippy.vrBtn.SetInteractable(true);
    }

    public void ClippyClosed()
    {
        uiInstance = null;
        PlayerCtrl.clippyOpen = false;
        //Reset OVRGrabbers (they can't tell that the Canvas instance was destroyed)
        grabberL.GrabVolumeEnable(false);
        grabberR.GrabVolumeEnable(false);
        grabberL.GrabVolumeEnable(true);
        grabberR.GrabVolumeEnable(true);
        SwapLabels();
    }

    //Swap Labels & Colors of the buttons
    private void SwapLabels()
    {
        if (otherSideClippy == null) FindOtherClippy();
        if (PlayerCtrl.clippyOpen)
        {
            btnLabel.text = "CLOSE";
            vrBtn.SetAltCol(true);

            otherSideClippy.btnLabel.text = "CLOSE";
            otherSideClippy.vrBtn.SetAltCol(true);
        } else
        {
            btnLabel.text = "CLIPPY";
            vrBtn.SetAltCol(false);

            otherSideClippy.btnLabel.text = "CLIPPY";
            otherSideClippy.vrBtn.SetAltCol(false);
        }
    }

    private void FindOtherClippy()
    {
        GameObject avatar = GameObject.Find("LocalAvatar");
        otherSideClippy = (gameObject.name == "clippyL") ? Util.FindInactiveChild(avatar, "clippyR").GetComponent<Clippy>() : Util.FindInactiveChild(avatar, "clippyL").GetComponent<Clippy>();
    }

    
    public void IntroDone()
    {
        //skip intro in the future
        PlayerCtrl.clippyIntroDone = true;
    }

    public void enableClippy()
    {
        temporaryLock = false;
        vrBtn.SetInteractable(true);
        //update color
        //SwapLabels();
    }

    //////////////////////////////////////////////////////////////////////////// Spawn
    public void SpawnSeed()
    {
        GameObject seed = Instantiate(seedPrefab, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
                   Quaternion.identity);

        //position 40cm in front of player gaze
        seed.transform.position += player.transform.forward * .4f;
    }

    public void SpawnWaterFiler()
    {
        GameObject waterFilter = Instantiate(waterFilterPrefab, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
                   Quaternion.identity);

        //position 80cm in front of player gaze * 30cm down
        waterFilter.transform.position += player.transform.forward * .8f + Vector3.down * .3f;
    }
}
