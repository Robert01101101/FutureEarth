using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    private Transform camera;

    private void Start()
    {
        camera = Util.FindInactiveChild(PlayerCtrl.playerCtrl.gameObject, "CenterEyeAnchor").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(camera, Vector3.up);
    }
}
