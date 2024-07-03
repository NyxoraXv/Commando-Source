using System.Collections;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 0.5f; // Adjusted speed
    public float homingDuration = 0.5f; // Adjusted homing duration to a reasonable value
    public Transform target;
    private bool isHoming = true;
    private float damage = 1f; // Damage value initialized to 1, can be adjusted

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Homing());
    }

    IEnumerator Homing()
    {
        float homingTime = 0f;

        while (homingTime < homingDuration)
        {
            if (target != null)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;
                Debug.Log("Homing: Moving towards target");
            }

            homingTime += Time.deltaTime;
            yield return null;
        }

        isHoming = false;
        Debug.Log("Homing complete: Moving straight");
    }

    void Update()
    {
        if (!isHoming)
        {
            transform.position += transform.up * speed * Time.deltaTime; // Ensure it moves forward in its original direction
            Debug.Log("Moving straight");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Hit(damage); // Apply damage to the player
            }

            Destroy(gameObject); // Destroy the projectile on collision with player
        }
    }

    public void SetDamage(float dmg)
    {
        damage = dmg; // Method to set the damage from BossController4
    }
}
