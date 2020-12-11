using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateChewing : MonoBehaviour
{

    public ParticleSystem chewingParticles;

    //On collision with mouth, activate the chewing animation
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.name == "mouth")
        {
            Chew();
        }
    }

    //Activate Chewing Particles
    public void Chew()
    {
        Instantiate(chewingParticles, transform.position, Quaternion.identity);
    }
}
