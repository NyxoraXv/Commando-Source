using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    public MainPlayer.CollectibleType type = MainPlayer.CollectibleType.HeavyMachineGun;
    private int collectiblePoints = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.IsPlayer(collision))
        {
            GameManager.GetPlayer().GetComponent<MainPlayer>().getCollectible(type);

            if (type == MainPlayer.CollectibleType.Ammo) // collectible sound
            {
                AudioManager.PlayAmmoGrab();
                AudioManager.PlayOkayVoice();
            }
            else if (type == MainPlayer.CollectibleType.MedKit)
            {
                AudioManager.PlayMedKitGrab();
            }
            Destroy(gameObject);
        }
    }
}
