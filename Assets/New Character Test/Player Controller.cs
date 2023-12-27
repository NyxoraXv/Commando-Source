using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runMultiplier = 2f;
    public float walkMultiplier = 0.5f;
    public float jumpForce = 10f;
    public float shootCooldown = 0.5f;
    public Transform weaponAimPoint;
    public Transform weaponAimPointFlipped;
    public GameObject bulletPrefab;
    public float shootAnimationDuration = 0.2f;
    public float rateOfFire = 5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isSprinting = false;
    private bool canShoot = true;
    private bool isFacingRight = true;
    private bool isFalling = false;
    private bool isFiring = false;
    private float timeSinceLastShot = 0f;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private float speed;

    //added variable
    public GameObject foreground;
    private bool wasFiring2;
    private Health health;
    private bool asObjUp = false;
    private bool haveMachineGun = false;
    public GameObject granadeSpawner;
    public GameObject granate;
    Cinemachine.CinemachineBrain cinemachineBrain;

    public RuntimeAnimatorController test;
    public enum CollectibleType
    {
        HeavyMachineGun,
        Ammo,
        MedKit,
    };

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = test;
    }

    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        cinemachineBrain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
        registerHealth();
        isFacingRight = true;
    }

    private void registerHealth()
    {
        health = GetComponent<Health>();
        // register health delegate
        health.onDead += OnDead;
        health.onHit += OnHit;
    }

    void Update()
    {
        HandleInput();
        Move();
        HandleJumping();
        HandleShooting();
        UpdateGroundedStatus();
        UpdateAnimatorParameters();
    }


    private void OnDead(float damage) // health delegate onDead
    {
        Died();
        GameManager.PlayerDied();
        AudioManager.PlayDeathAudio();
    }

    private void OnHit(float damage) // health delegate onHit
    {
        UIManager.UpdateHealthUI(health.GetHealth(), health.GetMaxHealth());
        AudioManager.PlayMeleeTakeAudio();
    }

    void Died()
    {
        animator.SetBool("isDying", true);
    }

    void ThrowGranate()
    {
        if (GameManager.GetBombs() > 0)
        {
            if (MobileManager.GetButtonGrenade())
            {
                GameManager.RemoveBomb();
                if (!wasFiring2)
                {
                    animator.SetBool("isThrowingGranate", true);
                    wasFiring2 = true;
                }
                else
                {
                    animator.SetBool("isThrowingGranate", false);
                    
                }
            }
            else
            {
                animator.SetBool("isThrowingGranate", false);
                wasFiring2 = false;
            }
        }
        else
        {
            /*Animazione in base a se è in piedi o meno*/
            animator.SetBool("isThrowingGranate", false);
            return;
        }
    }

    bool IsOutsideScreen(float moveH)
    {
        //Return a value between [0;1] - 0.5 if the player is in the mid of the camera
        var playerVPPos = Camera.main.WorldToViewportPoint(transform.position);

        //Prevent walking back when camera is blending
        if (moveH < -Mathf.Epsilon && cinemachineBrain.IsBlending)
            return true;

        //Control if the camera is out of the sprite map
        if ((playerVPPos.x < 0.03f || playerVPPos.x > 1 - 0.03f))
            return true;
        return false;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Walkable") || col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Marco Boat"))
        {
            isGrounded = true;
            animator.ResetTrigger("Jump");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.collider.tag);
        if (collision.collider.CompareTag("Water Dead"))
        {
            health.Hit(100);

            if (foreground != null)
                gameObject.transform.parent = foreground.transform;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Roof"))
        {
            asObjUp = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Roof"))
        {
            asObjUp = false;
        }
    }

    private IEnumerator WaitGranate()
    {
        yield return new WaitForSeconds(0.1f);
        BulletManager.GetGrenadePool().Spawn(granadeSpawner.transform.position, granadeSpawner.transform.rotation);
        yield return new WaitForSeconds(0.15f);
    }

    public void getCollectible(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.HeavyMachineGun:
                if (!haveMachineGun)
                {
                    rateOfFire = 10;
                }
                else
                {
                    GameManager.RechargAmmoMG();
                }
                break;
            case CollectibleType.MedKit:
                health.IncreaseHealth();
                break;
            case CollectibleType.Ammo:
                GameManager.AddAmmo();

                if (!haveMachineGun)
                {
                    GameManager.SetHeavyMachineAmmo(0);
                    UIManager.UpdateAmmoUI();
                }
                break;
            default:
                Debug.Log("Collectible not found");
                break;
        }
    }



    //this is main code, not revamp

    void UpdateGroundedStatus()
    {
        // Cast a ray downwards to check if the player is close to the ground
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        // Check if the ray hit something (ground)
        isGrounded = hit.collider != null;

        // Set isFalling based on the vertical velocity
        isFalling = !isGrounded && GetComponent<Rigidbody2D>().velocity.y < 0;
    }


    void HandleInput()
    {
        if (!isFiring)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            bool isRunning = Input.GetKey(KeyCode.LeftShift) && (horizontalInput != 0); // Allow sprinting only when moving

            UpdateSprintingState();
            SetSpeed(horizontalInput, isRunning);
            UpdateAnimatorParameters();
            FlipSprite();

            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            // Player is firing, disable sprinting and jumping
            isSprinting = false;
        }
    }




    void UpdateSprintingState()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(Input.GetAxis("Horizontal")) > 0;
    }


    void SetSpeed(float horizontalInput, bool isRunning)
    {
        speed = isSprinting ? moveSpeed * runMultiplier : (horizontalInput != 0 ? moveSpeed * walkMultiplier : moveSpeed);
    }

    void UpdateAnimatorParameters()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isWalking = Mathf.Abs(horizontalInput) > 0;
        bool isIdle = !isWalking && !isSprinting;

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput * speed));
        animator.SetBool("IsRunning", isSprinting && isWalking);
        animator.SetBool("IsWalking", isWalking && !isSprinting);
        animator.SetBool("OnAir", !isGrounded);
        animator.SetBool("Fall", isFalling);
        animator.SetBool("IsShooting", isFiring);

        // Additional check to prevent sprinting animation when idle
        if (isIdle)
        {
            animator.SetBool("IsRunning", false);
        }
    }




    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(new Vector3(horizontalInput * speed * Time.deltaTime, 0, 0));
    }

    void FlipSprite()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput > 0)
        {
            isFacingRight = true;
        }
        else if (horizontalInput < 0)
        {
            isFacingRight = false;
        }

        if (isFiring)
        {
            // Flip the sprite based on the aiming point during shooting
            isFacingRight = spriteRenderer.flipX ? false : true;
        }

        spriteRenderer.flipX = !isFacingRight;

        weaponAimPoint.gameObject.SetActive(isFacingRight);
        weaponAimPointFlipped.gameObject.SetActive(!isFacingRight);
    }

    void HandleJumping()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpForce);
        animator.SetTrigger("Jump");
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && canShoot && !isFiring)
        {
            isFiring = true;
            animator.SetBool("IsShooting", true);
            Shoot();
            StartCoroutine(ShootingCooldown());
        }

        if (Input.GetMouseButtonUp(0))
        {
            isFiring = false;
            animator.SetBool("IsShooting", false);
        }
    }

    IEnumerator ShootingCooldown()
    {
        while (Input.GetMouseButton(0))
        {
            // Shoot and wait for the specified rate of fire
            Shoot();
            yield return new WaitForSeconds(1f / rateOfFire);
        }

        canShoot = true; // Reset the canShoot flag when the mouse button is released
    }


    void Shoot()
    {
        Transform activeAimPoint = spriteRenderer.flipX ? weaponAimPointFlipped : weaponAimPoint;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePosition - activeAimPoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, activeAimPoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = shootDirection * 10f;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // No cooldown for shooting
        float adjustedAnimationDuration = Mathf.Min(shootAnimationDuration, 1f / rateOfFire);
        Invoke("ResetShootingAnimation", adjustedAnimationDuration);
    }

    void ResetCooldown()
    {
        canShoot = true;
    }

    void ResetShootingAnimation()
    {
        animator.SetBool("IsShooting", false);
        canShoot = true; // Reset the canShoot flag
    }

}
