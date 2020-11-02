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
    public Button button;
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
        button.interactable = false;
    }

    public void BtnClippyOpen()
    {
        if (!temporaryLock)
        {
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
                if (PlayerCtrl.clippyIntroDone) uiInstanceClippyUI.Init(PlayerCtrl.treeCount);

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
        button.interactable = false;
        otherSideClippy.button.interactable = false;
        yield return new WaitForSeconds(.5f);
        temporaryLock = false;
        button.interactable = true;
        otherSideClippy.button.interactable = true;
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
            vrBtn.SetColor(VRbtn.NORMALCOLOR, new Color32(0xFF, 0xC9, 0xC9, 0xff));
            vrBtn.SetColor(VRbtn.PRESSEDCOLOR, new Color32(0xFF, 0xFF, 0xFF, 0xff));

            otherSideClippy.btnLabel.text = "CLOSE";
            otherSideClippy.vrBtn.SetColor(VRbtn.NORMALCOLOR, new Color32(0xFF, 0xC9, 0xC9, 0xff));
            otherSideClippy.vrBtn.SetColor(VRbtn.PRESSEDCOLOR, new Color32(0xFF, 0xFF, 0xFF, 0xff));
        } else
        {
            btnLabel.text = "CLIPPY";
            vrBtn.ResetColors();

            otherSideClippy.btnLabel.text = "CLIPPY";
            otherSideClippy.vrBtn.ResetColors();
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
        button.interactable = true;
        //update color
        SwapLabels();
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

        //position 40cm in front of player gaze
        waterFilter.transform.position += player.transform.forward * .4f;
    }
}
