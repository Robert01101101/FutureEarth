using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
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
    }
}
