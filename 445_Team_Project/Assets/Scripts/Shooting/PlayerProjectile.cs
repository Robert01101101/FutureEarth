using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float bulletDamage = 10f;
    [SerializeField] GameObject hitEffect, hitEffectEnemy;

    void Awake()
    {
        //Ignore the collisions between its bullets and itself
        Physics.IgnoreLayerCollision(9, 12);
    }

    private void Start()
    {
        StartCoroutine(BulletTimeout());
    }

    IEnumerator BulletTimeout()
    {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }

    //TODO: spawn audiosource with ricochet sound depending on what it hit
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            // do damage here, for example:
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(bulletDamage);
            Debug.Log(collision);
            SpawnAudio(collision.GetContact(0).point, true);
            Destroy(gameObject);
        } else { //else if (collision.gameObject.layer == 8) {
            SpawnAudio(collision.GetContact(0).point, false);
            Destroy(gameObject);
        }
    }

    private void SpawnAudio(Vector3 spawnPos, bool enemyHit)
    {
        //Instantiate hit effect at hit location
        Instantiate(enemyHit ? hitEffectEnemy : hitEffect, spawnPos, Quaternion.identity);
    }
}
