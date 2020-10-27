using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Permissions;
using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;
    public Transform gunEnd;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    public float weaponRange = 100f;
    public float damage = 10f;
    private LineRenderer line;
    private AudioSource gunAudio;
    private WaitForSeconds shotDuration = new WaitForSeconds(.5f);

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        line = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
}

    public void TriggerShoot()
    {
        GetComponent<Animator>().SetTrigger("Fire");
        StartCoroutine(ShotEffect());
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();
        yield return shotDuration;
    }


    //This function creates the bullet behavior
    void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        // failed attempt to get raycast to work, any Solutions?

        //line.SetPosition(0, barrelLocation);
        //RaycastHit hit;
        //if (Physics.Raycast(barrelLocation.position, barrelLocation.forward, out hit, weaponRange))
        //{
        //UnityEngine.Debug.Log(hit.transform.name);
        //Target target = hit.transform.GetComponent<Target>();
        //line.SetPosition(1, hit.point);
        //Destroy(line, 0.5f);

        //if target is active, target take damage
        //if (target != null)
        //{
        //target.TakeDamage(damage);
        //}
        //}
        //else
        //{
           // line.setPosition(1, (barrelLocation.forward * weaponRange));
        //}

        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }


        //Create a bullet and add force on it in direction of the barrel
        Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

}
