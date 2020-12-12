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

        if (agent.isStopped) Debug.Log("NavAgent stopped");
        if (!agent.isActiveAndEnabled) Debug.Log("NavAgent inactive");
        if (!agent.isOnNavMesh) Debug.Log("NavAgent not on nav mesh");
        if (agent.isPathStale) Debug.Log("Agent Path is stale");
        if (agent.isOnOffMeshLink) Debug.Log("Agent isOnOffMeshLink");
    }

    ///////////////////////////////////////////////////////////// PORTED OVER FROM ENEMY CLASS ////////////////////////////////////////////////
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        // other stuff you want to happen when enemy takes damage
        if (health <= 0)
        {
            if (!dying) Die();
        }
    }

    private bool dying = false;
    void Die()
    {
        dying = true;
        GameCtrl.spawnEnemy.RemoveEnemy();
        
        if (GameCtrl.partsFound < 3) { 
            if (GameCtrl.partsFound == 0) { GameCtrl.partsFound++; Instantiate(partArray[0], transform.position, transform.rotation); Destroy(gameObject); }
            else if (GameCtrl.partsFound == 1) { GameCtrl.partsFound++; Instantiate(partArray[1], transform.position, transform.rotation); Destroy(gameObject); }
            else if (GameCtrl.partsFound == 2) { GameCtrl.partsFound++; Instantiate(partArray[2], transform.position, transform.rotation); Destroy(gameObject); }
        } else
        {
            int pick = Random.Range(0, 7);
            if (pick > 5 && GameCtrl.GetWaterFilterCount() < 1) pick = Random.Range(1, 4);
            if (pick < 6)
            {
                //spawn part (law of large number dictates that player will get 1 chip, 2 pumps, 3 tubes)
                if (pick == 1 || pick == 2) { pick = 1; }
                else if (pick > 2) pick = 2;
                GameObject spawnPart = partArray[pick];
                Instantiate(spawnPart, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else
            {
                //turn into good bot
                DisableBot();
                if (!PlayerCtrl.explainedBotSlap)
                {
                    PlayerCtrl.explainedBotSlap = true;
                    PlayerCtrl.playerCtrl.PlayClippyAudio(20);
                }
            }
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
        if (!agent.isStopped) agent.isStopped = true;
        agent.stoppingDistance = 0;

        Debug.Log("Bot Disabled");
    }


    IEnumerator TurnIntoGoodBot()
    {
        //TODO: play metal bang sound to indicate it worked
        eyeRenderer.material = goodMat;
        altIndicator.material = goodMat;

        yield return new WaitForSeconds(1);
        
        lookingForGarbage = true;
        anim.SetBool("awaitingRepair", false);
    }
    
    void ChaseGarbage()
    {
        lookingForGarbage = false;
        anim.SetBool("chasePlayer", true);
        if (agent.isStopped) agent.isStopped = false;

        if (garbage == null) garbage = GameCtrl.GetClosestGarbagePile(this.transform).GetComponent<Garbage>();
        agent.SetDestination(garbage.transform.position);

        Debug.Log("Chasing garbage");
        Debug.Log("Garbage target = " + garbage.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (friendly)
        {
            Debug.Log("hit: " + other.gameObject.name);
            if (other.gameObject.tag == "Garbage")
            {
                if (!agent.isStopped) agent.isStopped = true;
                anim.SetBool("grabGarbage", true);
                if (garbage.gameObject != other.gameObject) garbage = other.gameObject.GetComponent<Garbage>();
                garbage.AddBot(this);

                stillInGarbage = true;
                StartCoroutine(MonitorIfNeedToMoveCloser());
            }
            else if (other.gameObject.tag == "Player")
            {
                VerifyRepair();
            }
        }
    }

    public void VerifyRepair()
    {
        if (waitingForRepair)
        {
            if (!PlayerCtrl.slappedBot)
            {
                PlayerCtrl.slappedBot = true;
                PlayerCtrl.playerCtrl.PlayClippyAudio(21);
            }

            StartCoroutine(TurnIntoGoodBot());
            waitingForRepair = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (friendly)
        {
            if (other.gameObject.tag == "Garbage")
            {
                stillInGarbage = true;
            }
        }   
    }

    IEnumerator MonitorIfNeedToMoveCloser()
    {
        while (stillInGarbage)
        {
            stillInGarbage = false;
            yield return new WaitForSeconds(1);
        }

        Debug.Log("moving closer");
        //garbage too far. stop animation & scooch closer
        anim.SetBool("grabGarbage", false);
        garbage.RemoveBot(this);
        yield return new WaitForSeconds(2);
        ChaseGarbage();
    }

    
    Vector3 lastPos = Vector3.zero;
    IEnumerator BugFixMonitor()
    {
        while (friendly)
        {
            if (Vector3.Distance(transform.position, lastPos) < .1f)
            {
                //hasn't moved
            } else
            {

            }
            lastPos = transform.position;
            yield return new WaitForSeconds(8);
        }
    }

    public void FindNewPile()
    {
        stillInGarbage = false;
        anim.SetBool("grabGarbage", false);
        anim.SetBool("chasePlayer", true);
        garbage = GameCtrl.GetClosestGarbagePile(this.transform).GetComponent<Garbage>();
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
