using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ProjectileWeaponBehaviour
{
    BulletController bc;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        bc = FindObjectOfType<BulletController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * bc.speed * Time.deltaTime;
    }
}
