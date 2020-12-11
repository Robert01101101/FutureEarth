using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelper : MonoBehaviour
{
    public EnemyController enemyCtrl;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemyCtrl.VerifyRepair();
        }
    }
}
