using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ChampionBehaviour : EnemyBehaviour
{
    public AudioClip attackClip;
    public AudioClip hitClip;

    const float maxAggroRange = 20.0f;
    Vector3 startLocation;
    Vector3 currentRandomLocation;
    NavMeshAgent agent;
    Animator animator;
    const float attackRange = 1.5f;

    const float chargeDelay = 1.0f;
    const float chargeDuration = 0.6f;
    const float chargeSpeed = 13.0f;
    const float defaultAccel = 8.0f;
    const float defaultSpeed = 3.2f;

    float chargeTimer = 0.0f;
    float chargeDelayTimer = 0.0f;

    const float chargeCdMin = 5.0f;
    const float chargeCdMax = 7.0f;
    float chargeCd = 10.0f;
    bool charging;
    
    //health 3, 5, 8
    
    public void Start()
    {
        enemy = GetComponent<Enemy>();
        enemy.health = GameManager.difficulty switch
        {
            1 => 3,
            2 => 5,
            _ => 8
        };
        startLocation = transform.position;
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
        var playerPosition = enemy.playerRef.transform.position;

        if (Vector3.Distance(playerPosition, transform.position) <= enemy.acquisitionRange)
        {
            enemyState = EnemyState.Aggro;
        }
    }

    public override void acquirePlayer()
    {
        if (charging)
        {
            charge();
            return;
        }

        chargeCd -= Time.deltaTime;
        var playerPosition = enemy.playerRef.transform.position;
        
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

        if (chargeCd <= 0.0f)
        {
            chargeDelayTimer = chargeDelay;
            chargeTimer = chargeDuration;
            charging = true;
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

    void charge()
    {
        if (chargeDelayTimer >= 0.0f)
        {
            agent.angularSpeed = 0.0f;
            agent.SetDestination(transform.position);
            agent.speed = 0.0f;
            agent.acceleration = 10000.0f;
            chargeDelayTimer -= Time.deltaTime;
            return;
        }

        if (chargeTimer >= 0.0f)
        {
            agent.speed = chargeSpeed;
            agent.Move(transform.forward * Time.deltaTime * chargeSpeed);
            chargeTimer -= Time.deltaTime;
            return;
        }

        agent.angularSpeed = 120.0f;
        agent.SetDestination(transform.position);
        agent.speed = defaultSpeed;
        chargeCd = Random.Range(chargeCdMin, chargeCdMax);
        charging = false;
    }

    public void AttackHit()
    {
        float hitRadius = 0.8f;
        Vector3 hitCenter = transform.position + transform.forward * 1.0f;

        Collider[] hits = Physics.OverlapSphere(hitCenter, hitRadius);

        foreach (Collider hit in hits)
        {
            AudioManager.Instance.PlaySFX(hitClip, 1f);
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<Player>().IOnDamage(1);
            }
        }

        Debug.Log("HIT FRAME");
    }

}
