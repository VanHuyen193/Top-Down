using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "weaponScriptableObject", menuName = "ScriptableObject/Weapon")]
public class WeaponScriptTableObject : ScriptableObject
{
    [SerializeField]
    GameObject prefab;  // Để spawn ra vũ khí
    public GameObject Prefab { get => prefab; private set => prefab = value; } // Chỉ cho phép đọc

    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }

    [SerializeField]
    int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }
}
