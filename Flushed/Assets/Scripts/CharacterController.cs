using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Run,
        Dash,
        Jump,
        Locked,
    }

    [SerializeField] float health = 100;
    [SerializeField] float invincibilityTime = 0.3f;
    [SerializeField] Transform meleeHitPoint;

    [SerializeField] bool useCameraController;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float dashSpeed = 30f;
    [SerializeField] GameObject crosshairPrefab;

    [SerializeField] List<WeaponData> weapons = new List<WeaponData>();

    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    [SerializeField] KeyCode DashKey = KeyCode.LeftShift;
    [SerializeField] KeyCode ShootKey = KeyCode.Mouse0;

    [SerializeField] GameObject weaponSlot;
    [SerializeField] GameObject launchPoint;
    [SerializeField] Animator anim;
    [SerializeField] Animator weaponAnimator;

    private WeaponData currentWeapon;

    private PlayerState currentState = PlayerState.Idle;

    private GameObject cam;

    private ParticlePalette particlePalette;

    private GameObject crosshair;

    private Rigidbody2D rb;

    private bool isFacingRight = true;

    private bool dashRequest;

    private bool attack;

    private bool isGrounded;

    private Vector3 mousePos;

    private int weaponId;

    private float fireRateTimer;
    private float invincibilityTimer;

    private int currentAmmo;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (mousePos.x > transform.position.x)
        {
            isFacingRight = true;
        }

        if (mousePos.x < transform.position.x)
        {
            isFacingRight = false;
        }

        if (attack)
        {
            StartCoroutine(MeleeWeaponAnimationTime(0.2f));
        }

        CrosshairController();
        HandleInput();

        if (useCameraController)
        {
            CameraController();
        }

        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        if (invincibilityTimer <= 0)
        {
            health -= damage;

            if (health <= 0)
            {
                Die();
            }

            invincibilityTimer = invincibilityTime;
        }    
    }

    private void Die()
    {
        Destroy(gameObject);
    }


    private void Init()
    {
        
        rb = GetComponent<Rigidbody2D>();

        crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);

        cam = GameObject.FindGameObjectWithTag("MainCamera");

        particlePalette = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<ParticlePalette>();

        anim = GetComponent<Animator>();
    }

    private void HandleInput()
    {
        CheckGround();
        Vector2 movement = new Vector2();

        movement.x = Input.GetAxis("Horizontal");

        if (currentState != PlayerState.Locked && currentState != PlayerState.Dash)
        {
            Move(movement);
        }

        if (Input.GetKeyDown(JumpKey) && isGrounded)
        {
            Jump();
        }

        HandleDash();

        if (currentWeapon != weapons[0])
        {
            if (currentWeapon != null)
            {
                if (currentWeapon.useProjectile)
                {
                    if (Input.GetKey(ShootKey))
                    {
                        if (currentWeapon != null)
                        {
                            if (currentWeapon.useProjectile)
                            {
                                Shoot();
                            }
                        }
                    }

                    if (Input.GetKeyDown(ShootKey))
                    {
                        if (currentAmmo <= 0)
                        {
                            ThrowWeapon();
                        }
                    }
                }
                else
                {
                    if (Input.GetKeyDown(ShootKey))
                    {
                        MeleeAttack();
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(ShootKey))
            {
                UseKnife();
            }
        }

        weaponSlot.GetComponent<FollowCursor>().FlipWeapon(isFacingRight);
    }

    private void UseKnife()
    {
        anim.SetTrigger("Knife");
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(DashKey))
        {
            dashRequest = true;
        }

        if (dashRequest)
        {
            StartCoroutine(Dash(0.1f, dashSpeed));
        }
    }

    private IEnumerator Dash(float dashTime, float speed)
    {
        currentState = PlayerState.Dash;

        if (isFacingRight)
        {
            rb.MovePosition(rb.position + new Vector2(1, 0) * speed * Time.deltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + new Vector2(-1, 0) * speed * Time.deltaTime);
        }

        yield return new WaitForSeconds(dashTime);

        dashRequest = false;

        currentState = PlayerState.Idle;
    }

    private void Move(Vector2 movement)
    {
        currentState = PlayerState.Run;

        if (!dashRequest)
        {
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);

            if (movement.x != 0)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }

            if (isFacingRight)
            {
                transform.localRotation = transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.localRotation = transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), Mathf.Infinity, ~playerMask);

        if (hit.distance < 1.2f)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x,jumpHeight);
    }

    private void CrosshairController()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        crosshair.transform.position = new Vector3(mousePos.x, mousePos.y, -5);

        Cursor.visible = false;
    }

    private void CameraController()
    {
        Vector3 cameraShift = mousePos - transform.position;

        float xPos = Mathf.Clamp(cameraShift.x, -1f, 1f);
        float yPos = Mathf.Clamp(cameraShift.y, -1f, 1f);

        cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(transform.position.x + xPos, transform.position.y + yPos + 1, -10), 2f * Time.deltaTime);
    }

    private void PickUpWeapon(int wpId) 
    {
        weaponSlot.GetComponent<SpriteRenderer>().sprite = weapons[wpId].weaponSprite;
        weaponSlot.GetComponent<FollowCursor>().UpdateWeapon(weapons[wpId]);
        currentWeapon = weapons[wpId];
        currentAmmo = weapons[wpId].ammo;
        
    }

    private void Shoot()
    {
        if (fireRateTimer <= 0)
        {
            if (currentWeapon != null)
            {
                if (currentAmmo > 0)
                {
                    GameObject bullet = Instantiate(currentWeapon.projectile, new Vector3(weaponSlot.GetComponentInChildren<Transform>().transform.position.x, weaponSlot.GetComponentInChildren<Transform>().transform.position.y, 2), Quaternion.identity);

                    Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();

                    bulletRB.AddForce(launchPoint.transform.right * currentWeapon.speed);

                    bullet.GetComponent<Bullet>().damage = currentWeapon.damage;

                    bulletRB.transform.rotation = launchPoint.transform.rotation;

                    Destroy(bullet.gameObject, currentWeapon.destroyTime);

                    fireRateTimer = currentWeapon.fireRate / 60;

                    currentAmmo--;
                }
            }
        }
    }

    private void MeleeAttack()
    {
        if (fireRateTimer <= 0)
        {
            if (currentWeapon != null && currentWeapon.animation)
            {
                if (currentAmmo > 0)
                {
                    if (!attack)
                    {
                        weaponAnimator.enabled = true;

                        weaponAnimator.SetTrigger("Attack");
                        attack = true;
                        Collider2D[] enemys = Physics2D.OverlapCircleAll(meleeHitPoint.position, 2, enemyLayer);
                        if (enemys.Length > 0)
                        {

                            foreach (Collider2D enemy in enemys)
                            {
                                enemy.gameObject.GetComponent<Enemy>().TakeDamage(currentWeapon.damage);
                            }

                        }
                        currentAmmo--;

                    }
                }
            }
        }
    }

    private IEnumerator MeleeWeaponAnimationTime(float animationTime)
    {
        yield return new WaitForSeconds(animationTime);
        attack = false;
        weaponAnimator.enabled = false;
    }


    private void ThrowWeapon()
    {
        GameObject bullet = Instantiate(currentWeapon.weapon, new Vector3(weaponSlot.GetComponentInChildren<Transform>().transform.position.x, weaponSlot.GetComponentInChildren<Transform>().transform.position.y, 2), Quaternion.identity);

        bullet.GetComponent<CircleCollider2D>().enabled = false;

        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();

        bulletRB.AddForce(launchPoint.transform.right * currentWeapon.throwSpeed);

        Destroy(bullet.gameObject, currentWeapon.throwDestroyTime);

        PickUpWeapon(0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            weaponId = collision.GetComponent<Weapon>().weaponData.weaponId;
            PickUpWeapon(weaponId);

            Instantiate(particlePalette.WeaponPickupParticle, collision.transform.position, Quaternion.identity);

            Destroy(collision.gameObject);
        }
    }
}
