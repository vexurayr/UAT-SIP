using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooter : MonoBehaviour
{
    // Leave 0 to not override damage set in projectile prefab
    public float overrideProjectileDamage;

    public GameObject projectile;
    public float fireForce;
    public float lifeSpan;

    protected float startDamage;

    public abstract void Shoot();
}