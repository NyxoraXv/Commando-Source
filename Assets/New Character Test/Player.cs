using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MainPlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
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

    private Vector3 initScale, negatedScale;

    private Vector2 _mouseWorldPos;

    public GameObject foreground;

    private Health health;
    private bool isDead;

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
        health.IncreaseHealth();

        UIManager.DisableReviveUI();
    }

    private void OnHit(float damage) // health delegate onHit
    {
        UIManager.UpdateHealthUI(health.GetHealth(), health.GetMaxHealth());
        AudioManager.PlayMeleeTakeAudio();
    }

    public void getCollectible(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.HeavyMachineGun:
                UIManager.UpdateAmmoUI();
                break;
            case CollectibleType.MedKit:
                health.IncreaseHealth();
                break;
            case CollectibleType.Ammo:
                GameManager.addAmmo(150);
                    UIManager.UpdateAmmoUI();
                break;
            default:
                Debug.Log("Collectible not found");
                break;
        }
    }

    private void Update()
    {
            HandleInput();
    }

    private void HandleInput()
    {
        HandleMovement();
        Aim();
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

        if (fireButtonPressed /* || MobileManager.GetButtonFire1() */ && GameManager.getAmmo() > 0)
        {
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
