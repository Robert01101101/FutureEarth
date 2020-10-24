using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for all logic related to the player. 
/// Uses singleton pattern - all properties & methods are accessibly through: PlayerCtrl.playerCtrl or for static methods: PlayerCtrl.IncreaseTreeCount() (example)
/// Current responsibilities:
/// - Track game variables such as amount of trees grown (move to GameCtrl)
/// - Control non-spatial audio
/// - Control clippy priming sequence (adio & things like enabling interactions at the right time)
/// </summary> 

public class PlayerCtrl : MonoBehaviour
{
    //OVR
    OVRPlayerController ovrPlayerController;

    //Audio
    public AudioSource buttonSound;
    public AudioSource[] clippyAudio;

    //Singleton player instance
    public static PlayerCtrl playerCtrl;

    //A3 - spawn UI on correct screen (skip intro once done)
    [HideInInspector] public static bool clippyIntroDone = false;
    [HideInInspector] public static int treeCount = 0;
    [HideInInspector] public static bool clippyOpen = false;


    //Singleton pattern - only one instance that is accessible from anywhere though PlayerCtrl.playerCtrl
    //from: https://riptutorial.com/unity3d/example/14518/a-simple-singleton-monobehaviour-in-unity-csharp
    void Awake()
    {
        if (playerCtrl == null)
        {
            playerCtrl = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this); }

        ovrPlayerController = GetComponent<OVRPlayerController>();
        StartCoroutine(ClippyPrimingSequence());
    }

    public void PlayAudioBtn()
    {
        buttonSound.Stop();
        buttonSound.Play();
    }

    public static void IncreaseTreeCount()
    {
        treeCount++;
    }

    IEnumerator ClippyPrimingSequence()
    {
        ovrPlayerController.SetHaltUpdateMovement(true);
        yield return new WaitForSeconds(4);
        clippyAudio[0].Play();
        yield return new WaitForSeconds(7);
        clippyAudio[1].Play();
        yield return new WaitForSeconds(8);
        clippyAudio[2].Play();
        yield return new WaitForSeconds(3);

        //has landed
        clippyAudio[3].Play();
        ovrPlayerController.SetHaltUpdateMovement(false);
        GameObject.Find("Hatch").GetComponent<Hatch>().SetReady();
        yield return new WaitForSeconds(26);

        //use clippy
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyR").GetComponent<Clippy>().enableClippy();
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyL").GetComponent<Clippy>().enableClippy();
        clippyAudio[4].Play();

        yield return new WaitForSeconds(9);
        clippyAudio[5].Play();
    }

    
    public void PlayClippyAudio(int clip)
    {
        if (clippyAudio[clip - 1].isPlaying) clippyAudio[clip-1].Stop();
        clippyAudio[clip].Play();
    }
}
