using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Component that you attach to all projectile prefab. All spawned projectiles will fly in the direction
/// they are facing and deal damage when they hit an object.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource
    {
        projectile,
        owner
    };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0,0,0);

    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if(rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed * weapon.Owner.Stats.speed;
        }

        // Prevent the area from beion 0, as it hides the projectile
        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        // Set how much piercing this object has.
        piercing = stats.piercing;
        if(stats.lifespan > 0)
        {
            // Destroy the projectile after its lifespan expires
            Destroy(gameObject, stats.lifespan);
        }
        // If the projectile is auto-aiming, automatically find a suitable enemy
        if (hasAutoAim)
        {
            AcquireAutoAimFacing();
        }
    }

    //If the projectile is homing, it will automatically find a suitable target to move
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; // We need to determine where to aim.

        // Find all enemmies on the screen.
        EnemyStats[] enemies = FindObjectsOfType<EnemyStats>();

        // Find all breakable props in the screen
        BreakableProps[] breakableProps = FindObjectsOfType<BreakableProps>();

        // Create a list to hold both types
        List<Transform> targets = new List<Transform>();

        // Add all enemies' transforms to the list
        foreach (var enemy in enemies)
        {
            targets.Add(enemy.transform);
        }

        // Add all breakable props' transforms to the list
        foreach (var prop in breakableProps)
        {
            targets.Add(prop.transform);
        }

        // Select a closest target (if there is at least 1)
        // Otherwise, pick a random angle
        if (targets.Count > 0)
        {
            Transform closestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (Transform potentialTarget in targets)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = potentialTarget;
                }
            }

            Vector2 difference = closestTarget.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // Point the projectile towards wher we are aiming at.
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    protected virtual void FixedUpdate()
    {
        // Only drive movement ourselves if this is kinematic
        if(rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime * weapon.Owner.Stats.speed;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        // Only collide with enemies or breakable stuff
        if (es)
        {
            // If there os an owner and the damage source is set to owner
            // we will caculate knockback using the owner instead of the projectile
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            
            // Deals damage and destroys the projectile
            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        // Destroy this object if it has run out of health from hitting other stuff
        if(piercing <= 0)
        {
            Destroy(gameObject);
        }
    }
}
