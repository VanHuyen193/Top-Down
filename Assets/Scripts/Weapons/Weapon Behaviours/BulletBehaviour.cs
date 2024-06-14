using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ProjectileWeaponBehaviour
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rd = gameObject.GetComponent<Rigidbody2D>();
        rd.AddForce(transform.right * currentSpeed, ForceMode2D.Impulse);
    }
}
