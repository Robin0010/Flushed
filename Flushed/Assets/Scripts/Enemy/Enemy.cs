using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;

    [SerializeField] LayerMask enemyMask;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] Transform meleeHitPoint;

    private float health;

    private float projectileTimer;
    private bool canAttack;
    private bool isFacingRight = true;

    [Header("Patrol Settings")]
    public bool mustPatrol;
    public Rigidbody2D rb;
    private bool mustTurn;
    public Transform groundCheckpos;
    public LayerMask groundLayer;
    public Collider2D bodyCollider;

    private float speed;

    private void Start()
    {
        health = enemyData.health;
        speed = enemyData.health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Update()
    {
        if (mustPatrol)
        {
            Patrol();
        }

        ProjectileTimer();
        CheckForPlayer();
    }
    private void FixedUpdate()
    {
        if (mustPatrol)
        {
            mustTurn = !Physics2D.OverlapCircle(groundCheckpos.position, 0.1f, groundLayer);
        }
    }


    private void Die()
    {
        Destroy(gameObject);
    }

    private void CheckForPlayer()
    {
        if (enemyData.lookDirections == 1)
        {
            if (enemyData.useProjectile)
            {
                RaycastHit2D hit2D;

                if (isFacingRight)
                {
                    hit2D = Physics2D.Raycast(transform.position, transform.right, enemyData.seeDistance, ~enemyMask);
                }
                else
                {
                    hit2D = Physics2D.Raycast(transform.position, -transform.right, enemyData.seeDistance, ~enemyMask);
                }

                Debug.DrawLine(transform.position, hit2D.point);

                if (hit2D.collider != null)
                {
                    if (hit2D.collider.gameObject.CompareTag("Player"))
                    {
                        Attack(null, isFacingRight);
                    }
                }
            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(meleeHitPoint.position, enemyData.attackRange, playerLayer);

                if (colliders.Length > 0)
                {
                    Attack(colliders[0], isFacingRight);
                }
            }
        }
        else if (enemyData.lookDirections == 2)
        {


            RaycastHit2D hit2D_01 = Physics2D.Raycast(transform.position, transform.right, enemyData.seeDistance, ~enemyMask);
            RaycastHit2D hit2D_02 = Physics2D.Raycast(transform.position, -transform.right, enemyData.seeDistance, ~enemyMask);

            Debug.DrawLine(transform.position, hit2D_01.point);
            Debug.DrawLine(transform.position, hit2D_02.point);

            if (hit2D_01.collider != null)
            {
                if (hit2D_01.collider.gameObject.CompareTag("Player"))
                {
                    Attack(null, true);
                }
            }

            if (hit2D_02.collider != null)
            {
                if (hit2D_02.collider.gameObject.CompareTag("Player"))
                {
                    Attack(null, false);
                }
            }
        }
    }

    private void Attack(Collider2D playerCollider,bool facingRight)
    {
        if (enemyData.useProjectile)
        {
            if (canAttack)
            {
                GameObject projectile = Instantiate(enemyData.projectile, transform.position, enemyData.projectile.transform.rotation);

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                if (facingRight)
                {
                    rb.AddForce(transform.right * enemyData.projectileSpeed);
                }
                else
                {
                    rb.AddForce(-transform.right * enemyData.projectileSpeed);
                }

                projectile.GetComponent<Bullet>().damage = enemyData.damage;

                projectileTimer = enemyData.fireRate;

                canAttack = false;
            }
        }
        else
        {
            playerCollider.gameObject.GetComponent<CharacterController>().TakeDamage(enemyData.damage);
        }
    }

    private void ProjectileTimer()
    {
        if (projectileTimer > 0)
        {
            projectileTimer -= Time.deltaTime;
        }

        if (projectileTimer <= 0)
        {
            canAttack = true;
        }
    }

    void Patrol()
    {
        if (mustTurn || bodyCollider.IsTouchingLayers(groundLayer))
        {
            Flip();
        }

        rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        speed *= -1;
        mustPatrol = true;
        isFacingRight = !isFacingRight;
    }
}
