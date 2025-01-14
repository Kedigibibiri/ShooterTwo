using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;

    float distanceToTarget = Mathf.Infinity;

    public bool isProvoked = false;
    public bool isAttacking = false;


    CapsuleCollider enemyCollider;
    NavMeshAgent navMeshAgent;
    EnemyHealth health;
    Transform target;

    void Start()
    {
        enemyCollider = GetComponent<CapsuleCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        target = FindObjectOfType<PlayerHealth>().transform;
    }

    void Update()
    {
        if (health.IsDead())
        {
            enemyCollider.enabled = false;
            enabled = false;
            navMeshAgent.enabled = false;
        }

        else
        {
            distanceToTarget = Vector3.Distance(target.position, transform.position);

            if (isProvoked)
            {
                EngageTarget();
            }
            else if (distanceToTarget <= chaseRange)
            {
                isProvoked = true;

            }
        }
    }

    public void OnDamageTaken()
    {
        isProvoked = true;
    }


    void EngageTarget()
    {
        FaceTarget();
        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
            isAttacking = false;
        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance+.1)
        {
            AttackTarget();
            isAttacking = true;
        }
    }

    void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("attack", false); 
        GetComponent<Animator>().SetTrigger("move");
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("attack", true);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
