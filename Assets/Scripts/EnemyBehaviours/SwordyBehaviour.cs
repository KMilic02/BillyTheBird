using UnityEngine;
using UnityEngine.AI;

public class SwordyBehaviour : EnemyBehaviour
{
    const float maxAggroRange = 20.0f;
    Vector3 startLocation;
    Vector3 currentRandomLocation;
    NavMeshAgent agent;
    Animator animator;
    const float attackRange = 1.5f;


    const float minPatrolTimer = 4.0f;
    const float maxPatrolTimer = 6.0f;
    const float patrolRange = 4.0f;

    public AudioClip attackClip;
    public AudioClip detectClip;
    bool hasDetectedPlayer = false;

    float patrolTimer;
    
    public void Start()
    {
        enemy = GetComponent<Enemy>();
        enemy.health = 1;
        startLocation = transform.position;
        patrolTimer = Random.Range(minPatrolTimer, maxPatrolTimer);
        currentRandomLocation = startLocation;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    
    public override void updateBehaviour()
    {
        if (enemy.playerRef == null)
        {
            enemy.playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            return;
        }
        
        switch (enemyState)
        {
            case EnemyState.Idle:
                idle();
                break;
            case EnemyState.Aggro:
                acquirePlayer();
                break;
        }
        UpdateAnimation();
    }

    public override void idle()
    {
        patrolTimer -= Time.deltaTime;
        agent.SetDestination(currentRandomLocation);

        if (patrolTimer <= 0.0f)
        {
            patrolTimer = Random.Range(minPatrolTimer, maxPatrolTimer);
            currentRandomLocation = Random.insideUnitSphere * patrolRange;
        }
        var playerPosition = enemy.playerRef.transform.position;

        if (Vector3.Distance(playerPosition, transform.position) <= enemy.acquisitionRange && Vector3.Distance(startLocation, transform.position) <= patrolRange)
        {
            enemyState = EnemyState.Aggro;
             if (!hasDetectedPlayer)
            {
                AudioManager.Instance.PlaySFX(detectClip, 0.7f);
                hasDetectedPlayer = true;
            }
        }
    }

    public override void acquirePlayer()
    {
        var playerPosition = enemy.playerRef.transform.position;

        if (Vector3.Distance(playerPosition, transform.position) >= enemy.acquisitionRange + enemy.aggroExtraAcquisitionRange
            || Vector3.Distance(startLocation, transform.position) >= maxAggroRange)
        {
            enemyState = EnemyState.Idle;
            agent.SetDestination(currentRandomLocation);
        }
        
        transform.LookAt(playerPosition);

        var rotation = transform.rotation.eulerAngles;
        rotation.x = 0.0f;
        rotation.z = 0.0f;
        transform.rotation = Quaternion.Euler(rotation);

        agent.SetDestination(playerPosition);
        
        enemy.cooldownTimer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(playerPosition, transform.position);

        if (distanceToPlayer <= attackRange && enemy.cooldownTimer >= enemy.attackCooldown)
        {
            Attack();
        }
    }

void UpdateAnimation()
{
    float speed = agent.velocity.magnitude;
    animator.SetFloat("Speed", Mathf.Clamp(speed, 0f, 1f));
}

void Attack()
{
    enemy.cooldownTimer = 0f;
    animator.SetTrigger("Attack");
    AudioManager.Instance.PlaySFX(attackClip, 0.9f);
}

public void AttackHit()
{
    float hitRadius = 0.8f;
    Vector3 hitCenter = transform.position + transform.forward * 1.0f;

    Collider[] hits = Physics.OverlapSphere(hitCenter, hitRadius);

    foreach (Collider hit in hits)
    {
        if (hit.CompareTag("Player"))
        {
            hit.GetComponent<Player>().IOnDamage(1);
        }
    }
    Debug.Log("HIT FRAME");
}

}
