using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] EnemyBehaviour enemyBehaviour;

    [HideInInspector] public float cooldownTimer;
    [HideInInspector] public float attackTimer;
    
    public float attackCooldown;
    public float movementSpeed;
    public int attackCount;
    public float[] attackRanges;
    public float[] attackDelays;
    public float acquisitionRange;
    public float aggroExtraAcquisitionRange;

    [HideInInspector] public Player playerRef;

    void Start()
    {
        playerRef = GameObject.FindFirstObjectByType<Player>();
    }

    void Update()
    {
        // Safety check: ensure playerRef is still valid
        if (playerRef == null || playerRef.gameObject == null)
        {
            playerRef = GameObject.FindFirstObjectByType<Player>();
            if (playerRef == null) return; // No player found, skip this frame
        }

        enemyBehaviour.updateBehaviour();
    }
    
    public int health { get; set; }

    public void IOnDamage(int damage)
    {
        health -= damage;
        
        if  (health <= 0)
            IOnDeath();
    }

    public void IOnDeath()
    {
        if (TryGetComponent<BossTag>(out var _))
        {
            playerRef.nextLevel();
        }

        // Stop all coroutines and disable immediately
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        // Clear reference when destroyed
        playerRef = null;
    }
}
