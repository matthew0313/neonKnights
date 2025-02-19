using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask IsGround, IsPlayer;

    public float health;

    // d이동하는거 
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float WalkRangee;

    // 공격격
    public float attackCooldown;
    private bool alreadyAttacked;
    public GameObject projectile;

    // 탐지나 공격 범위위
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // 플레이어 감지 범위 체크
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, IsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, IsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patrol();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
    }

    private void Patrol()
    {
        if (!walkPointSet)
        {
            SearchPOint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //도착하면ㅇ 새롭게게
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchPOint()
    {
        float randomZ = Random.Range(-WalkRangee, WalkRangee);
        float randomX = Random.Range(-WalkRangee, WalkRangee);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, IsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

private void AttackPlayer()
{
    agent.SetDestination(transform.position);
    transform.LookAt(player);

    if (!alreadyAttacked)
    {
        StartCoroutine(ShootBullet());
    }
}

private IEnumerator ShootBullet()
{
    alreadyAttacked = true;

    Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
    rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
    rb.AddForce(transform.up * 8f, ForceMode.Impulse);

    Destroy(rb.gameObject, 3f);

    yield return new WaitForSeconds(3f);
    alreadyAttacked = false;
}


    private void REsetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0 )
        {
            Invoke(nameof(DestroyEnemy), .3f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

}
