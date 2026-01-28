using UnityEngine;

public class BossTag : MonoBehaviour
{
    public static BossTag activeBoss { get; private set; }

    private Enemy enemyComponent;
    private EnemyBehaviour enemyBehaviour;
    private int maxHealth;
    private bool maxHealthInitialized = false;
    private bool wasActive = false;

    void Awake()
    {
        // Clear static reference on new scene instance
        if (activeBoss != null && activeBoss != this)
        {
            activeBoss = null;
        }
    }

    void Start()
    {
        enemyComponent = GetComponent<Enemy>();
        enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

    void Update()
    {
        // Boss is active when it's in Aggro or Attack state
        bool isActive = enemyBehaviour != null &&
                       (enemyBehaviour.enemyState == EnemyBehaviour.EnemyState.Aggro ||
                        enemyBehaviour.enemyState == EnemyBehaviour.EnemyState.Attack);

        // Boss just became active
        if (isActive && !wasActive)
        {
            activeBoss = this;
            Debug.Log($"[BossTag] Boss {gameObject.name} is now ACTIVE (aggro state)");
        }
        // Boss just became inactive
        else if (!isActive && wasActive)
        {
            if (activeBoss == this)
                activeBoss = null;
            Debug.Log($"[BossTag] Boss {gameObject.name} is now INACTIVE");
        }

        wasActive = isActive;
    }

    void OnDisable()
    {
        if (activeBoss == this)
            activeBoss = null;
    }

    void OnDestroy()
    {
        // Ensure cleanup when destroyed
        if (activeBoss == this)
            activeBoss = null;
    }

    public int GetHealth()
    {
        return enemyComponent != null ? enemyComponent.health : 0;
    }

    public int GetMaxHealth()
    {
        if (!maxHealthInitialized && enemyComponent != null)
        {
            maxHealth = enemyComponent.health;
            maxHealthInitialized = true;
            Debug.Log($"[BossTag] Max health initialized to {maxHealth} for {gameObject.name}");
        }

        return maxHealth;
    }

    public string GetBossName()
    {
        return gameObject.name.ToUpper();
    }
}
