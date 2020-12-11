using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int width;
    public int height;
    public int minEnemies;
    public int maxEnemies;
    private int enemyCount = 0;
    private bool spawnRoutineRunning = true;

    private void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            EnemyDrop();
        }
        spawnRoutineRunning = false;
    }

    private void Update()
    {
        if (!spawnRoutineRunning)
        {
            if (enemyCount < maxEnemies)
            {
                //Debug.Log("Spawn New Enemy");
                if (enemyCount < minEnemies)
                {
                    //Add enemies quickly
                    StartCoroutine(AddEnemy(1, 3));
                }
                else
                {
                    //Add enemies slowly
                    StartCoroutine(AddEnemy(5, 20));
                }
            }
        }
       // Debug.Log(enemyCount);
    }

    IEnumerator AddEnemy(int minDelay, int maxDelay)
    {
        spawnRoutineRunning = true;
        int delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);
        EnemyDrop();
        spawnRoutineRunning = false;
    }

    private void EnemyDrop()
    {
        //ensure not spawning right next to player
        bool closeToPlayer = true; bool notGround = true;
        Vector3 alignedSpawnPosition = Vector3.zero;

        while (closeToPlayer || notGround)
        {
            Vector3 spawnCenter = transform.position;

            //Determine random location based on the spawn center
            Vector2 offset = new Vector2(Random.Range(-width / 2, width / 2), Random.Range(-height / 2, height / 2));
            Vector3 spawnPosition = new Vector3(spawnCenter.x + offset.x, spawnCenter.y+50, spawnCenter.z + offset.y);
            alignedSpawnPosition = spawnPosition;

            //determine correct y position using raycast to ground

            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
            {
                //Place at hit
                alignedSpawnPosition = hit.point;
                if (hit.collider.gameObject.tag != "Mountain" && hit.collider.gameObject.tag != "Garbage" && hit.collider.gameObject.layer == 8) notGround = false;
            }

            if ((alignedSpawnPosition - PlayerCtrl.playerCtrl.gameObject.transform.position).magnitude > 10) closeToPlayer = false;
            //TODO: Add more checks -> Ensure player not looking in that direction
        }

        //Spawn Enemy
        GameObject spawnedEnemy = Instantiate(enemyPrefab, alignedSpawnPosition, Quaternion.identity);
        //Add random rotation
        spawnedEnemy.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
        enemyCount++;
    }

    /////////////////////////////////////// PUBLIC INTERFACE
    public void RemoveEnemy()
    {
        enemyCount--;
    }

    ///////////////////////////////////////////////////////////////////// DEBUG //////////////////////////////////////////////////
    //Visualize spawn area in Scene 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 20, height));
    }
}
