using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon")]
public class WeaponData : ScriptableObject
{

    public int weaponId;

    public Sprite weaponSprite;
    public GameObject weapon;

    public bool useProjectile;
    public bool followCursor;
    public bool animation;

    public GameObject projectile;

    public float fireRate;
    public float damage;
    public float speed;
    public float destroyTime;
    public int ammo;

    public float throwSpeed;
    public float throwDestroyTime;

    public Vector2 offset;
    public Vector2 launchPoint;
}
