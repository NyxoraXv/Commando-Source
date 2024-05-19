using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fall : MonoBehaviour

{
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah objek yang bersentuhan memiliki tag "Player"
        if (other.CompareTag("Player"))
        {
            // Dapatkan komponen Health dari objek Player
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Set nilai health menjadi 0
                playerHealth.health = 0;
            }
        }
    }
}
