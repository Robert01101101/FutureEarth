using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float bulletDamage = 10f;

    void Awake()
    {
        //Ignore the collisions between its bullets and itself
        Physics.IgnoreLayerCollision(9, 12);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            // do damage here, for example:
            collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            Debug.Log(collision);
        } 
    }
}
