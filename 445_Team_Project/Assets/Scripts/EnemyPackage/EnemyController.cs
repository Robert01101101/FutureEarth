using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;
    Transform target;
    UnityEngine.AI.NavMeshAgent agent;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public Transform barrelLocation;
    public float shotPower = 500f;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        //Ignore the collisions between its bullets and itself
        Physics.IgnoreLayerCollision(10, 11);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        // if player within range, follow player
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            //if player is within stopping distance, rotate to look at player + attack
            if (distance <= agent.stoppingDistance)
            {
                FaceTarget();
                if (!alreadyAttacked)
                {
                    //Attack player
                    GameObject bullet = Instantiate(projectile, barrelLocation.position, barrelLocation.rotation);
                    bullet.GetComponent<Rigidbody>().AddForce(transform.forward * shotPower);
                    Destroy(bullet, 2);
                    //End Attack
                    alreadyAttacked = true;
                    Invoke(nameof(ResetAttack), timeBetweenAttacks);
                }
            }
        }

    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //Change to face target
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //Visualize lookRadius in Scene 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
