using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemys/Enemy")]
public class EnemyData : ScriptableObject
{
    public GameObject projectile;

    public bool useProjectile;

    public float health;
    public float movementSpeed;
    public float damage;
    public float seeDistance;
    public float attackRange;
    public float projectileSpeed;
    public float fireRate;
    public int lookDirections;
}
