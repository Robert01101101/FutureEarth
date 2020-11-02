using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform barrelLocation;
    public float shotPower = 500f;
    public OVRInput.Button shootingButton;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(shootingButton, OVRInput.GetActiveController()))
        {
            shoot();
        }
    }

    //Shooting
    void shoot()
    {
        Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
    }
}
