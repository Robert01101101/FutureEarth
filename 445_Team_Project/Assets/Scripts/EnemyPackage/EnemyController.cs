using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 30f;
    Transform target;
    UnityEngine.AI.NavMeshAgent agent;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public Transform barrelLocation;
    public float shotPower;
    private Animator anim;
    private AudioSource enemyShotSound;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerCtrl.playerCtrl.transform;
        agent = GetComponent<NavMeshAgent>();
        //Ignore the collisions between its bullets and itself
        Physics.IgnoreLayerCollision(10, 11);
        enemyShotSound = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        // if player within range, follow player
        if (distance <= lookRadius)
        {
            ChasePlayer();

            //if player is within stopping distance, rotate to look at player + attack
            if (distance <= agent.stoppingDistance)
            {
                FaceTarget();
                if (!alreadyAttacked)
                {
                    Attack();
                    //End Attack
                    alreadyAttacked = true;
                    Invoke(nameof(ResetAttack), timeBetweenAttacks);
                }
            }
        }

    }

    void ChasePlayer()
    {
        anim.SetBool("chasePlayer", true);
        agent.SetDestination(target.position);
    }

    void Attack()
    {
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        anim.SetBool("attackPlayer", true);
        yield return new WaitForSeconds(.5f);
        enemyShotSound.Stop();
        enemyShotSound.Play();
        //Attack player
        GameObject bullet = Instantiate(projectile, barrelLocation.position, barrelLocation.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward*shotPower + Vector3.up*shotPower/8, ForceMode.Force);
        anim.SetBool("attackPlayer", false);
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
