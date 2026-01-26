using UnityEngine;
using UnityEngine.AI;

public class SwordyBehaviour : EnemyBehaviour
{
    const float maxAggroRange = 20.0f;
    Vector3 startLocation;
    Vector3 currentRandomLocation;
    NavMeshAgent agent;

    const float minPatrolTimer = 4.0f;
    const float maxPatrolTimer = 6.0f;
    const float patrolRange = 4.0f;

    float patrolTimer;
    
    public void Start()
    {
        enemy = GetComponent<Enemy>();
        enemy.health = 1;
        startLocation = transform.position;
        patrolTimer = Random.Range(minPatrolTimer, maxPatrolTimer);
        currentRandomLocation = startLocation;
        agent = GetComponent<NavMeshAgent>();
    }
    
    public override void updateBehaviour()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                idle();
                break;
            case EnemyState.Aggro:
                acquirePlayer();
                break;
        }
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
    }
}
