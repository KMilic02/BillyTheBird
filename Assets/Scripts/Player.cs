using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player : MonoBehaviour, IDamageable
{
    [Header("Player")]
    public Rigidbody rigidbody;
    [SerializeField] CollisionData collisionData;
    
    PlayerState playerState = new PlayerState();
    Collider playerCollider;
    AudioSource glideSource;

    bool dead;

    float invincibilityTimer = 0.0f;
    
    void Start()
    {
        glideSource = GetComponent<AudioSource>();
        glideSource.clip = glideClip;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        collisionData.onTriggerEnterEvents.Add(checkPerching);
        collisionData.onTriggerEnterEvents.Add(getCollectible);
        collisionData.onTriggerEnterEvents.Add(deathPlane);
        collisionData.onEnterEvents.Add(checkDashEndOnCollision);
        
        collisionData.onEnterEvents.Add(checkEnemyCollision);
        collisionData.onTriggerEnterEvents.Add(checkProjectileCollision);
        
        playerCollider = GetComponent<Collider>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        health = 4 - GameManager.difficulty;
    }

    void Update()
    {
        handleMovement();
        handleCameraRotation();

        UpdateAnimatorStates();
        
        invincibilityTimer -= Time.deltaTime;
        
        AudioManager.Instance.PlayAudioSource(glideSource, isRealyGliding);
        
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

    void checkEnemyCollision(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        var contactNormal = collision.GetContact(0).normal;
        Debug.Log(contactNormal);

        if (Vector3.Dot(contactNormal, Vector3.up) >= 0.5f)
        {
            var velocity = rigidbody.linearVelocity;
            velocity.y = bounceAmount;
            rigidbody.linearVelocity = velocity;
            enemy.IOnDamage(1);
        }
        /*else
        {
            IOnDamage(1);
            if (invincibilityTimer > 0.0f) //extra check za bounce, glupo al radi
                rigidbody.linearVelocity = contactNormal * bounceAmount;
        }*/
    }

    public void nextLevel()
    {
        StartCoroutine(GameManager.Instance.FadeOut(
            () => GameManager.Instance.loadScene(
                GameManager.sceneList[GameManager.sceneList.IndexOf(SceneManager.GetActiveScene().name) + 1]
            )
        ));
    }
    
    void checkProjectileCollision(Collider collision)
    {
        if (!collision.gameObject.TryGetComponent<ProjectileTag>(out _))
        {
            return;
        }
        
        IOnDamage(1);
    }
    
    void deathPlane(Collider collision)
    {
        if (collision.CompareTag("DeathPlane"))
            IOnDeath();
    }
    
    public int health { get; set; }

    public void IOnDamage(int damage)
    {
        if (invincibilityTimer > 0.0f)
            return;

        StartCoroutine(flash());
        
        invincibilityTimer = 1.0f;
        health -= damage;

        if (health <= 0)
            IOnDeath();
    }

    IEnumerator flash()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void IOnDeath()
    {
        if (dead)
            return;
        dead = true;
        GameManager.seeds -= seedsCollectedInScene;
        GameManager.feathers -= feathersCollectedInScene;
        
        transform.Rotate(transform.right, 180,0f);
        StartCoroutine(GameManager.Instance.FadeOut(() =>
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.startMusic);
            GameManager.Instance.loadScene(SceneManager.GetActiveScene().name);
        }));
        enabled = false;
    }
}
