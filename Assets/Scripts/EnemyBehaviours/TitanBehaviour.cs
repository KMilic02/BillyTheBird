using UnityEngine;

public class TitanBehaviour : EnemyBehaviour
{
    public GameObject arrowPrefab;
    public Transform hand;
    Animator animator;

    public ParticleSystem clapParticles;

    const int lightningBoltStrikes = 3;
    int lightningBoltStrikesCurrent;
    
    public void Start()
    {
        enemy = GetComponent<Enemy>();
        enemy.health = 1;
        animator = GetComponent<Animator>();
        
        enemy.health = GameManager.difficulty switch
        {
            1 => 4,
            2 => 6,
            _ => 9
        };
    }
    
    public override void updateBehaviour()
    {
        if (enemy.playerRef == null)
        {
            enemy.playerRef = GameObject.FindFirstObjectByType<Player>();
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
            case EnemyState.Attack:
                attack();
                break;
        }
    }

    public override void idle()
    {
        var playerPosition = enemy.playerRef.transform.position;
        animator.SetBool("IsAiming", false);

        if (Vector3.Distance(playerPosition, transform.position) <= enemy.acquisitionRange)
        {
            enemyState = EnemyState.Aggro;
            enemy.cooldownTimer = enemy.attackCooldown / 2.0f;
        }
    }

    public override void acquirePlayer()
    {
        var playerPosition = enemy.playerRef.transform.position;

        if (Vector3.Distance(playerPosition, transform.position) >= enemy.acquisitionRange + enemy.aggroExtraAcquisitionRange)
        {
            enemyState = EnemyState.Idle;
        }
        
        transform.LookAt(playerPosition);
        animator.SetBool("IsAiming", true);

        var rotation = transform.rotation.eulerAngles;
        rotation.x = 0.0f;
        rotation.z = 0.0f;
        //rotation.y += 90f;
        transform.rotation = Quaternion.Euler(rotation);
        
        enemy.cooldownTimer += Time.deltaTime;

        if (enemy.cooldownTimer >= enemy.attackCooldown)
        {
            lightningBoltStrikesCurrent = 0;
            enemyState = EnemyState.Attack;
            enemy.attackTimer = 0.0f;
            enemy.cooldownTimer = 0.0f;
        }
    }

    public override void attack()
    {
        enemy.attackTimer += Time.deltaTime;

        if (enemy.attackTimer >= enemy.attackDelays[0])
        {
            animator.SetTrigger("IsShooting");
            var arrowInstance = Instantiate(arrowPrefab, hand.position, Quaternion.identity);
            arrowInstance.transform.LookAt(enemy.playerRef.transform.position);

            lightningBoltStrikesCurrent++;
            enemy.attackTimer = 0.0f;
            
            if (lightningBoltStrikesCurrent >= lightningBoltStrikes)
                enemyState = EnemyState.Aggro;
        }
    }
}
