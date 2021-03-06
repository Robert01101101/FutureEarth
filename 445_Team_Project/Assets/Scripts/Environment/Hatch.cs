using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatch : MonoBehaviour
{
    //Open hatch when player pushes against it, by triggering an animation.
    public AudioSource ambient, waves1, waves2;
    AudioSource audio;
    Animator animator;
    bool done = false;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!done)
        {
            if (collision.gameObject.tag == "Player")
            {
                animator.SetBool("open", true);
                done = true;
                audio.Play();
                ambient.enabled = true;
                waves1.enabled = true;
                waves2.enabled = true;
            }
        }
    }

    public void SetReady()
    {
        done = false;
    }
}
