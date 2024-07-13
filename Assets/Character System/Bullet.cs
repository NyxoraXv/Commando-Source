using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private float expireTime;
    private bool isSpawned;

    public float bulletSpeed = 10f;  // New variable for bullet speed
    public float lifeTime = 5f;
    public float damageShot;

    private ParticleSystem particleSystem;
    public enum LauncherType
    {
        Player,
        Enemy
    };
    public LauncherType launcher = LauncherType.Player;

    // Prefab to instantiate when needed
    public GameObject explosionPrefab;

    void OnEnable()
    {
        damageShot = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.Levels[CharacterManager.Instance.GetOwnedCharacterLevel(CharacterManager.Instance.selectedCharacter)].Damage;
        Init();
    }

    private void Init()
    {
        expireTime = lifeTime;
        Vector3 bulletDirection = transform.right;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = bulletDirection * bulletSpeed;  // Set the velocity based on bullet speed
        isSpawned = true;
        AudioManager.PlayNormalShotAudio();
    }


    void Update()
    {
        if (!isSpawned)
            return;

        expireTime -= Time.deltaTime;
        if (expireTime <= 0)
            Despawn();
    }

    private void Despawn()
    {
        if (!isSpawned)
            return;
        isSpawned = false;

        rb.velocity = Vector2.zero;

        // Instantiate the explosion
        GameObject explosionGO = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ParticleSystem explosionPS = explosionGO.GetComponent<ParticleSystem>();

        // Check if the explosion particle system exists
        if (explosionPS != null)
        {
            // Get the duration of the particle system
            float explosionDuration = explosionPS.main.duration;

            // Destroy the explosion game object after the particle system has finished
            Destroy(explosionGO, explosionDuration);
        }
        else
        {
            // If there's no particle system, just destroy the explosion game object
            Destroy(explosionGO);
        }

        // Despawn the bullet based on who launched it
        if (launcher == LauncherType.Player)
        {
            BulletManager.GetNormalBulletPool()?.Despawn(gameObject);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Destroy the bullet when out of camera
    private void OnBecameInvisible()
    {
        Despawn();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((launcher != LauncherType.Enemy && (collider.CompareTag("Enemy")) || collider.CompareTag("EnemyBomb")) || (GameManager.IsPlayer(collider) && launcher != LauncherType.Player) || collider.CompareTag("Building") || collider.CompareTag("Roof") || collider.CompareTag("Walkable"))
        {
            if (!collider.CompareTag("Walkable"))
            {
                if (GameManager.IsPlayer(collider))
                    GameManager.GetPlayer(collider).GetComponent<Health>()?.Hit(damageShot);
                else
                    collider.gameObject.GetComponent<Health>()?.Hit(damageShot);

                AudioManager.PlayShotHitAudio();
                Despawn();
            }
            Despawn();
        }
    }
}
