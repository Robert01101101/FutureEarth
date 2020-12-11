using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 30f;
    Transform target;
    UnityEngine.AI.NavMeshAgent agent;

    //Enemy Port
    public float health = 50f;
    public float origHealth = 50 * 2;
    public GameObject [] partArray;
    int index;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public Transform barrelLocation;
    public float shotPower;
    private Animator anim;
    private AudioSource enemyShotSound;

    //Garbage cleaning
    private bool friendly = false;
    private bool lookingForGarbage = true;
    private bool stillInGarbage = true;
    private Garbage garbage;

    //Eyes
    public MeshRenderer eyeRenderer, altIndicator;
    public Material goodMat, offMat;
    

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
        if (!friendly)
        {
            ///// Attack /////
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
        } else
        {
            ///// Clean /////
            if (friendly)
            {
                if (lookingForGarbage) ChaseGarbage();
            }
        }
    }

    ///////////////////////////////////////////////////////////// PORTED OVER FROM ENEMY CLASS ////////////////////////////////////////////////
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        // other stuff you want to happen when enemy takes damage
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameCtrl.spawnEnemy.RemoveEnemy();
        int pick = Random.Range(0, partArray.Length+1);
        Debug.Log(pick);

        if (pick < partArray.Length)
        {
            //spawn part
            GameObject spawnPart = partArray[pick];
            Instantiate(spawnPart, transform.position, transform.rotation);
            Destroy(gameObject);
        } else
        {
            //turn into good bot
            DisableBot();
        }
    }

    ///////////////////////////////////////////////////////////////////// ATTACK //////////////////////////////////////////////////
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

    ///////////////////////////////////////////////////////////////////// GARBAGE COLLECTION //////////////////////////////////////////////////
    bool waitingForRepair = false;
    void DisableBot()
    {
        eyeRenderer.material = offMat;
        altIndicator.material = offMat;
        waitingForRepair = true;
        friendly = true;
        lookingForGarbage = false;
        health = origHealth;
        anim.SetBool("awaitingRepair", true);
        anim.SetBool("chasePlayer", false);
        anim.SetBool("grabGarbage", false);

        Debug.Log("Bot Disabled");
    }


    IEnumerator TurnIntoGoodBot()
    {
        //TODO: play metal bang sound to indicate it worked

        yield return new WaitForSeconds(1);
        eyeRenderer.material = goodMat;
        altIndicator.material = goodMat;
        lookingForGarbage = true;

        anim.SetBool("awaitingRepair", false);
    }
    
    void ChaseGarbage()
    {
        lookingForGarbage = false;
        anim.SetBool("chasePlayer", true);
        if (agent.isStopped) agent.isStopped = false;

        garbage = GameCtrl.GetClosestGarbagePile(this.transform).GetComponent<Garbage>();
        agent.SetDestination(garbage.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Garbage")
        {
            if (!agent.isStopped) agent.isStopped = true;
            anim.SetBool("grabGarbage", true);
            garbage.AddBot(this);

            stillInGarbage = true;
            StartCoroutine(MonitorIfNeedToMoveCloser());
        } else if (other.gameObject.tag == "Player") {
            VerifyRepair();
        }
    }

    public void VerifyRepair()
    {
        if (waitingForRepair)
        {
            StartCoroutine(TurnIntoGoodBot());
            waitingForRepair = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Garbage")
        {
            stillInGarbage = true;
        }
    }

    IEnumerator MonitorIfNeedToMoveCloser()
    {
        while (stillInGarbage)
        {
            stillInGarbage = false;
            yield return new WaitForSeconds(1);
        }

        //garbage too far. stop animation & scooch closer
        anim.SetBool("grabGarbage", false);
        garbage.RemoveBot(this);
        yield return new WaitForSeconds(2);
        ChaseGarbage();
    }

    public void FindNewPile()
    {
        stillInGarbage = false;
        anim.SetBool("grabGarbage", false);
        anim.SetBool("chasePlayer", true);
        ChaseGarbage();
    }

    ///////////////////////////////////////////////////////////////////// DEBUG //////////////////////////////////////////////////
    //Visualize lookRadius in Scene 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }


}
