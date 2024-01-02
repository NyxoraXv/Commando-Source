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
    public float speed = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.Levels[CharacterManager.Instance.GetOwnedCharacterLevel(CharacterManager.Instance.selectedCharacter)].Agility/10;
    public float speedMultiplier = 2f;

    [Header("Jump")]
    public LayerMask groundLayer;
    public float JumpForce = ((CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.Levels[CharacterManager.Instance.GetOwnedCharacterLevel(CharacterManager.Instance.selectedCharacter)].Agility)/10);
    private bool IsGrounded;

    [Header("Shoot")]
    public float rateOfFire; // Shots per second
    public GameObject bulletPrefab;

    private float timeBetweenShots;
    private float lastShotTime;

    [Header("Aim Points")]
    [SerializeField] private Transform aimPointLeft;
    [SerializeField] private Transform aimPointRight;
    [SerializeField] private Transform aimPointRightDown;
    [SerializeField] private Transform aimPointRightUp;
    [SerializeField] private Transform aimPointLeftDown;
    [SerializeField] private Transform aimPointLeftUp;

    private Health health;

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
        rb = gameObject.GetComponent<Rigidbody2D>();
        avatar = gameObject.GetComponent<SpriteRenderer>();
        cinemachineBrain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
        registerHealth();
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
        Destroy(this);
    }

    void Died()
    {
        animator.SetBool("isDying", true);
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
                GameManager.AddAmmo();
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
    }

    private bool Move()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");

        if (Sprint() && ((horizontalAxis != 0) || MobileManager.GetAxisHorizontal() != 0))
        {
            Vector2 movement = new Vector2(horizontalAxis, 0f) * (speed*speedMultiplier);
            rb.velocity = new Vector2(movement.x, rb.velocity.y);
            FlipSprite(horizontalAxis);
            return true;
        }
        else if (!Sprint() && ((horizontalAxis != 0) || MobileManager.GetAxisHorizontal() != 0))
        {
            Vector2 movement = new Vector2(horizontalAxis, 0f) * speed;
            rb.velocity = new Vector2(movement.x, rb.velocity.y);
            FlipSprite(horizontalAxis);
            return true;
        }
        else
        {
            return false;
        }
    }

    private float Aim()
    {
        if (Application.isMobilePlatform)
        {
            return MobileManager.GetAxisVertical();
        }
        else
        {
            return Input.GetAxis("Vertical");
        }
    }

    private bool Sprint()
    {
        float sprint = Input.GetAxis("Sprint");
        if(sprint != 0)
        {
            return sprint !=0;

        }else if (MobileManager.GetButtonGrenade()) {

            return MobileManager.GetButtonGrenade();

        }
        else
        {
            return false;
        }
    }

    private void FlipSprite(float axis)
    {
        if(axis < 0)
        {
            avatar.flipX = true;
        }else if(axis > 0)
        {
            avatar.flipX = false;
        }
        else
        {
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walkable") || collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walkable") || collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IsGrounded = false;
        }
    }


    private bool Jump()
    {
        if (IsGrounded && ((Input.GetAxis("Jump") != 0) || (MobileManager.GetButtonJump())))
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            return true;
        }
        else
        {
            return false;
        }
    }

    void Shoot()
    {
        float currentTime = Time.time;
        bool fireButtonDown = Input.GetButtonDown("Fire1");

        if ((fireButtonDown || MobileManager.GetButtonFire1()) && currentTime - lastShotTime >= timeBetweenShots)
        {
            if (bulletPrefab != null)
            {
                Transform aimPoint = GetAimPoint();
                Instantiate(bulletPrefab, aimPoint.position, aimPoint.rotation);

                lastShotTime = currentTime;
            }
            else
            {
                Debug.LogError("Bullet prefab is not assigned in the MainPlayer script.");
            }
        }
    }



    Transform GetAimPoint()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Transform selectedAimPoint = null;

        if(verticalInput != 0)
        {
            if (verticalInput > 0 && avatar.flipX == false)
            {
                selectedAimPoint = aimPointRightUp;
            } else if (verticalInput > 0 && avatar.flipX == true)
            {
                selectedAimPoint = aimPointLeftUp;
            } else if (verticalInput < 0 && avatar.flipX == false)
            {
                selectedAimPoint = aimPointRightDown;
            } else if (verticalInput < 0 && avatar.flipX == true)
            {
                selectedAimPoint = aimPointLeftDown;
            }
        }
        else
        {
            if (avatar.flipX == true)
            {
                selectedAimPoint = aimPointRight;
            }
            else
            {
                selectedAimPoint = aimPointLeft;
            }
        }

        return selectedAimPoint;
    }


    private void HandleMovement()
    {
        animator.SetBool("IsWalking", (Move() && !Sprint()));
        animator.SetBool("IsRunning", (Move() && Sprint()));
        animator.SetFloat("Aim", Aim());
        animator.SetBool("Jump", Jump());
        animator.SetBool("IsGrounded", IsGrounded);
        Shoot();
    }
}
