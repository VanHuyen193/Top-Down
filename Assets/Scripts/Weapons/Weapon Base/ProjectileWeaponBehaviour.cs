using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptTableObject weaponData;
    protected Vector3 direction;
    public float destroyAfterSeconds;

    // Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentPierce = weaponData.Pierce;
        currentCooldownDuration = weaponData.CooldownDuration;
    }

    protected virtual void Start()
    { 
        Destroy(gameObject, destroyAfterSeconds);
    }
    // Thiết lập hướng của vũ khí
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if(dirx < 0 && diry == 0) //left
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry < 0) //down
        {
            scale.y = scale.y * -1;
            rotation.z = -90f;
        }
        else if (dirx == 0 && diry > 0) // up
        {
            rotation.z = 90f;
        }
        else if (dirx > 0 && diry > 0) // right up
        {
            rotation.z = 45f;
        }
        else if (dirx > 0 && diry < 0) // right down
        {
            rotation.z = -45f;
        }
        else if (dirx < 0 && diry > 0) // left up
        {
            scale.x = scale.x * -1;
            rotation.z = -45f;
        }
        else if (dirx < 0 && diry < 0) // left down
        {
            scale.x = scale.x * -1;
            rotation.z = 45f;
        }
        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);
            ReducePierce();
        }
        else if (collision.CompareTag("Props"))
        {
            if(collision.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(currentDamage);
                ReducePierce();
            }
        }
    }


    // Xóa vũ khí khi pierce giảm về 0
    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
