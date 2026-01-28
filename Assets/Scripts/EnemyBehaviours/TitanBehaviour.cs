using Unity.Mathematics.Geometry;
using UnityEngine;

public class TitanBehaviour : EnemyBehaviour
{
    public GameObject arrowPrefab;
    public Transform hand;
    Animator animator;

    Vector3 startingPosition;

    public ParticleSystem clapParticles;

    public ParticleSystem tpParticles_1;
    public ParticleSystem tpParticles_2;

    bool shooting;
    bool clapping;
    bool clapReturn;
    
    const int lightningBoltStrikes = 3;
    int lightningBoltStrikesCurrent;

    float clapTimer;
    const float clapDelayMin = 7.0f;
    const float clapDelayMax = 10.0f;

    float clapReturnTimer = 0.0f;

    public AudioClip lightningClip;
    public AudioClip clapClip;
    public AudioClip teleportClip;
    
    public void Start()
    {
        clapTimer = Random.Range(clapDelayMin, clapDelayMax);
        enemy = GetComponent<Enemy>();
        enemy.health = 1;
        animator = GetComponent<Animator>();
        
        enemy.health = GameManager.difficulty switch
        {
            1 => 4,
            2 => 6,
            _ => 9
        };
        
        startingPosition = transform.position;
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
        clapTimer -= Time.deltaTime;

        if (clapTimer <= 0.0f)
        {
            clapTimer = Random.Range(clapDelayMin, clapDelayMax);
            clapping = true;
            enemyState = EnemyState.Attack;
            enemy.attackTimer = 0.0f;
            tpParticles_1.Play();
            tpParticles_2.Play();
            AudioManager.Instance.PlaySFX(teleportClip, 0.8f);

            transform.position = playerPosition + enemy.playerRef.transform.forward * 1.0f;
            enemy.cooldownTimer = Mathf.Min(enemy.attackCooldown - 1.0f, enemy.cooldownTimer);
            animator.SetTrigger("IsClaping");
        }
        
        if (enemy.cooldownTimer >= enemy.attackCooldown)
        {
            lightningBoltStrikesCurrent = 0;
            enemyState = EnemyState.Attack;
            enemy.attackTimer = 0.0f;
            enemy.cooldownTimer = 0.0f;

            clapTimer = Mathf.Max(clapTimer, 1.5f);
            
            shooting = true;
            animator.SetTrigger("IsShooting");
        }
    }

    public override void attack()
    {
        if (shooting)
        {
            enemy.attackTimer += Time.deltaTime;

            if (enemy.attackTimer >= enemy.attackDelays[0])
            {
                animator.SetTrigger("IsShooting");
                AudioManager.Instance.PlaySFX(lightningClip, 0.9f);
                var arrowInstance = Instantiate(arrowPrefab, hand.position, Quaternion.identity);
                arrowInstance.transform.LookAt(enemy.playerRef.transform.position);

                lightningBoltStrikesCurrent++;
                enemy.attackTimer = 0.2f;

                if (lightningBoltStrikesCurrent >= lightningBoltStrikes)
                {
                    enemyState = EnemyState.Aggro;
                    shooting = false;
                }
            }
        }

        if (clapping)
        {
            enemy.attackTimer += Time.deltaTime;

            if (enemy.attackTimer >= 2.4f)
            {
                clapParticles.Play();
                AudioManager.Instance.PlaySFX(clapClip, 1f);

                if (enemy.playerRef.isGrounded() || Vector3.Distance(clapParticles.transform.position, enemy.playerRef.transform.position) <= 3.0f)
                {
                    enemy.playerRef.IOnDamage(1);
                }

                clapping = false;
                clapReturnTimer = 0.0f;
                clapReturn = true;
            }
        }

        if (clapReturn)
        {
            clapReturnTimer += Time.deltaTime;
            if (clapReturnTimer >= 1.2f)
            {
                enemyState = EnemyState.Aggro;
                clapReturn = false;
                transform.position = startingPosition;
                AudioManager.Instance.PlaySFX(teleportClip, 0.8f);
                tpParticles_1.Play();
            }
        }
    }
}
