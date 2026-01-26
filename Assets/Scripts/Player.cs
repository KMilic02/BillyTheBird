using System;
using UnityEngine;

public partial class Player : MonoBehaviour, IDamageable
{
    [Header("Player")]
    public Rigidbody rigidbody;
    [SerializeField] CollisionData collisionData;
    
    PlayerState playerState = new PlayerState();
    Collider playerCollider;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        collisionData.onTriggerEnterEvents.Add(checkPerching);
        collisionData.onTriggerEnterEvents.Add(getCollectible);
        collisionData.onEnterEvents.Add(checkDashEndOnCollision);
        
        playerCollider = GetComponent<Collider>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        handleMovement();
        handleCameraRotation();
        
        #if UNITY_EDITOR
        debugUpdate();
        #endif
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
