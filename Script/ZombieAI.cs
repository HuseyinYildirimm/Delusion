using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class ZombieAI : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent; // diðer zombileri de görmesini saðlýycaz
    Animator anim;
    Transform target;

    [HideInInspector] public FieldOfView fow;

    #region Ragdoll
    Rigidbody[] rigRagdoll;
    Collider[] colRagdoll;
    bool ragdollMode = false;

    #endregion

    bool isDeathing = false;
    bool canAttack = true;
    public bool isAttacking = false;
    bool generated = false;
    int randomRunIndex;
    int patrollingIndex;
    bool isNotGround;


    public bool isPatrolling;
    public Transform centerPoint;
    [SerializeField] float patrollRange;

    [SerializeField] float distance;
    [SerializeField] float chaseDistance = 8f;
    [SerializeField] float turnSpeed = 8f;
    [SerializeField] float damageAmount = 25f;

    #region Audio
    [Header("Audio")]
    public AudioSource attackAS;
    public AudioClip[] attackAC;
    public AudioSource chaseAS;
    public AudioClip[] chaseAC;
    public AudioSource patrollAS;
    public AudioClip[] patrollAC;
    AudioClip nullClipChase;
    AudioClip nullClipPatroll;

    bool isSoundAttack;
    bool isSoundChase;
    bool isSoundPatroll;
    bool isPlaying = false;
    #endregion

    public void Awake()
    {
        fow = GetComponentInChildren<FieldOfView>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = Random.Range(9, 12);
        chaseDistance = agent.stoppingDistance + 2;

        anim = GetComponent<Animator>();
        randomRunIndex = Random.Range(0, 6);
        patrollingIndex = Random.Range(0, 3);

        #region Audio
        attackAS = GetComponent<AudioSource>();
        chaseAS = GetComponent<AudioSource>();
        nullClipChase = chaseAC[Random.Range(0, chaseAC.Length)];
        patrollAS = GetComponent<AudioSource>();
        nullClipPatroll = patrollAC[Random.Range(0, patrollAC.Length)];

        attackAS.Stop();
        #endregion 

        GetRagdollbits();
        RagdollModeOff();

    }

    
    #region ****RAGDOLL****
    void GetRagdollbits()
    {
        rigRagdoll = GetComponentsInChildren<Rigidbody>();
        colRagdoll = GetComponentsInChildren<Collider>();
    }

    void RagdollModeOn()
    {
        ragdollMode = true;
        foreach (var rig in rigRagdoll)
        {
            rig.isKinematic = false;
        }
        foreach (Collider col in colRagdoll)
        {
            col.enabled = true;
        }
        anim.enabled = false;
        agent.enabled = false;
        agent.updatePosition = false;
        agent.updateRotation = false;
        canAttack = false;
        isDeathing = true;
    }

    void RagdollModeOff()
    {
        foreach (Rigidbody rig in rigRagdoll)
        {
            rig.isKinematic = true;

        }

        foreach (Collider col in colRagdoll)
        {
            col.enabled = true;
        }
        //zombieHealth.RagdollCollider();       //xd
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        RagdollModeOn();

        Rigidbody hitRigidbody = rigRagdoll.OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitPoint)).First();

        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);


    }
    #endregion

    public void Update()
    {

        distance = Vector3.Distance(transform.position, target.position);

        //play the ýdle anim  Player enters the distance Zombie send a raycast  Zombie saw to playerafter play the scream anim   and play the run anim

        if (distance <= chaseDistance && !isDeathing && canAttack && !PlayerHealth.singleton.isDead)
        {
            anim.SetBool("isChase", false);
            anim.SetBool("isPatrolling", false);
            AttackPlayer();
            isAttacking = true;
        }
        if (distance > chaseDistance && !isDeathing && !isAttacking && fow.isEntry)
        {
            anim.SetBool("isPatrolling", false);
            isPatrolling = false;
            ChasePlayer();
        }
        if (!fow.isEntry && !isDeathing && !isAttacking)
        {
            isPatrolling = true;
            Patrolling();
        }
        else if (isDeathing)
        {
            DisableZombie();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RagdollModeOn();
        }
    }

    #region ***** PATROLL *****
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 0.5f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }
    void Patrolling()
    {
        Vector3 point;

        if (RandomPoint(centerPoint.position, patrollRange, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.red, 1f);

            agent.SetDestination(point);

            anim.SetBool("isPatrolling", true);
            anim.SetInteger("PatrollingIndex", patrollingIndex);

            switch (patrollingIndex)
            {
                case 1:
                    agent.speed = 2.5f;
                    randomRunIndex = 3;
                    break;

                case 2:
                    agent.speed = 0f;
                    randomRunIndex = 3;
                    break;

                default:
                    isNotGround = true;
                    agent.speed = 3f;
                    break;
            }
            isSoundPatroll = true;
            if (!isPlaying)
            {
                StartCoroutine(PatrollTime());
                
            }


        }
    }
    #endregion  


    public void ZombieDamageEffect()//deðiþtirilcek
    {
        anim.Play("Reaction_Hit");
    }

    public void ZombieDeathAnim()//zombi ölüm
    {
        agent.updatePosition = false;
        agent.enabled = false;

        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isPatrolling", false);
        isDeathing = true;
        anim.SetTrigger("isDeathing");
    }

    void ChasePlayer()//player kovalamaca
    {
        do
        {
            if (!ragdollMode)
            {

                agent.updateRotation = true;
                agent.updatePosition = true;
                agent.SetDestination(target.position);

                switch (randomRunIndex)
                {
                    case 1:
                        agent.speed = 17f;
                        agent.acceleration = 30f;
                        break;
                    case 2:
                        agent.speed =5f;
                        break;

                    case 3:
                        agent.speed = 4f;

                        break;
                    case 4:
                        agent.speed = 8f;
                        break;
                    case 5:
                        agent.speed =22f;
                        agent.acceleration = 35f;
                        break;
                    case 6:
                        agent.speed = 34f;
                        agent.acceleration = 45f;
                        break;
                    default:
                        agent.speed = 20f;
                        agent.acceleration = 35f;
                        break;

                }

                isSoundChase = true;
                anim.SetBool("isChase", true);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isRunning", true);
                anim.SetInteger("RunIndex", randomRunIndex);


                if (!isPlaying)
                {
                    StartCoroutine(ChaseTime());
                }
            }
        } while (chaseDistance < 3f);

    }

    void AttackPlayer()
    {

        Vector3 direction = target.position - transform.position;//bakýcaðýmýz pozisyonu belirledik
        direction.y = 0;//yukarýya ve aþaðýya bakamýcagý için 0'a eþitledik

        transform.rotation = Quaternion.Slerp(transform.rotation,
        Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);//yönümüzü player a çevirdik

        if (true)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;

            var randomAttackIndex = Random.Range(0, 5);
            anim.SetInteger("AttackIndex", randomAttackIndex);
            anim.SetBool("isChase", false);
            anim.SetBool("isAttacking", true);
            anim.SetBool("isRunning", false);
        }

        isSoundAttack = true;

        generated = false;
    }

    void DisableZombie()
    {
        canAttack = false;
        anim.SetBool("isChase", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
    }

    void DamageZombie() //AnimationEvent
    {
        if (distance <= chaseDistance)
        {
            PlayerHealth.singleton.DamagePlayer(damageAmount);
        }

    }

    IEnumerator AttackTime() //Animation Event
    {
        if (isSoundAttack)
        {
            agent.enabled = false;
            canAttack = false;

            attackAS.clip = attackAC[Random.Range(0, attackAC.Length)];
            attackAS.loop = false;
            attackAS.volume = 0.75f;
            attackAS.Play();

            yield return new WaitForSeconds(0.5f);
            isAttacking = false;
            canAttack = true;
            isSoundAttack = !isSoundAttack;
            agent.enabled = true;

            anim.SetBool("isAttacking", false);

            yield return new WaitForSeconds(2f);
        }
        else yield break;

    }

    IEnumerator ChaseTime()
    {
        if (isSoundChase )
        {
            //-
            isPlaying = true;

            yield return new WaitForSeconds(1f);
            chaseAS.clip = nullClipChase;
            chaseAS.loop = false;
            chaseAS.volume = 0.6f;
            chaseAS.Play();
          
            yield return new WaitForSeconds(6f);
            isSoundChase = !isSoundChase;

            isPlaying = false;
            //-
            if (distance <= chaseDistance)
            {
                chaseAS.Stop();
            }
        }
        else yield break;

    }

    IEnumerator PatrollTime()
    {
        if (isSoundPatroll )
        {
            isPlaying = true;

            yield return new WaitForSeconds(1f);
            patrollAS.clip = nullClipPatroll;
            patrollAS.loop = false;
            patrollAS.volume = 0.2f;
            patrollAS.Play();
          
            yield return new WaitForSeconds(5f);
            isSoundPatroll = !isSoundPatroll;
           
            isPlaying = false;
           
        }
        else yield break;
    }


}