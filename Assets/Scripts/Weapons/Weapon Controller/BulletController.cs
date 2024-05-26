using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    
    // Ghi đè
    protected override void Attack()
    {
        // Gọi phương thức Attack từ lớp cha
        base.Attack();
        GameObject spawnedBullet = Instantiate(prefab);
        spawnedBullet.transform.position = transform.position;
        // Tham chiếu và thiết lập hướng
        spawnedBullet.GetComponent<BulletBehaviour>().DirectionChecker(pm.lastMoveVector);

    }
}
