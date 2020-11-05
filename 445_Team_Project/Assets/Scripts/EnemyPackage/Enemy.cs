using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public GameObject spawnPart;
    int index;

    void Start()
    {

    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        // other stuff you want to happen when enemy takes damage
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        Instantiate(spawnPart, transform.position, transform.rotation);
    }
}
