using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fell : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.health = 0;
            }
        }
    }
}