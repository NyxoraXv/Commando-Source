using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterTheMosquitos : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (GameplayManager.IsPlayer(collider))
        {
            CameraManager.AfterMosquitos();
            Destroy(gameObject);
        }
    }
}
