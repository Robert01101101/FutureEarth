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
    public AudioSource buttonSound;
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

    ///////////////////TODO: LINK UP WITH HIT
    public void PlayerHitVignette()
    {
        StartCoroutine(PlayerHitVignetteRoutine());
    }

    IEnumerator PlayerHitVignetteRoutine()
    {
        //Fade In
        vignette._debugForceOn = true;
        vignette._debugForceValue = 0f;
        vignette.ApplyPreset(vignettePresetRed);
        float time = .5f;
        float startVal = 0;
        float endVal = 0.8f;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            float curVal = Mathf.SmoothStep(startVal, endVal, (elapsedTime / time));
            vignette._debugForceValue = curVal;

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Keep for 1 sec
        yield return new WaitForSeconds(1f);

        //Fade Out
        startVal = 0.8f;
        endVal = 0f;
        elapsedTime = 0;
        while (elapsedTime < time)
        {
            float curVal = Mathf.SmoothStep(startVal, endVal, (elapsedTime / time));
            vignette._debugForceValue = curVal;

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        vignette._debugForceOn = false;
        vignette.ApplyPreset(vignettePresetDefault);
    }

    /////////////////////////////////////////////////////////////////////// CLIPPY AUDIO / NARRATIVE HARDCODING ///////////////////////
    public void PlayAudioBtn()
    {
        buttonSound.Stop();
        buttonSound.Play();
    }

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
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyR").GetComponent<Clippy>().enableClippy();
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyL").GetComponent<Clippy>().enableClippy();

    }

    
    public void PlayClippyAudio(int clip)
    {
        if (clippyAudio[clip - 1].isPlaying) clippyAudio[clip-1].Stop();
        clippyAudio[clip].Play();
    }

    private void SkipIntro()
    {
        GameObject.Find("Hatch").GetComponent<Hatch>().SetReady();
        clippyIntroDone = true;
        StartCoroutine(SkipIntroEnableClippy());
    }

    IEnumerator SkipIntroEnableClippy ()
    {
        yield return new WaitForSeconds(5);
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyR").GetComponent<Clippy>().enableClippy();
        Util.FindInactiveChild(GameObject.Find("LocalAvatar"), "clippyL").GetComponent<Clippy>().enableClippy();
        Debug.Log("Skipped Intro, Clippy enabled");
    }
}
