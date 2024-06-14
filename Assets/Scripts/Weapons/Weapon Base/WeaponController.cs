using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptTableObject weaponData;
    protected float currentCooldown;

    protected PlayerMovement pm;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Tham chiếu đến script PlayerMovement
        pm = FindObjectOfType<PlayerMovement>();
        currentCooldown = weaponData.CooldownDuration;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f)
        {
            Attack();
        }
    }
    // Cho phép ghi đè
    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration;
    }
}
