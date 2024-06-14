using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : WeaponController
{
    Vector3 mousePos;
    Vector3 lookDir;
    float shortestDistance = Mathf.Infinity;
    GameObject nearestTarget = null;

    protected override void Update()
    {
        base.Update();
    }

    // Ghi đè
    protected override void Attack()
    {
        // Gọi phương thức Attack từ lớp cha
        base.Attack();

        FindNearestTarget();

        if (nearestTarget != null)
        {
            lookDir = (nearestTarget.transform.position - transform.position).normalized;

            GameObject spawnedBullet = Instantiate(weaponData.Prefab);
            spawnedBullet.transform.position = transform.position;
            spawnedBullet.GetComponent<BulletBehaviour>().DirectionChecker(lookDir);
        }
        //else
        //{
        //    Debug.Log("No target found within range.");
        //}
    }

    void FindNearestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        BreakableProps[] breakableProps = Object.FindObjectsOfType<BreakableProps>();

        Vector3 currentPosition = transform.position;
        shortestDistance = Mathf.Infinity; // Reset khoảng cách nhỏ nhất qua mỗi lần search
        nearestTarget = null; 

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestTarget = enemy;
                }
            }
        }

        foreach (BreakableProps prop in breakableProps)
        {
            if (prop != null)
            {
                float distanceToProp = Vector3.Distance(currentPosition, prop.transform.position);
                if (distanceToProp < shortestDistance)
                {
                    shortestDistance = distanceToProp;
                    nearestTarget = prop.gameObject;
                }
            }
        }
    }
}
