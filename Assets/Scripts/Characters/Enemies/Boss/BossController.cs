using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{

    [Header("Enemy information")]
    GameObject followPlayer;
    public float attackDamage = 25f;
    public bool isMovable = true;
    public AudioClip deathClip;
    private Health health;
    private float maxHealth;
    private BlinkingSprite blinkingSprite;
    public Transform bossSpawner;
    public GameObject projSpawner;
    private float spawnOffsetUp = 0.05f;

    public GameObject boat;
    private bool isSpawned = false;

    [Header("Knockback")]
    public float knockbackForce = 2.0f; // Adjust the value as needed


    [Header("Speeds")]
    public float speed = 0.7f;
    private float chargingSpeed = 0f;
    private float restSpeed = 0.10f;
    private float sprintSpeed = 2.05f;
    private float initialSpeed = 0.7f;
    

    [Header("Throwable")]
    public GameObject normalFire;
    public GameObject heavyBomb;
    public bool canThrow = true;

    [Header("Enemy activation")]
    public const float CHANGE_SIGN = -1;

    private Rigidbody2D rb;
    private Animator animator;



    [Header("Time shoot")]
    private float shotTime = 0.0f;
    public float fireDelta = 0.5f;
    private float nextFire = 2f;

    [Header("Time sprint")]
    private bool canSprint = true;

    [Header("Camera")]
    public Parallaxing parallax;
    public RunningTarget runningTarget;

    private Health bossHealth;

    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
       
        followPlayer = GameManager.GetPlayer();
        registerHealth();
        maxHealth = health.GetMaxHealth();
        rb = GetComponent<Rigidbody2D>();
        blinkingSprite = GetComponent<BlinkingSprite>();

        bossHealth = GetComponent<Health>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.IsGameOver())
            return;

        if (!isSpawned && followPlayer.transform.position.x >= bossSpawner.position.x)
        {
            isSpawned = true;
            //parallax.setActive(false);
            AudioManager.StartBossAudio();
            StartCoroutine(Spawn());
        }

        if (isSpawned)
        {
            if (health.IsAlive())
            {
                if (health.GetHealth() <= maxHealth / 2)
                {
                    StartCoroutine(HalfHealth());
                }
                /*Run and attacks*/
                float playerDistance = transform.position.x - followPlayer.transform.position.x;

                if (rb && isMovable)
                {
                    rb.MovePosition(rb.position + new Vector2(1 * speed, 0) * Time.deltaTime);
                }

                if (canSprint && Random.Range(0, 100) < 10) // 10% chance of sprint
                {
                    canSprint = false;
                    StartCoroutine(Sprint());
                }

                if (!(health.GetHealth() <= maxHealth / 2) && this.transform.position.y >= -.9f)
                {
                    shotTime = shotTime + Time.deltaTime;

                    if (shotTime > nextFire)
                    {
                        nextFire = shotTime + fireDelta;

                        StartCoroutine(WaitFire(normalFire));

                        nextFire = nextFire - shotTime;
                        shotTime = 0.0f;
                    }
                }
                else if (this.transform.position.y >= -.9f && health.GetHealth() <= maxHealth / 2)
                {
                    shotTime = shotTime + Time.deltaTime;

                    if (shotTime > nextFire)
                    {
                        nextFire = shotTime + fireDelta;

                        StartCoroutine(WaitFire(heavyBomb));

                        nextFire = nextFire - shotTime;
                        shotTime = 0.0f;
                    }
                }
            }
        }
    }

    private void registerHealth()
    {
        health = GetComponent<Health>();


        // register health delegate
        health.onDead += OnDead;
        health.onHit += OnHit;
        
    }

    private void OnHit(float damage)
    {
        blinkingSprite.Play();

        float currentBossHealth = bossHealth.GetHealth();
        float maxBossHealth = bossHealth.GetMaxHealth();

        // Update the boss's health in the UI
        UIManager.UpdateBossHealthUI(currentBossHealth, maxBossHealth);

        if (!bossHealth.IsAlive())
        {
        // Deactivate boss health bar when the boss is defeated
        UIManager.HideBossHealthBar();
        }
    }

    private IEnumerator Die()
    {
        AudioManager.PlayDestroy3();
        animator.SetBool("isDying", true);
        if (rb)
            rb.isKinematic = true;
        if (GetComponent<BoxCollider2D>())
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (GetComponent<CapsuleCollider2D>())
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
        }

        yield return new WaitForSeconds(1.8f);
        AudioManager.PlayDestroy1();
        Destroy(gameObject);
    }

    public void setFollow(GameObject follow)
    {
        followPlayer = GameManager.GetPlayer();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(7f);

        while (this.transform.position.y < -.1f)
        {
            this.transform.position = new Vector3( this.transform.position.x, this.transform.position.y + spawnOffsetUp, this.transform.position.z);
            yield return new WaitForSeconds(.1f);
        }

        CameraManager.AfterBossSpawn();
        runningTarget.SetRunning(true);

        rb.simulated = true;
        yield return new WaitForSeconds(1.75f);

        runningTarget.SetSpeed(initialSpeed);
    }

    private IEnumerator HalfHealth()
    {
        animator.SetBool("isHalfHealth", true);
        projSpawner.transform.position.Set(-0.63f, -0.21f,0);
        yield return new WaitForSeconds(1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.collider != null)
    {
        if (GameManager.IsPlayer(collision))
        {
            Health playerHealth = followPlayer.GetComponent<Health>();
            playerHealth.Hit(attackDamage);

            // Calculate the direction from the boss to the player for the knockback effect
            Vector2 knockbackDirection = (followPlayer.transform.position - transform.position).normalized;

            // Apply the knockback force to the player's Rigidbody2D
            Rigidbody2D playerRigidbody = followPlayer.GetComponent<Rigidbody2D>();
            playerRigidbody.velocity = Vector2.zero; // Reset player's velocity
            playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
        else if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.gameObject.GetComponent<Health>().onHit(attackDamage);

            // Calculate the knockback direction based on the collision
            Vector2 knockbackDirection = (collision.collider.transform.position - transform.position).normalized;

            // Apply the knockback force to the colliding enemy's Rigidbody2D
            Rigidbody2D enemyRigidbody = collision.collider.gameObject.GetComponent<Rigidbody2D>();
            enemyRigidbody.velocity = Vector2.zero; // Reset enemy's velocity
            enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
}


    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject != null)
        {
            if (collider.CompareTag("Walkable"))
            {
                GameObject bridge = collider.gameObject;
                StartCoroutine(DestroyBridge(bridge));
            }
        }

    }

    private IEnumerator WaitFire(GameObject throwableObj)
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(throwableObj, projSpawner.transform.position, projSpawner.transform.rotation);
        yield return new WaitForSeconds(0.15f);
    }

    private IEnumerator DestroyBridge(GameObject bridge)
    {
        if (bridge)
        {
            if (bridge)
                bridge.GetComponent<Animator>().SetBool("onDestroy", true);
            yield return new WaitForSeconds(0.2f);
            if (bridge)
                bridge.GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(1f);
            if (bridge)
                bridge.GetComponent<Animator>().SetBool("onDestroy", false);
            Destroy(bridge);
        }

    }

    private void OnDead(float damage)
    {
        this.GetComponent<Animator>().SetBool("isDying", true);
        GameManager.AddRewardAll(500, 0.23f, 25f, 250);
        StopCoroutine("Sprint");
        StopCoroutine("WaitFire");
        GameManager.PlayerWin();
        StopBossCoroutines();
    }

    private void StopBossCoroutines()
    {
        StopAllCoroutines();
    }

    private IEnumerator Sprint()
    {
        speed = chargingSpeed;
        yield return new WaitForSeconds(1.5f);
        runningTarget.SetRunning(true);
        speed = sprintSpeed;
        yield return new WaitForSeconds(1.2f);
        speed = restSpeed;
        yield return new WaitForSeconds(1f);
        speed = initialSpeed;
        yield return new WaitForSeconds(5f); // wait until next possible sprint
        canSprint = true;
    }
}
