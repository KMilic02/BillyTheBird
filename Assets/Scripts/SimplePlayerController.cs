using System;
using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    [Header("References")]
    public new Rigidbody rigidbody;
    public Camera mainCamera;
    public CollisionData collisionData;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 50.0f;
    [SerializeField] float maxMoveSpeed = 10.0f;
    [SerializeField] float stoppingSpeed = 30.0f;
    [SerializeField] float airControlMultiplier = 0.25f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 10.0f;
    [SerializeField] float perchJumpMultiplier = 1.33f;

    [Header("Gliding")]
    [SerializeField] float maxGlidingFallSpeed = -2.0f;

    [Header("Camera")]
    [SerializeField] float cameraSensitivity = 0.3f;
    [SerializeField] float cameraDistance = 4.0f;

    PlayerState playerState = new PlayerState();
    Vector2 cameraRotation = new Vector2(0, 0);
    bool isGliding;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (collisionData != null)
        {
            collisionData.onTriggerEnterEvents.Add(checkPerching);
            collisionData.onTriggerEnterEvents.Add(getCollectible);
        }
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

    void handleMovement()
    {
        setIsGliding();

        if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
        {
            move();
        }
        else
        {
            stop();
        }

        if (isGrounded() && playerState == State.Jumping)
        {
            playerState.transitionTo(Signal.Land);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }

        glide();
    }

    void move()
    {
        if (playerState == State.Perched)
            return;

        var forwardAmount = Input.GetAxis("Vertical") * transform.forward;
        var sidewaysAmount = Input.GetAxis("Horizontal") * transform.right;

        var direction = (forwardAmount + sidewaysAmount).normalized;

        var currentVelocity = rigidbody.linearVelocity;
        currentVelocity.y = 0.0f;

        if (Vector3.Dot(currentVelocity.normalized, direction) <= 0.99f)
        {
            var angle = Vector3.Angle(currentVelocity.normalized, direction);

            var turnRate = 1 + angle / 180.0f;
            if (Math.Abs(rigidbody.linearVelocity.y) >= 0.001f)
            {
                turnRate *= airControlMultiplier;
            }

            var skewedVector = Vector3.RotateTowards(currentVelocity.normalized, direction, 2.0f * Time.deltaTime * turnRate, 0.0f);
            skewedVector *= currentVelocity.magnitude;
            skewedVector.y = rigidbody.linearVelocity.y;

            rigidbody.linearVelocity = skewedVector;
            currentVelocity = rigidbody.linearVelocity;
        }

        if (currentVelocity.magnitude <= maxMoveSpeed)
        {
            float speedMultiplier = isGrounded() ? 1.0f : airControlMultiplier;
            rigidbody.AddForce(Time.deltaTime * movementSpeed * speedMultiplier * direction, ForceMode.VelocityChange);
        }
    }

    void stop()
    {
        var velocity = rigidbody.linearVelocity;
        var yVelocityStored = velocity.y;

        float stopMultiplier = isGrounded() ? 1.0f : airControlMultiplier;
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, stoppingSpeed * stopMultiplier * Time.deltaTime);
        velocity.y = yVelocityStored;

        rigidbody.linearVelocity = velocity;
    }

    void jump()
    {
        if (playerState == State.Grounded || playerState == State.Perched)
        {
            float multiplier = playerState == State.Perched ? perchJumpMultiplier : 1.0f;
            rigidbody.AddForce(multiplier * jumpForce * Vector3.up, ForceMode.Impulse);
            playerState.transitionTo(Signal.Jump);
        }
    }

    void setIsGliding()
    {
        isGliding = Input.GetKey(KeyCode.Space) && playerState == State.Jumping && rigidbody.linearVelocity.y < 0.0f;
    }

    void glide()
    {
        if (isGliding)
        {
            if (rigidbody.linearVelocity.y < maxGlidingFallSpeed)
            {
                var velocity = rigidbody.linearVelocity;
                velocity.y = maxGlidingFallSpeed;
                rigidbody.linearVelocity = velocity;
            }
        }
    }

    bool isGrounded()
    {
        return collisionData != null
               && collisionData.collidersAllTouching.Count > 0;
    }

    void checkPerching(Collider collision)
    {
        if (playerState != State.Jumping)
            return;

        if (collision.gameObject.TryGetComponent<Perchable>(out var perchable))
        {
            var perchTransform = perchable.transform;
            rigidbody.linearVelocity = Vector3.zero;
            transform.position = perchTransform.position + Vector3.up * 0.5f;
            playerState.transitionTo(Signal.Perch);
        }
    }

    void getCollectible(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.IOnCollect(null);
        }
    }

    void handleCameraRotation()
    {
        if (mainCamera == null) return;

        var mouseMovement = Input.mousePositionDelta;
        cameraRotation.x -= mouseMovement.y * cameraSensitivity;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);
        cameraRotation.y += mouseMovement.x * cameraSensitivity;

        mainCamera.transform.rotation = Quaternion.Euler(cameraRotation);

        var rotation = transform.rotation.eulerAngles;
        rotation.y = cameraRotation.y;
        transform.rotation = Quaternion.Euler(rotation);
    }

    void updateCameraPosition()
    {
        if (mainCamera == null) return;

        mainCamera.transform.position = transform.position - mainCamera.transform.forward * cameraDistance;
    }
}
