using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierShooter : Shooter
{
    public float damageDecrease;
    public Camera pOV;

    // Transform where projectile will spawn
    public Transform firepointTransform;

    // Transform that bullet spawn location rotates around
    public Transform firepointPivot;
    public float circleSize;

    private Vector3 selfToMouseVector;
    private float selfToMouseAngle;
    private float newDamage;

    public void Start()
    {
        startDamage = overrideProjectileDamage;
    }

    public void Update()
    {
        RotateBulletSpawn();
    }

    // Keeps bullet spawn location between player character and mouse cursor
    public void RotateBulletSpawn()
    {
        // Turns screen pixels into world coordinates
        selfToMouseVector = Input.mousePosition;
        selfToMouseVector.z = firepointPivot.transform.position.z - pOV.transform.position.z;
        selfToMouseVector = pOV.ScreenToWorldPoint(selfToMouseVector);

        // Makes sure spawn location stays in line with mouse
        selfToMouseVector = selfToMouseVector - firepointPivot.transform.position;

        // Complicated trigonometry
        selfToMouseAngle = Mathf.Atan2(selfToMouseVector.y, selfToMouseVector.x) * Mathf.Rad2Deg;

        if (selfToMouseAngle < 0.0f)
        {
            selfToMouseAngle += 360.0f;
        }

        // Rotates bullet spawn relative to the object it's attached to
        firepointTransform.transform.localEulerAngles = new Vector3(0, 0, selfToMouseAngle);

        // Bullet spawn will always be CircleSize distance away from the bullet spawn pivot
        float xPos = Mathf.Cos(Mathf.Deg2Rad * selfToMouseAngle) * circleSize;
        float yPos = Mathf.Sin(Mathf.Deg2Rad * selfToMouseAngle) * circleSize;
        firepointTransform.localPosition = new Vector3(xPos, yPos, 0);
    }

    // Creates a new shell object, saves the damage it'll do and which object fired it, applies a force to the rigidbody to make it fly
    // and destroys it after a set period of time
    public override void Shoot()
    {
        GameObject newShell = Instantiate(projectile, firepointTransform.position, firepointTransform.rotation);
        DamageOnHit doh = newShell.GetComponent<DamageOnHit>();

        // If damage on hit component exists, set its damage and owner to the data saved in the pawn
        if (doh != null)
        {
            if (overrideProjectileDamage > 0)
            {
                doh.damageDone = overrideProjectileDamage;
            }
            
            doh.owner = GetComponent<Pawn>();
        }

        Rigidbody2D rb = newShell.GetComponent<Rigidbody2D>();

        // If rigidbody exists, apply force in that direction with that much power
        if (rb != null)
        {
            rb.AddForce(firepointTransform.right * fireForce);
        }

        // If the object exists after 2nd variable in seconds, destroy object
        Destroy(newShell, lifeSpan);
    }

    public void ChangeDamage(float colorControllerOutput)
    {
        newDamage = damageDecrease * colorControllerOutput;

        overrideProjectileDamage = startDamage - newDamage;
    }
}