using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clippy : MonoBehaviour
{
    public GameObject clippyUiPrefab, uiInstance;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("CenterEyeAnchor");
    }

    public void BtnClippyOpen()
    {
        if (uiInstance == null)
        {
            uiInstance = Instantiate(clippyUiPrefab, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
               player.transform.rotation);
            //position 50cm in front of player
            uiInstance.transform.position += player.transform.forward * .5f;
            uiInstance.GetComponent<ClippyUI>().SetClippy(this);
            Debug.Log("opened Clippy");
            Debug.Log(uiInstance);
        }
    }

    public void ClippyClosed()
    {
        uiInstance = null;
    }
}
