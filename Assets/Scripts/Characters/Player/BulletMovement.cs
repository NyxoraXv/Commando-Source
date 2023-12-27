using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float expireTime;
    private bool isSpawned;

    public float bulletForce = 3;
    public float lifeTime = 5;
    public float damageShot = 1;
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
        //damageShot = CharacterManager.Instance.GetCharacterPrefab(CharacterManager.Instance.selectedCharacter).GetComponent<CharacterInformation>().Character.Levels[CharacterManager.Instance.GetOwnedCharacterLevel(CharacterManager.Instance.selectedCharacter)].Damage;
        Init();
    }

    private void Init()
    {
        
        Debug.Log(damageShot);
        expireTime = lifeTime;
        Vector3 bulletDirection = transform.right;
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(bulletDirection * bulletForce, ForceMode2D.Impulse);
        isSpawned = true;
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

        // Instantiate explosion prefab
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (launcher == LauncherType.Player)
        {
            BulletManager.GetNormalBulletPool()?.Despawn(this.gameObject);
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
        if ((launcher != LauncherType.Enemy && (collider.CompareTag("Enemy")) || collider.CompareTag("EnemyBomb")) || (GameManager.IsPlayer(collider) && launcher != LauncherType.Player) || collider.CompareTag("Building") || collider.CompareTag("Roof"))
        {
            if (GameManager.IsPlayer(collider))
                GameManager.GetPlayer(collider).GetComponent<Health>()?.Hit(damageShot);
            else
                collider.gameObject.GetComponent<Health>()?.Hit(damageShot);

            AudioManager.PlayShotHitAudio();
            Despawn();
        }
    }
}
