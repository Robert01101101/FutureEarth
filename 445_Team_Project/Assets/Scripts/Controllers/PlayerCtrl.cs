using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sigtrap.VrTunnellingPro;

/// <summary>
/// Responsible for all logic related to the player. 
/// Uses singleton pattern - all properties & methods are accessibly through: PlayerCtrl.playerCtrl or for static methods: PlayerCtrl.IncreaseTreeCount() (example)
/// 
/// Features:
/// - Control non-spatial audio
/// - Control clippy priming sequence (adio & things like enabling interactions at the right time)
/// - TODO: Player Vision (hit VFX, vignette)
/// </summary> 

public class PlayerCtrl : MonoBehaviour
{
    public bool skipIntro;
    public Tunnelling vignette;
    public TunnellingPreset vignettePresetDefault, vignettePresetRed;

    //OVR
    OVRPlayerController ovrPlayerController;

    //Audio
    public AudioSource buttonSound, laser, heartBeat, pickupSound;
    public AudioSource[] clippyAudio;

    //Singleton player instance
    public static PlayerCtrl playerCtrl;
    [HideInInspector] public static bool clippyIntroDone = false;
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
        if (!skipIntro) { StartCoroutine(ClippyPrimingSequence()); } else { SkipIntro(); }
    }

    /////////////////////////////////////////////////////////////////////// PLAYER HIT VFX /////////////////////////////////////////////
    //Turn vision red on hit
    //TODO: Fix - works in editor but not in build
    public void PlayerHitVignette()
    {
        StartCoroutine(PlayerHitVignetteRoutine());
    }

    IEnumerator PlayerHitVignetteRoutine()
    {
        heartBeat.Stop();
        heartBeat.Play();

        //Fade In
        vignette.SetForceMode(true, 0f);
        vignette.ApplyPreset(vignettePresetRed);
        float time = .5f;
        float startVal = 0;
        float endVal = 0.7f;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            float curVal = Mathf.SmoothStep(startVal, endVal, (elapsedTime / time));
            vignette.SetForceMode(true, curVal);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Keep for 1 sec
        yield return new WaitForSeconds(1f);

        //Fade Out
        time = 4f;
        startVal = 0.7f;
        endVal = 0f;
        elapsedTime = 0;
        while (elapsedTime < time)
        {
            float curVal = Mathf.SmoothStep(startVal, endVal, (elapsedTime / time));
            vignette.SetForceMode(true, curVal);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        vignette.SetForceMode(false, 0f);
        vignette.ApplyPreset(vignettePresetDefault);
    }

    /////////////////////////////////////////////////////////////////////// GENERAL AUDIO /////////////////////////////////////////////
    private int parts = 0;
    public void PlayPickupSound()
    {
        pickupSound.Stop();
        pickupSound.Play();

        parts++;
        if (parts == 1)
        {
            clippyAudio[13].Play();
        }
        else if (parts == 5)
        {
            clippyAudio[14].Play();
            StartCoroutine(PlantTreesAudio());
        }
    }

    public void PlayAudioBtn()
    {
        buttonSound.Stop();
        buttonSound.Play();
    }



    /////////////////////////////////////////////////////////////////////// CLIPPY AUDIO / NARRATIVE HARDCODING ///////////////////////
    public void PlayClippyAudio(int clip)
    {
        if (clippyAudio[clip - 1].isPlaying) clippyAudio[clip - 1].Stop();
        clippyAudio[clip].Play();

        //approach enemy timer
        if (clip == 11) StartCoroutine(ApproachEnemyAudio());
        if (clip == 16) StartCoroutine(MoreTreesAudio());
    } 

    //////////// Clippy Audio narration up until beginning the mission brief
    IEnumerator ClippyPrimingSequence()
    {
        //Approaching earth
        ovrPlayerController.SetHaltUpdateMovement(true);
        yield return new WaitForSeconds(3);
        clippyAudio[0].Play();
        yield return new WaitForSeconds(8);
        clippyAudio[1].Play();
        yield return new WaitForSeconds(8.5f);
        clippyAudio[2].Play();
        yield return new WaitForSeconds(3.5f);

        //has landed
        clippyAudio[3].Play();
        ovrPlayerController.SetHaltUpdateMovement(false);
        GameObject.Find("Hatch").GetComponent<Hatch>().SetReady();
        yield return new WaitForSeconds(26);

        
        //first step
        clippyAudio[4].Play();
        yield return new WaitForSeconds(11);
        //explore
        clippyAudio[5].Play();
        yield return new WaitForSeconds(15);

        //beginBrief
        clippyAudio[6].Play();
        GameObject clippyL = Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyR");
        GameObject clippyR = Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyL");
        clippyL.GetComponent<Clippy>().enableClippy();
        clippyR.GetComponent<Clippy>().enableClippy();
        clippyL.GetComponent<Shooting>().EnableGun();
        clippyR.GetComponent<Shooting>().EnableGun();
    }

    /////// Event-based clippy timers
    IEnumerator PlantTreesAudio()
    {
        yield return new WaitForSeconds(20);
        clippyAudio[15].Play();
    }

    IEnumerator ApproachEnemyAudio()
    {
        GameCtrl.spawnEnemy.SpawnEnemies();
        yield return new WaitForSeconds(30);
        clippyAudio[12].Play();
    }

    IEnumerator MoreTreesAudio()
    {
        yield return new WaitForSeconds(15);
        clippyAudio[17].Play();
    }

    IEnumerator SkipIntroEnableClippy ()
    {
        yield return new WaitForSeconds(5);
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyR").GetComponent<Clippy>().enableClippy();
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyL").GetComponent<Clippy>().enableClippy();
        Debug.Log("Skipped Intro, Clippy enabled");
    }

    //////////////////////////////////////////////////////////////////////// OTHER /////////////////////////////////////////////////////
    private void SkipIntro()
    {
        GameObject.Find("Hatch").GetComponent<Hatch>().SetReady();
        clippyIntroDone = true;
        StartCoroutine(SkipIntroEnableClippy());
    }
}
