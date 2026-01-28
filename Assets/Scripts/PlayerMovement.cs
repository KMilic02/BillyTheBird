using System;
using UnityEngine;
using System.Collections;

public partial class Player
{
    [Header("Audio")]
    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip glideClip;
    
    [Header("Movement")]
    const float jumpForce = 7.0f;
    float movementSpeed => 25.0f + seeds * 0.15f;
    float stoppingSpeed => 30.0f + seeds * 0.2f;
    float maxMoveSpeed => 5.0f + seeds * 0.025f;
    const float maxGlidingFallSpeed = -2.0f;

    bool isGliding;

    public bool isRunning;
    public bool isRealyGliding;
    public bool isJumping;
    
    const float dashSpeed = 15.0f;
    const float dashDuration = 0.35f;
    const float bounceAmount = 3.0f;
    float dashTimer;
    Vector3 dashDirection;
    bool landedAfterDash = true;

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
        
        dashLoop();
        
        if (isGrounded() && playerState == State.Jumping)
            playerState.transitionTo(Signal.Land);
        
        if (!isGrounded() && playerState == State.Grounded)
            playerState.transitionTo(Signal.Jump);

        if (playerState != State.Dashing && isGrounded())
            landedAfterDash = true;

        if (playerState == State.Grounded)
            glideDurationLeft = maxGlideDuration;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }

        glide();

        if (canDash && Input.GetKeyDown(KeyCode.LeftShift))
        {
            dash();
        }
    }

    void dashLoop()
    {
        if (playerState.getState() is State.Dashing)
        {
            rigidbody.linearVelocity = dashDirection * dashSpeed;
            
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0.0f)
            {
                endDash();
            }
        }
    }
    
    void move()
    {
        if (playerState.getState() is State.Perched or State.Dashing)
            return;

        isRunning = true;    

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
                turnRate *= 0.25f;
            }
            
            var skewedVector = Vector3.RotateTowards(currentVelocity.normalized, direction, 2.0f * Time.deltaTime * turnRate, 0.0f);
            skewedVector *= currentVelocity.magnitude;
            skewedVector.y = rigidbody.linearVelocity.y;
            
            rigidbody.linearVelocity = skewedVector;
            currentVelocity = rigidbody.linearVelocity;
        }
        
        if (currentVelocity.magnitude <= maxMoveSpeed)
        {
            rigidbody.AddForce(Time.deltaTime * (isGrounded() ? movementSpeed : movementSpeed * 0.25f) * direction, ForceMode.VelocityChange);
        }
    }

    void stop()
    {
        if (playerState.getState() is State.Dashing)
            return;

        isRunning = false;    
        
        var velocity = rigidbody.linearVelocity;
        var yVelocityStored = velocity.y;
        
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, 
            (!isGrounded() ? stoppingSpeed * 0.25f : stoppingSpeed) * Time.deltaTime);
        velocity.y = yVelocityStored;
        
        rigidbody.linearVelocity = velocity;
    }

    void jump()
    {
        if (playerState == State.Grounded || playerState == State.Perched)
        {
            isJumping = true;
            rigidbody.AddForce((playerState == State.Perched ? 1.33f : 1.0f) * jumpForce * Vector3.up, ForceMode.Impulse);
            playerState.transitionTo(Signal.Jump);
            AudioManager.Instance.PlaySFX(jumpClip, 0.9f);
            StartCoroutine(ResetJump());
        }
    }

    IEnumerator ResetJump() {

        yield return new WaitForSeconds(0.5f);

        isJumping = false;
    }

    void setIsGliding()
    {
        isGliding = Input.GetKey(KeyCode.Space) && playerState == State.Jumping && rigidbody.linearVelocity.y < 0.0f; 
    }

    void glide()
    {
        isRealyGliding = false;

        if (isGliding)
        {
            if (rigidbody.linearVelocity.y < maxGlidingFallSpeed && glideDurationLeft > 0.0f)
            {
                isRealyGliding = true;
                var velocity = rigidbody.linearVelocity;
                //velocity.y = Mathf.MoveTowards(velocity.y, -maxGlidingFallSpeed, Time.deltaTime * 100.0f);
                velocity.y = maxGlidingFallSpeed;
                rigidbody.linearVelocity = velocity;
                glideDurationLeft -=  Time.deltaTime;
            }
        }
    }

    public bool isGrounded()
    {
        var groundCheckPosition = playerCollider.bounds.center - Vector3.up * playerCollider.bounds.extents.y;
        var groundCheckExtents = playerCollider.bounds.extents.x * 0.5f;

        //Todo ovo nije lose testirat -> groundCheckExtents * 1.0f mozd daju true kao half extents, to znaci da ak se dodiruje zid u zraku da ce se moc skakat
        bool boxCast = Physics.CheckBox(groundCheckPosition, 
            new Vector3(groundCheckExtents * 1.0f, 0.02f, groundCheckExtents * 1.0f), 
            Quaternion.Euler(0.0f, transform.rotation.y, 0.0f),
            LayerMask.GetMask("Ground"));
        
        return collisionData.collidersAllTouching.Count > 0 && boxCast;
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

    void checkDashEndOnCollision(Collision collision)
    {
        if (playerState != State.Dashing)
            return;

        if (!collision.gameObject.TryGetComponent<ICollectible>(out _))
        {
            endDash();
        }
    }

    void dash()
    {
        if (!playerState.transitionTo(Signal.Dash))
            return;
        AudioManager.Instance.PlaySFX(dashClip, 0.85f);
        rigidbody.useGravity = false;
        dashTimer = dashDuration;
        dashDirection = mainCamera.transform.forward;
    }

    void endDash()
    {
        playerState.transitionTo(Signal.EndDash);
        rigidbody.useGravity = true;
        dashTimer = 0.0f;
        rigidbody.linearVelocity /= 1.5f;
        landedAfterDash = false;
    }
}
