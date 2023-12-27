using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runMultiplier = 2f;
    public float walkMultiplier = 0.5f;
    public float jumpForce = 10f;
    public float shootCooldown = 0.5f;
    public Transform weaponAimPoint;
    public GameObject bulletPrefab;
    public float shootAnimationDuration = 0.2f;
    public float rateOfFire = 5f;  // Shots per second

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float speed;
    private bool isGrounded;
    private bool isSprinting = false;
    private bool canShoot = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleInput();
        Move();
        HandleJumping();
        HandleShooting();
    }

    void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        UpdateSprintingState();

        SetSpeed(horizontalInput, isRunning);

        bool isWalking = !isSprinting && horizontalInput != 0;

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput * speed));
        animator.SetBool("IsRunning", isSprinting || isRunning);
        animator.SetBool("IsWalking", isWalking);

        FlipSprite(horizontalInput);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void UpdateSprintingState()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
    }

    void SetSpeed(float horizontalInput, bool isRunning)
    {
        if (isSprinting)
        {
            speed = moveSpeed * runMultiplier;
        }
        else if (horizontalInput != 0)
        {
            speed = moveSpeed * walkMultiplier;
        }
        else
        {
            speed = moveSpeed;
        }
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(new Vector3(horizontalInput * speed * Time.deltaTime, 0, 0));
    }

    void FlipSprite(float horizontalInput)
    {
        spriteRenderer.flipX = horizontalInput < 0;
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
        if (Input.GetMouseButton(0) && canShoot)
        {
            animator.SetBool("IsShooting", true);
            Shoot();
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Reset the shooting animation when the left mouse button is released
            animator.SetBool("IsShooting", false);
        }
    }



    void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, weaponAimPoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = shootDirection * 10f;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        canShoot = false;
        Invoke("ResetCooldown", 1f / rateOfFire);

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
    }
}
