using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public Projectile projectilePrefab; // If attached, a projectile will spawn every time the weapon cooldown
        public Aura auraPrefab; // If attached, an aura will spawn when weapon is equipped
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s1.name ?? s2.name; // Use the name from s1 if it's not null, otherwise use the name from s2
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.projectilePrefab = s2.projectilePrefab == null ? s1.projectilePrefab : s2.projectilePrefab; // Use the projectilePrefab from s1 if it's not null, otherwise use the name from s2
            result.auraPrefab = s2.auraPrefab == null ? s1.auraPrefab : s2.auraPrefab; // Use the auraPrefab from s1 if it's not null, otherwise use the name from s2
            result.spawnVariance = s2.spawnVariance;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            return result;
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected Stats currentStats;
    public WeaponData data;
    protected float currentCooldown;
    protected PlayerMovement movement;

    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        ActivateCooldown();
    }

    protected virtual void Awake()
    {
        if (data)
        {
            currentStats = data.baseStats;
        }
    }

    protected virtual void Start()
    {
        if (data)
        {
            Initialise(data);
        }
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f)
        {
            Attack(currentStats.number + owner.Stats.amount);
        }
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to {1}", data.maxLevel, currentLevel));
            return false;
        }

        currentStats += data.GetLevelData(++currentLevel);
        return true;
    }

    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }

    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.Stats.might;
    }

    public virtual float GetArea()
    {
        return currentStats.area + owner.Stats.area;
    }

    public virtual Stats GetStats()
    {
        return currentStats;
    }

    public virtual bool ActivateCooldown(bool strict = false)
    {
        // When <strick> is enabled and the cooldown is not yet finished,
        // do not refresh the cooldown.
        if (strict && currentCooldown > 0) return false;

        // Calculate what the cooldown is going to be, factoring  in the cooldown
        // reduction stat in the player character.
        float actualCooldown = currentStats.cooldown * Owner.Stats.cooldown;

        // Limit the maximum cooldown to the actual cooldown, so we cannot increase
        // the cooldown above the cooldown stat if we accidentally call this function
        // multiple times.
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }
}
