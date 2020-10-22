using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placeholder class for the project
/// </summary> 

public class PlayerCtrl : MonoBehaviour
{
    //A3
    AudioSource buttonSound;

    private void Start()
    {
        buttonSound = GetComponent<AudioSource>();
    }

    public void PlayAudioBtn()
    {
        buttonSound.Stop();
        buttonSound.Play();
    }
}
