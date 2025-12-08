using System;
using UnityEngine;

public class BasicControlsPrototype : MonoBehaviour
{
    public Rigidbody rigidbody;

    const float jumpForce = 7.0f;
    const float movementSpeed = 25.0f;
    const float stoppingSpeed = 30.0f;
    const float maxMoveSpeed = 5.0f;
    const float cameraSensitivity = 0.3f;
    
    Vector2 cameraRotation = new Vector2(0, 0);

    void Start()
    {

    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) 
        {
            move();           
        }
        else
        {
            stop();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }

    }

    void LateUpdate()
    {
        //Camera.main.transform.rotation = transform.rotation;
    }



    void move()
    {
        var currentVelocity = rigidbody.linearVelocity;
        currentVelocity.y = 0.0f;

        if (Vector3.Dot(currentVelocity.normalized, transform.forward) <= 0.99f)
        {
            var angle = Vector3.Angle(currentVelocity.normalized, transform.forward);
            
            var turnRate = 1 + angle / 180.0f;
            if (rigidbody.linearVelocity.y >= 0.001f)
            {
                turnRate *= 0.25f;
            }
            
            var skewedVector = Vector3.RotateTowards(currentVelocity.normalized, transform.forward, 2.0f * Time.deltaTime * turnRate, 0.0f);
            skewedVector *= currentVelocity.magnitude;
            skewedVector.y = rigidbody.linearVelocity.y;
            
            rigidbody.linearVelocity = skewedVector;
            currentVelocity = rigidbody.linearVelocity;
        }
        
        if (currentVelocity.magnitude <= maxMoveSpeed)
        {
            rigidbody.AddForce(Time.deltaTime * movementSpeed * transform.forward, ForceMode.VelocityChange);
        }
    }

    void stop()
    {
        var velocity = rigidbody.linearVelocity;
        var yVelocityStored = velocity.y;
        
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, stoppingSpeed * Time.deltaTime);
        velocity.y = yVelocityStored;
        
        rigidbody.linearVelocity = velocity;
    }

    void jump()
    {
        if (rigidbody.linearVelocity.y <= 0.001f)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
