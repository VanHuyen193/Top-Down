using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount; // Number of times this attack will happen

    protected override void Update()
    {
        base.Update();

        // Otherwise, if the attack interval goes from obove 0 to below, we also call attack
        if(currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if(currentAttackInterval <= 0)
            {
                Attack(currentAttackCount);
            }
        }
    }

    public override bool CanAttack()
    {
        if(currentAttackCount > 0)
        {
            return true;
        }
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            ActivateCooldown(true);
            return false;
        }
        if(!CanAttack())
        {
            return false;
        }
        float spawnAngle = GetSpawnAngle();

        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)(GetSpawnOffset(spawnAngle)),
            Quaternion.Euler(0, 0, spawnAngle)
        );
        prefab.weapon = this;
        prefab.owner = owner;
        owner.PlaySound(prefab.shootSound); // Play sound

        ActivateCooldown(true);
        
        attackCount--;
        if(attackCount > 0) 
        { 
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMoveVector.y, movement.lastMoveVector.x) * Mathf.Rad2Deg;
    }
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
