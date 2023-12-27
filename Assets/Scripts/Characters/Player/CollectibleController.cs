using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    public Player.CollectibleType type = Player.CollectibleType.HeavyMachineGun;
    private int collectiblePoints = 1000;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.IsPlayer(collision))
        {
            GameManager.GetPlayer().GetComponent<Player>().getCollectible(type);
            GameManager.AddScore(collectiblePoints);

            if (type == Player.CollectibleType.Ammo) // collectible sound
            {
                AudioManager.PlayAmmoGrab();
                AudioManager.PlayOkayVoice();
            }
            else if (type == Player.CollectibleType.HeavyMachineGun)
            {
                AudioManager.PlayHeavyMachineGunVoice();
            }
            else if (type == Player.CollectibleType.MedKit)
            {
                AudioManager.PlayMedKitGrab();
            }
            Destroy(gameObject);
        }
    }
}
