using System;
using UnityEngine;

public partial class Player : MonoBehaviour, IDamageable
{
    [Header("Player")]
    public Rigidbody rigidbody;
    [SerializeField] CollisionData collisionData;
    
    PlayerState playerState = new PlayerState();

    int seeds;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        collisionData.onTriggerEnterEvents.Add(checkPerching);
        collisionData.onTriggerEnterEvents.Add(getCollectible);
    }

    void Update()
    {
        handleMovement();
        handleCameraRotation();
    }

    void LateUpdate()
    {
        updateCameraPosition();
    }

    void getCollectible(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.IOnCollect(this);
        }
    }

    public void addSeeds(int amount)
    {
        seeds += amount;
        updateUI();
    }
    
    public int health { get; set; }

    public void IOnDamage(int damage)
    {   
        health -= damage;

        if (health <= 0)
            IOnDeath();
    }

    public void IOnDeath()
    {
        
    }
}
