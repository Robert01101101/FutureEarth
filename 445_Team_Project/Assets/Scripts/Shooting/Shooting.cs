using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform barrelLocation;
    public float shotPower = 500f;
    public float damage = 10f;
    public OVRInput.Button shootingButton;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(shootingButton, OVRInput.Controller.LTouch) && gameObject.name == "clippyL" || OVRInput.GetDown(shootingButton, OVRInput.Controller.RTouch) && gameObject.name == "clippyR")
        {
            shoot();
        }
    }

    //Shooting
    void shoot()
    {
        Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        Destroy(bulletPrefab, 2);
    }

}
