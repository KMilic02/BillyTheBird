using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] EnemyBehaviour enemyBehaviour;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
