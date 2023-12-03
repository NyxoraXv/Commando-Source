using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class toBoss : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameplayManager.IsPlayer(collision))
        {
            GameplayManager.LoadScene((int)GameplayManager.Missions.Mission3Boss, true);
        }
    }
}
