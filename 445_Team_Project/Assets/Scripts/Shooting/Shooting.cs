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
    private GameObject bulletInstance;
    private bool enabled = false;
    public GameObject muzzleFlash;

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            if (OVRInput.GetDown(shootingButton, OVRInput.Controller.LTouch) && gameObject.name == "clippyL" || OVRInput.GetDown(shootingButton, OVRInput.Controller.RTouch) && gameObject.name == "clippyR")
            {
                shoot();
            }
        }
    }

    //Shooting
    void shoot()
    {
        muzzleFlash = Instantiate(muzzleFlash, barrelLocation.position, barrelLocation.rotation);
        bulletInstance = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        bulletInstance.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        PlayerCtrl.playerCtrl.laser.Stop();
        PlayerCtrl.playerCtrl.laser.Play();
        Destroy(bulletInstance, 2);
    }

    public void EnableGun()
    {
        enabled = true;
    }

}
