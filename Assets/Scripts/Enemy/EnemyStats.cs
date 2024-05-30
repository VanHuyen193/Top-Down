using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    // Current stats
    float currentMoveSpeed;
    float currentHealth;
    float currentDamage;

    // Gọi khi game object mà script gắn vào được khởi tạo (gọi trước Start)
    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentDamage = enemyData.Damage;
        currentHealth = enemyData.MaxHealth;
    }

    public void TakeDamage(float dmg) 
    {
        currentHealth -= dmg;
        if (currentHealth <= 0) 
        {
            Kill();
        }
    }
    public void Kill()
    {
        Destroy(gameObject);
    }
}
