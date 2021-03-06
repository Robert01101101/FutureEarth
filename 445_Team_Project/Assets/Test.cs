using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject bulletPrefab;
    private GameObject bulletInstance;
    public Transform barrelLocation;
    float shotPower = 2f;

    bool done = false;

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !done)
        {
            bulletInstance = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
            bulletInstance.transform.rotation *= Quaternion.Euler(0, -90, 0);
            bulletInstance.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        }
    }
}
