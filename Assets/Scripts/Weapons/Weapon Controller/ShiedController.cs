using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiedController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedShied = Instantiate(weaponData.Prefab);
        spawnedShied.transform.position = transform.position;
        spawnedShied.transform.parent = transform;
    }
}
