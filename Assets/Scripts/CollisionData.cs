using System;
using UnityEngine;
using System.Collections.Generic;

public class CollisionData : MonoBehaviour
{
    public List<Collider> collidersAllTouching = new();
    public List<Action<Collision>> onEnterEvents = new();
    public List<Action<Collider>> onTriggerEnterEvents = new();
    public List<Action<Collision>> onExitEvents = new();
    public List<Action<Collider>> onTriggerExitEvents = new();
    
    void OnCollisionEnter(Collision collision)
    {
        collidersAllTouching.Add(collision.collider);
        
        onEnterEvents.ForEach(x => x(collision));
    }

    void OnCollisionExit(Collision collision)
    {
        collidersAllTouching.Remove(collision.collider);
        
        onExitEvents.ForEach(x => x(collision));
    }

    void OnTriggerEnter(Collider collision)
    {
        onTriggerEnterEvents.ForEach(x => x(collision));
    }

    void OnTriggerExit(Collider collision)
    {
        onTriggerExitEvents.ForEach(x => x(collision));
    }
}