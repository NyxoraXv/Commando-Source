using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.FantasyMonsters.Scripts.Tweens;

namespace Assets.FantasyMonsters.Scripts
{
    public class NewEnemyControl : MonoBehaviour
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
        public float activationDistance = 1.8f;
        public float attackDistance = 0.7f;         //Far attack
        public float meleeDistance = 0.5f;          //Near attack
        public const float CHANGE_SIGN = -1;
        private Rigidbody2D rb;
        public bool facingRight = false;

        //Enemy gravity
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

        void Awake()
        {
            SetState(MonsterState.Idle);
            followPlayer = GameManager.GetPlayer();
            if (followPlayer == null)
            {
                Debug.LogError("Follow player is not assigned.");
            }

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
            if (GameManager.IsGameOver())
                return;
        }

        void FixedUpdate()
        {
            if (GameManager.IsGameOver())
                return;

            if (health.IsAlive())
            {
                if (followPlayer == null)
                {
                    followPlayer = GameManager.GetPlayer();
                    if (followPlayer == null)
                    {
                        Debug.LogError("Follow player is still not assigned.");
                        return;
                    }
                }

                float playerDistance = 0;
                FlipShoot();
                try
                {
                    playerDistance = Vector2.Distance(transform.position, followPlayer.transform.position);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error calculating player distance: " + e.Message);
                    return;
                }

                if (playerDistance < activationDistance)
                {
                    if (playerDistance <= meleeDistance && canMelee)
                    {
                        animator.SetTrigger("Attack");

                        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

                        shotTime += Time.deltaTime;

                        if (shotTime > nextFire)
                        {
                            nextFire = shotTime + fireDelta;

                            // check also the correct height
                            if (Mathf.Abs(weaponRenderer.bounds.SqrDistance(followPlayer.transform.position)) <= meleeDistance)
                            {
                                followPlayer.GetComponent<Health>().Hit(attackDamage);
                                if (meleeAttackClip)
                                    AudioManager.PlayEnemyAttackAudio(meleeAttackClip);
                            }

                            nextFire -= shotTime;
                            shotTime = 0.0f;
                        }
                    }
                    else if (playerDistance <= attackDistance && canThrow)
                    {
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
                        // Move to the player
                        // Debug.Log($"isMovable: {isMovable}, collidingDown: {collidingDown}, canMelee: {canMelee}");
                        if (isMovable && collidingDown)
                        {
                            SetState(MonsterState.Walk);
                            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Ensure we only freeze rotation

                            Vector2 direction = followPlayer.transform.position - transform.position;
                            Vector2 targetPosition = rb.position + direction.normalized * speed * Time.deltaTime;

                            if (IsGrounded(targetPosition)) // Check if the target position is grounded
                            {
                                rb.MovePosition(targetPosition);
                            }
                        }
                        else
                        {
                            SetState(MonsterState.Idle);
                        }
                    }
                }
                /*else    
                {
                    Patrol();
                    return;
                }*/

                // Flip enemy
                if (followPlayer.transform.position.x < transform.position.x && facingRight)
                {
                    Flip();
                }
                else if (followPlayer.transform.position.x > transform.position.x && !facingRight)
                {
                    Flip();
                }
            }
        }

        void Patrol()
        {
            if (isMovable && collidingDown)
            {
                SetState(MonsterState.Walk);
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Ensure we only freeze rotation

                // Determine patrol direction based on facing direction
                Vector2 direction = facingRight ? Vector2.right : Vector2.left;

                // Calculate the target position for patrol
                Vector2 targetPosition = rb.position + direction * speed * Time.deltaTime;

                // Check if the initial target position is grounded
                if (IsGrounded(targetPosition))
                {
                    rb.MovePosition(targetPosition);
                }
                else
                {
                    facingRight = !facingRight; // Example: Reverse direction
                }
            }
            else
            {
                SetState(MonsterState.Idle); // Set to idle state if not movable or not grounded
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
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
            if (collision.collider.CompareTag("Walkable") || collision.collider.CompareTag("Marco Boat"))
            {
                collidingDown = false;
            }
        }

        void Flip()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            facingRight = !facingRight;
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

        private ParticleSystem deathParticlesPrefab; // Reference to the particle system prefab

        private GameObject deathEffectPrefab; // Reference to the prefab containing a particle system

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
                AudioManager.PlayEnemyDeathAudio(deathClip);
        }

        private IEnumerator WaitSecondaryAttack()
        {
            yield return new WaitForSeconds(0.5f);
            animator.SetTrigger("SecondaryAttack");
            if (rangeAttackClip)
            {
                AudioManager.PlayEnemyAttackAudio(rangeAttackClip);
            }

            GameObject projectile = Instantiate(throwableObj, projSpawner.transform.position, projSpawner.transform.rotation);
            HomingProjectile homingProjectile = projectile.GetComponent<HomingProjectile>();
            if (homingProjectile != null)
            {
                homingProjectile.SetDamage(attackDamage); // Set the damage value on the projectile
            }
        }

        private bool IsGrounded(Vector2 position)
        {
            // Perform a box cast or raycast to check if the position is on the ground
            RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(0.1f, 0.1f), 0, Vector2.down, 0.1f, LayerMask.GetMask("Walkable"));
            return hit.collider != null;
        }
    }
}
