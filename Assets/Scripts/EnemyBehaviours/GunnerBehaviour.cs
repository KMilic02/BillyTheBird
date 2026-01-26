using UnityEngine;

public class GunnerBehaviour : EnemyBehaviour
{
    public GameObject arrowPrefab;

    public void Start()
    {
        enemy = GetComponent<Enemy>();
        enemy.health = 1;
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
            case EnemyState.Attack:
                attack();
                break;
        }
    }

    public override void idle()
    {
        var playerPosition = enemy.playerRef.transform.position;

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

        var rotation = transform.rotation.eulerAngles;
        rotation.x = 0.0f;
        rotation.z = 0.0f;
        transform.rotation = Quaternion.Euler(rotation);
        
        enemy.cooldownTimer += Time.deltaTime;

        if (enemy.cooldownTimer >= enemy.attackCooldown)
        {
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
            var arrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            arrowInstance.transform.LookAt(enemy.playerRef.transform.position);
            arrowInstance.transform.position = transform.position;
            
            enemyState = EnemyState.Aggro;
        }
    }
}
