using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public GameObject destroyedVersion;


    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);

    }
}
