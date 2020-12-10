using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioSource oceanSound;
    private bool keepFadingIn;
    private bool keepFadingOut;
    private IEnumerator FadeInSound;
    private IEnumerator FadeOutSound;

    void Start()
    {
        oceanSound = GetComponent<AudioSource>();
        keepFadingIn = false;
        keepFadingOut = false;
    }

    //When player enters beach, play audio
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //FadeInSound = FadeIn(oceanSound, 0.01f, 1);
            //StartCoroutine("FadeInSound");
            oceanSound.Play();
            oceanSound.loop = true;
        }
    }

    //When player leaves beach, stop audio
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //FadeOutSound = FadeIn(oceanSound, 0.01f);
            //StartCoroutine("FadeOutSound");
        }
    }

    private IEnumerator FadeIn (AudioSource sound, float speed, float maxVolume) 
    {
        keepFadingIn = true;
        keepFadingOut = false;
        sound.volume = 0;
        while (sound.volume < maxVolume && keepFadingIn)
        {
            sound.volume += speed;
            yield return new WaitForSeconds(0.1f);
        }
        sound.Play();
        sound.loop = true;
    }

    private IEnumerator FadeIn(AudioSource sound, float speed)
    {
        keepFadingIn = false;
        keepFadingOut = true;
        while (sound.volume >= speed && keepFadingOut)
        {
            sound.volume += speed;
            yield return new WaitForSeconds(0.1f);
        }
        sound.volume = 0;
        sound.Stop();
        sound.loop = false;
    }
}
