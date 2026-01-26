using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    protected Enemy enemy;
    public enum EnemyState
    {
        Idle,
        Aggro,
        Attack
    }

    [HideInInspector] public EnemyState enemyState;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        enemy.health = 1;
    }
    
    public virtual void updateBehaviour()
    {

    }
    
    public virtual void idle()
    {

    }

    public virtual void acquirePlayer()
    {

    }

    public virtual void attack()
    {

    }
}
