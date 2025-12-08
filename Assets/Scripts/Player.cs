using System;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody rigidbody;
    [SerializeField] CollisionData collisionData;
    
    PlayerState playerState = new PlayerState();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        collisionData.onTriggerEnterEvents.Add(checkPerching);
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
}
