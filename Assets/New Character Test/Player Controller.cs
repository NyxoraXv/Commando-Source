using UnityEngine;

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

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private float speed;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isFacingRight = true;
    }

    void Update()
    {
        HandleInput();
        Move();
        HandleJumping();
        HandleShooting();
        UpdateGroundedStatus();
        UpdateAnimatorParameters(); // <-- Removed the parameter
    }

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
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        UpdateSprintingState();
        SetSpeed(horizontalInput, isRunning);
        UpdateAnimatorParameters(); // <-- Removed the parameter
        FlipSprite(horizontalInput);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void UpdateSprintingState()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    void SetSpeed(float horizontalInput, bool isRunning)
    {
        speed = isSprinting ? moveSpeed * runMultiplier : (horizontalInput != 0 ? moveSpeed * walkMultiplier : moveSpeed);
    }

    void UpdateAnimatorParameters()
    {
        bool isWalking = !isSprinting && Input.GetAxis("Horizontal") != 0;
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal") * speed));
        animator.SetBool("IsRunning", isSprinting || Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("OnAir", !isGrounded);
        animator.SetBool("Fall", isFalling);
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(new Vector3(horizontalInput * speed * Time.deltaTime, 0, 0));
    }

    void FlipSprite(float horizontalInput)
    {
        if (horizontalInput > 0)
        {
            isFacingRight = true;
        }
        else if (horizontalInput < 0)
        {
            isFacingRight = false;
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
        if (Input.GetMouseButton(0) && canShoot)
        {
            animator.SetBool("IsShooting", true);
            Shoot();
        }

        if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("IsShooting", false);
        }
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
