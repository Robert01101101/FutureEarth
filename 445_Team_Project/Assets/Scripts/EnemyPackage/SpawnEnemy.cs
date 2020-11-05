using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    public int xPos;
    public int zPos;
    public int enemyCount;
    public int maxEnemies;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemyDrop());
    }

    IEnumerator EnemyDrop()
    {
        while (enemyCount < maxEnemies)
        {
            //Define range of x values on the map where enemies can spawn from
            xPos = Random.Range(-172, -188);
            //Define range of z values on the map where enemies can spawn from
            zPos = Random.Range(656, 611);
            Instantiate(enemy, new Vector3(xPos, 247, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount += 1;
        }
    }
}
