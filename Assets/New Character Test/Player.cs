using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MainPlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public Animator bottomAnimator;
    private SpriteRenderer avatar;

    [Header("Walk and Sprint")]
    public float speed;
    public float speedMultiplier = 2f;

    [Header("Jump")]
    public LayerMask groundLayer;
    public float JumpForce;
    private bool IsGrounded;

    [Header("Shoot")]
    public float rateOfFire; // Shots per second
    public GameObject bulletPrefab;
    public Transform Weapon;

    private float timeBetweenShots;
    private float lastShotTime;

    [Header("Aim Points")]
    [SerializeField] private Transform aimPoint;

    [SerializeField] private float TopAngleLimit = 40f;
    [SerializeField] private float BottomAngleLimit = -20f;

    [Header("Knife")]
    public float knifeDamage = 500f; // Adjust damage as needed
    private float knifeRange = 0.35f; // Adjust range as needed
    public LayerMask enemyLayer; // Layer mask for enemies
    public GameObject knifeHitVFXPrefab;

    private bool isKnifing = false;
    public float timeBetweenKnifes = 0.2f; // Time delay between consecutive knife attacks
    private float lastKnifeTime;

    [Header("Grenade")]
    public GameObject grenadePrefab;
    private float lastGrenadeThrowTime;
    public float grenadeCooldown = 2.0f;


    private Vector3 initScale, negatedScale;

    private Vector2 _mouseWorldPos;

    public GameObject foreground;

    private Health health;
    private bool isDead;

    private bool isHeavyMachineGunActive = false;
    private float heavyMachineGunDuration = 5f;
    private float currentHeavyMachineGunTime = 0f;

    Cinemachine.CinemachineBrain cinemachineBrain;
    public enum CollectibleType
    {
        HeavyMachineGun,
        Ammo,
        MedKit,
    };

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.PlayerController;
        rb = gameObject.GetComponent<Rigidbody2D>();
        avatar = gameObject.GetComponent<SpriteRenderer>();
        cinemachineBrain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
        registerHealth();
        initScale = transform.localScale;
        negatedScale = new Vector3 (-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z );

        speed = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.Levels[CharacterManager.Instance.GetOwnedCharacterLevel(CharacterManager.Instance.selectedCharacter)].Agility*0.2f;
        JumpForce = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.Levels[CharacterManager.Instance.GetOwnedCharacterLevel(CharacterManager.Instance.selectedCharacter)].Agility*1.2f;
        GameManager.addAmmo(250);
        GameManager.SetBombs(15);
    }

    private IEnumerator KnifeAttack()
    {
        isKnifing = true;

        yield return new WaitForSeconds(0.1f); // Adjust delay if needed, this is the animation delay

        // Perform a raycast to detect enemies in front of the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimPoint.right, knifeRange, enemyLayer);

        if (hit.collider != null)
        {
            // Check if the hit object is within the valid range
            float distanceToEnemy = Vector2.Distance(transform.position, hit.point);

            if (distanceToEnemy <= knifeRange)
            {
                // Instantiate the VFX at the hit point
                Instantiate(knifeHitVFXPrefab, hit.point, Quaternion.identity);

                // Damage the enemy
                bottomAnimator.SetTrigger("IsKniving");
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Hit(knifeDamage);
                }
            }
        }

        yield return new WaitForSeconds(timeBetweenKnifes);

        isKnifing = false;
    }



    private void registerHealth()
    {
        health = GetComponent<Health>();
        // register health delegate
        health.onDead += OnDead;
        health.onHit += OnHit;
    }

    private void OnDead(float damage) // health delegate onDead
    {
        Died();
        GameManager.PlayerDied();
        AudioManager.PlayDeathAudio();
        Weapon.gameObject.SetActive(false);
        this.enabled = false;
    }

    void Died()
    {
        animator.SetBool("isDying", true);
        isDead = true;
    }

    public void Revive()
    {
        animator.SetBool("isDying", false);
        isDead = false;
        Weapon.gameObject.SetActive(true);
        this.enabled = true;
        health.Revive();
        registerHealth();

        UIManager.DisableReviveUI();
    }

    private void OnHit(float damage) // health delegate onHit
    {
        if (!isDead) { 
        UIManager.UpdateHealthUI(health.GetHealth(), health.GetMaxHealth());
        AudioManager.PlayMeleeTakeAudio();
        }
    }
    public void getCollectible(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.HeavyMachineGun:
                // Activate the Heavy Machine Gun power-up
                ActivateHeavyMachineGun();
                break;
            case CollectibleType.MedKit:
                health.IncreaseHealth(health.GetMaxHealth() * 0.2f);
                break;
            case CollectibleType.Ammo:
                GameManager.addAmmo(150);
                GameManager.SetBombs(15);
                UIManager.UpdateAmmoUI();
                break;
            default:
                Debug.Log("Collectible not found");
                break;
        }
    }

    private void ActivateHeavyMachineGun()
    {
        isHeavyMachineGunActive = true;
        rateOfFire *= 4f;
        currentHeavyMachineGunTime = 0f;

        // Optionally, you can add visual/audio effects to indicate the activation of the Heavy Machine Gun power-up.
    }

    private void DeactivateHeavyMachineGun()
    {
        isHeavyMachineGunActive = false;
        rateOfFire /= 4f;

        // Optionally, you can add visual/audio effects to indicate the deactivation of the Heavy Machine Gun power-up.
    }

    private void Update()
    {
            HandleInput();
        if (isHeavyMachineGunActive)
        {
            currentHeavyMachineGunTime += Time.deltaTime;

            // Check if the Heavy Machine Gun duration has elapsed
            if (currentHeavyMachineGunTime >= heavyMachineGunDuration)
            {
                // Deactivate the Heavy Machine Gun power-up
                DeactivateHeavyMachineGun();
            }
        }
    }

    private void HandleInput()
    {
        HandleMovement();
        Aim();
        ThrowGrenade();
    }

    private bool CheckEnemiesNearby()
    {
        // Perform a raycast to detect enemies in front of the player
        RaycastHit2D hit = Physics2D.Raycast(aimPoint.position, aimPoint.right, knifeRange, enemyLayer);

        return (hit.collider != null);
    }

    private bool Move()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");

        if (Sprint() && ((horizontalAxis != 0) /*|| MobileManager.GetAxisHorizontal() != 0*/))
        {
            Vector2 movement = new Vector2(horizontalAxis, 0f) * (speed*speedMultiplier);
            rb.velocity = new Vector2(movement.x, rb.velocity.y);
            Weapon.transform.localPosition = new Vector3(-0.032f, 0.295f, 0.3607626f);
            return true;
        }
        else if (!Sprint() && ((horizontalAxis != 0)/* || MobileManager.GetAxisHorizontal() != 0*/))
        {
            Vector2 movement = new Vector2(horizontalAxis, 0f) * speed;
            rb.velocity = new Vector2(movement.x, rb.velocity.y);
            Weapon.transform.localPosition = new Vector3(-0.143f, 0.409f, 0.030607626f);

            return true;
        }
        else
        {
            Weapon.transform.localPosition = new Vector3(-0.143f, 0.409f, 0.030607626f);
            return false;
        }
    }

    private void Aim()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Example value, adjust as needed
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 pivotPoint = Weapon.position;

        Vector2 direction = new Vector2(
            mousePos.x - pivotPoint.x,
            mousePos.y - pivotPoint.y
        );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set the rotation without angle limits
        Weapon.rotation = Quaternion.Euler(0f, 0f, angle);
        aimPoint.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip the character based on the direction
        if (direction.x < 0f)
        {
            gameObject.transform.localScale = negatedScale;
        }
        else
        {
            gameObject.transform.localScale = initScale;
        }

        // Optionally, if you want to flip the weapon sprite based on the direction
        if (direction.x < 0f)
        {
            Weapon.transform.localScale = new Vector3(-1f, -1f, 1f);
        }
        else
        {
            Weapon.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }






    private bool Sprint()
    {
        float sprint = Input.GetAxis("Sprint");

        if (sprint != 0)
        {

            return true;
        }
        else
        {

            return false;
        }
    }


    private void FlipSprite(float axis)
    {
        avatar.flipX = axis != 0;
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walkable") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Marco Boat"))
        {
            IsGrounded = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.collider.tag);
        if (collision.collider.CompareTag("Water Dead"))
        {
            health.Hit(9999);
            animator.SetBool("IsDying", true);

            if (foreground != null)
                gameObject.transform.parent = foreground.transform;
        }
    }


    private bool Jump()
    {
        if (IsGrounded && ((Input.GetAxis("Jump") != 0)/* || (MobileManager.GetButtonJump())*/))
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            IsGrounded = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Shoot()
    {
        float currentTime = Time.time;
        bool fireButtonPressed = Input.GetButton("Fire1");

        // Check if the player is not currently knifing and the fire button is pressed
        if (!isKnifing && fireButtonPressed && GameManager.getAmmo() > 0)
        {
            // Check if there are enemies nearby before shooting
            if (CheckEnemiesNearby())
            {
                // Perform knife attack
                StartCoroutine(KnifeAttack());
            }
            else
            {
                // Otherwise, proceed with shooting
                if (currentTime - lastShotTime >= 1f / rateOfFire)
                {
                    if (bulletPrefab != null)
                    {
                        GameManager.spendAmmo(1);
                        Instantiate(bulletPrefab, aimPoint.position, aimPoint.rotation);
                        lastShotTime = currentTime;
                    }
                    else
                    {
                        Debug.LogError("Bullet prefab is not assigned in the MainPlayer script.");
                    }
                }
            }
        }
    }

    private void ThrowGrenade()
    {
        if (Input.GetAxis("grenade") == 1)
        {
            if (Time.time - lastGrenadeThrowTime >= grenadeCooldown)
            {
                if (GameManager.GetBombs() > 0)
                {
                    bottomAnimator.SetTrigger("IsThrowing");
                    GameManager.RemoveBomb();
                    Instantiate(grenadePrefab, aimPoint.position, aimPoint.rotation);
                    lastGrenadeThrowTime = Time.time;

                }
            }
        }
    }


    private void HandleMovement()
    {
        bool moved = Move();
        animator.SetBool("IsWalking", (moved && !Sprint()));
        animator.SetBool("IsRunning", (moved && Sprint()));
        animator.SetBool("Jump", Jump());
        animator.SetBool("IsGrounded", IsGrounded);
        Shoot();
    }
}
