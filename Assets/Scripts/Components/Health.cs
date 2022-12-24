using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float amount, Pawn source)
    {
        currentHealth = currentHealth - amount;

        // Makes sure current health never goes below 0 or higher than max health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(source.name + " did " + amount + " damage to " + gameObject.name);

        if (currentHealth <= 0)
        {
            Die(source);
        }
    }

    public void Die(Pawn source)
    {
        Destroy(gameObject);
    }
}