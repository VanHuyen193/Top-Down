using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehaviour : MeleeWeaponBehaviour
{
    List<GameObject> markedEnemies;
    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && !markedEnemies.Contains(collision.gameObject))
        {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);

            markedEnemies.Add(collision.gameObject);
        }
        else if (collision.CompareTag("Props") && !markedEnemies.Contains(collision.gameObject))
        {
            if (collision.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(currentDamage);
                markedEnemies.Add(collision.gameObject);
            }
        }
    }
}
