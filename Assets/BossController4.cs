using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.FantasyMonsters.Scripts.Tweens;

namespace Assets.FantasyMonsters.Scripts
{
    public class BossController4 : MonoBehaviour
    {
        [Header("Enemy information")]
        GameObject followPlayer;
        public float speed = 0.5f;
        public float attackDamage = 10f;
        public bool isMovable = true;
        public bool canMelee = true;
        public AudioClip deathClip;
        public AudioClip meleeAttackClip;
        public AudioClip rangeAttackClip;
        private Health health;
        private BlinkingSprite blinkingSprite;
        public GameObject projSpawner;

        [Header("Throwable")]
        public GameObject throwableObj;
        public bool canThrow = false;

        [Header("Enemy activation")]
        private bool isActive = false;
        public float attackDistance = 0.7f;         // Far attack
        public float meleeDistance = 0.5f;          // Near attack
        public const float CHANGE_SIGN = -1;
        private Rigidbody2D rb;
        public bool facingRight = false;

        // Enemy gravity
        public bool collidingDown = false;
        Vector2 velocity = Vector2.zero;

        [Header("Time shoot")]
        private float shotTime = 0.0f;
        public float fireDelta = 0.5f;
        private float nextFire = 0.5f;
        public float rangedDelta = 2f;
        public SpriteRenderer Head;
        public List<Sprite> HeadSprites;
        public Animator animator;
        public bool Variations;
        public event Action<string> OnEvent = eventName => { };
        private bool canFall = false;

        [Header("Weapon")]
        public SpriteRenderer weaponRenderer; // Declare the weapon SpriteRenderer

        [Header("Dash Settings")]
        public float dashCooldown = 5f; // Cooldown between dashes
        public float dashDuration = 1f; // Duration of the dash preparation and execution
        public float dashSpeed = 5f;    // Speed of the dash
        private float dashTimer;
        private bool isDashing;
        private bool dashPreparing;


        public void Awake()
        {
            SetState(MonsterState.Idle);
            followPlayer = GameManager.GetPlayer();
            Debug.Log("Follow player assigned: " + (followPlayer != null));
            rb = GetComponent<Rigidbody2D>();
            blinkingSprite = GetComponent<BlinkingSprite>();
            registerHealth();
            checkCanFall();

            if (Variations)
            {
                var variations = GetComponents<MonsterVariation>();
                var random = UnityEngine.Random.Range(0, variations.Length + 1);

                if (random > 0)
                {
                    variations[random - 1].Apply();
                }
            }

            GetComponent<LayerManager>().SetSortingGroupOrder((int)-transform.localPosition.y);

            var stateHandler = animator.GetBehaviours<StateHandler>().SingleOrDefault(i => i.Name == "Death");

            if (stateHandler)
            {
                stateHandler.StateExit.AddListener(() => SetHead(0));
            }

            gameObject.SetActive(false);  // Ensure the boss is inactive by default
        }

        public void ActivateBoss()
        {
            isActive = true;
            Debug.Log("Boss activated!");
        }

        public void Event(string eventName)
        {
            OnEvent(eventName);
        }

        public virtual void Spring()
        {
            ScaleSpring.Begin(this, 1f, 1.1f, 40, 2);
        }

        public void SetState(MonsterState state)
        {
            if (animator != null)
            {
                animator.SetInteger("State", (int)state);
            }
        }

        public void SetHead(int index)
        {
            if (index != 2 && animator.GetInteger("State") == (int)MonsterState.Death) return;

            if (index < HeadSprites.Count)
            {
                Head.sprite = HeadSprites[index];
            }
        }

        private void checkCanFall()
        {
            foreach (var parameter in animator.parameters)
            {
                if (parameter.name == "isFalling")
                {
                    canFall = true;
                    break;
                }
            }
        }

        public void setFollow()
        {
            followPlayer = GameManager.GetPlayer();
        }

        private void registerHealth()
        {
            health = GetComponent<Health>();
            // register health delegate
            health.onDead += OnDead;
            health.onHit += OnHit;
        }

        private void Update()
        {
            if (GameManager.IsGameOver() || !isActive)
                return;

            if (!isDashing && !dashPreparing)
            {
                dashTimer += Time.deltaTime;

                if (dashTimer >= dashCooldown)
                {
                    float playerDistance = followPlayer.transform.position.x - transform.position.x;
                    StartCoroutine(PrepareDash(playerDistance));
                }
            }
        }


        void FixedUpdate()
        {
            if (GameManager.IsGameOver() || !isActive)
                return;

            if (health.IsAlive())
            {
                float playerDistance = 0;
                FlipShoot();
                try
                {
                    playerDistance = followPlayer.transform.position.x - transform.position.x;
                }
                catch (Exception e) { }

                // Log player distance for debugging
                Debug.Log("Player distance: " + playerDistance);

                // Check if basic attack conditions are met before considering the dash
                if (Mathf.Abs(playerDistance) <= meleeDistance && canMelee)
                {
                    Debug.Log("Performing melee attack");
                    // Attack player - Primary attack (near)
                    animator.SetTrigger("Attack");

                    rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

                    shotTime += Time.deltaTime;

                    if (shotTime > nextFire)
                    {
                        nextFire = shotTime + fireDelta;

                        // Check also the correct height
                        if (Mathf.Abs(weaponRenderer.bounds.SqrDistance(followPlayer.transform.position)) <= meleeDistance)
                        {
                            followPlayer.GetComponent<Health>().Hit(1);
                            Debug.Log("Hit player with melee attack");
                            if (meleeAttackClip)
                                AudioManager.PlayEnemyAttackAudio(meleeAttackClip);
                        }

                        nextFire -= shotTime;
                        shotTime = 0.0f;
                    }
                }
                else if (!isDashing && !dashPreparing) // Only consider dash if not already dashing or preparing to dash
                {
                    // Dash logic
                    if (dashTimer >= dashCooldown && Mathf.Abs(playerDistance) <= attackDistance)
                    {
                        StartCoroutine(PrepareDash(playerDistance));
                    }
                    else if (Mathf.Abs(playerDistance) <= attackDistance && canThrow)
                    {
                        Debug.Log("Performing ranged attack");
                        // Attack player - Secondary attack (far)
                        if (!canMelee)
                            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        else
                            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                        shotTime += Time.deltaTime;

                        if (shotTime > nextFire)
                        {
                            nextFire = shotTime + rangedDelta;

                            StartCoroutine(WaitSecondaryAttack());

                            nextFire -= shotTime;
                            shotTime = 0.0f;
                        }
                    }
                    else
                    {
                        // Move towards the player if no attack actions are possible
                        float movementDirection = Mathf.Sign(playerDistance);

                        // Set animation state
                        SetState(MonsterState.Walk);

                        // Move the enemy towards the player
                        Vector2 targetPosition = rb.position + new Vector2(movementDirection * speed * Time.deltaTime, 0);
                        rb.MovePosition(targetPosition);
                    }
                }

                // Flip enemy
                if (playerDistance < 0 && !facingRight)
                {
                    Flip();
                }
                else if (playerDistance > 0 && facingRight)
                {
                    Flip();
                }
            }
        }

        void Flip()
        {
            // Get the direction from the enemy to the player
            Vector3 directionToPlayer = transform.position - followPlayer.transform.position;

            // Determine if the player is on the right side of the enemy
            bool playerIsOnRight = directionToPlayer.x > 0;

            // Update the facingRight variable based on player position
            facingRight = playerIsOnRight;

            // Flip the enemy's scale based on the facingRight variable
            float scaleX = Mathf.Abs(transform.localScale.x) * (facingRight ? 1 : -1);
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

            // Debug log for tracking
            Debug.Log("Facing Right: " + facingRight + ", Player Distance: " + directionToPlayer.x);
        }



        void FlipShoot()
        {
            if (projSpawner == null)
                return;

            if (facingRight)
            {
                // Fire right
                projSpawner.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                // Fire left
                projSpawner.transform.rotation = Quaternion.Euler(0, 0, -180);
            }
        }

        private void OnDead(float damage)
        {
            StartCoroutine(Die());
            SetState(MonsterState.Death);
        }

        private void OnHit(float damage)
        {
            animator.SetTrigger("isHitten");
            blinkingSprite.Play();
        }

        public ParticleSystem deathParticlesPrefab; // Reference to the particle system prefab

        public GameObject deathEffectPrefab; // Reference to the prefab containing a particle system

        private IEnumerator Die()
        {
            GameManager.AddRewardAll(1, 0.01f, 1f, 10);

            PlayDeathAudio();

            // Disable the collider and kinematic behavior
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

            isDashing = false;
            dashTimer = 0f;
            dashPreparing = false;

            // Instantiate the prefab containing the particle system
            if (deathEffectPrefab)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
        }

       private void PlayDeathAudio()
        {
            if (deathClip)
            {
                AudioManager.PlayEnemyDeathAudio(deathClip);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Print the name of the gameobject that the enemy collides with
            Debug.Log("Collided with: " + collision.collider.gameObject.name);

            // Check if the enemy collides with walkable surfaces or specific objects
            if (collision.collider.CompareTag("Walkable") || collision.collider.CompareTag("Marco Boat") || collision.collider.CompareTag("Water Dead"))
            {
                collidingDown = true; // Enemy is colliding with the ground or specific objects
            }

            // Check if the enemy collides with the player
            if (GameManager.IsPlayer(collision))
            {
                // Add a force to the enemy in a random direction
                switch (UnityEngine.Random.Range(0, 2)) // Range should be 0 to 2 to include both 0 and 1
                {
                    case 0:
                        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1f, 2f), ForceMode2D.Impulse);
                        break;
                    case 1:
                        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f, 2f), ForceMode2D.Impulse);
                        break;
                }
            }
            else if (collision.collider.CompareTag("Water Dead"))
            {
                // Enemy dies instantly upon colliding with "Water Dead" tagged objects
                health.Hit(health.GetHealth());
            }
        }


        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Walkable") || collision.collider.CompareTag("Marco Boaat"))
            {
                collidingDown = false;
            }
        }


        private IEnumerator WaitSecondaryAttack()
        {
            yield return new WaitForSeconds(0.5f);
            animator.SetTrigger("SecondaryAttack");
            if (rangeAttackClip)
            {
                AudioManager.PlayEnemyAttackAudio(rangeAttackClip);
            }
            Instantiate(throwableObj, projSpawner.transform.position, projSpawner.transform.rotation);
        }

        private IEnumerator PrepareDash(float playerDistance)
        {
            dashPreparing = true;
            // SetState(MonsterState.Idle); 
            yield return new WaitForSeconds(dashDuration); // Wait for a short duration before dashing
            isDashing = true;
            dashTimer = 0f; // Reset the dash timer
            dashPreparing = false;
            StartCoroutine(ExecuteDash(playerDistance));
        }
        
        private IEnumerator ExecuteDash(float playerDistance)
        {
            Collider2D bossCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = followPlayer.GetComponent<Collider2D>();

            // Ignore collision between boss and player
            Physics2D.IgnoreCollision(bossCollider, playerCollider, true);

            // Disable the boss's collider to prevent collisions with the player during the dash
            bossCollider.enabled = false;

            // Calculate dash direction based on player's position
            float dashDirection = Mathf.Sign(playerDistance);

            // Track the distance traveled during the dash
            float distanceTraveled = 0f;

            // Move backward for a short duration before dashing
            float backwardDuration = 2f; // Adjust this value as needed
            float backwardSpeed = 1f; // Adjust this value as needed

            while (backwardDuration > 0f)
            {
                // Calculate movement distance for this frame
                float movementThisFrame = backwardSpeed * Time.deltaTime;

                // Move the boss backward
                transform.Translate(new Vector3(-dashDirection * movementThisFrame, 0f, 0f));

                // Update backward duration and distance traveled
                backwardDuration -= Time.deltaTime;
                distanceTraveled += Mathf.Abs(movementThisFrame);

                yield return null; // Wait for the next frame
            }

            float stopDuration = 2f; // Adjust this value as needed
            while (stopDuration > 0f)
            {
                stopDuration -= Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Continue with the dash
            distanceTraveled = 0f; // Reset distance traveled for the dash phase
            float dashStartTime = Time.time; // Record the start time of the dash
            while (distanceTraveled < Mathf.Abs(playerDistance) && Time.time - dashStartTime < dashDuration)
            {
                // Calculate movement distance for this frame
                float movementThisFrame = dashSpeed * Time.deltaTime;

                // Move the boss towards the player's position
                transform.Translate(new Vector3(dashDirection * movementThisFrame, 0f, 0f));

                // Update the distance traveled
                distanceTraveled += Mathf.Abs(movementThisFrame);

                yield return null; // Wait for the next frame
            }


            // Re-enable the boss's collider after the dash to allow collisions again
            bossCollider.enabled = true;

            // Re-enable collision between boss and player
            Physics2D.IgnoreCollision(bossCollider, playerCollider, false);

            isDashing = false;
            dashTimer = 0f; // Reset the dash timer
        }
    }
}