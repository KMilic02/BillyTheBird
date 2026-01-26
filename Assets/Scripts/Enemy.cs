using UnityEngine;

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = GameObject.FindFirstObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
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
        gameObject.SetActive(false);
    }
}
