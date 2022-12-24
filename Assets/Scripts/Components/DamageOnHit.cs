using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public float damageDone;
    public Pawn owner;

    private Health otherHealth;

    // Built in function that detects when colliders of pawns are overlapping
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner.gameObject)
        {
            // Don't want to do self damage
            return;
        }

        if (other.gameObject.GetComponent<Health>())
        {
            // Gets health component from colliding objects
            otherHealth = other.gameObject.GetComponent<Health>();
        }

        // Deals damage to that object only if it has a health component
        if (otherHealth != null)
        {
            otherHealth.TakeDamage(damageDone, owner);
        }

        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}