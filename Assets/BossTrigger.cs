using UnityEngine;
using Assets.FantasyMonsters.Scripts;

public class BossTrigger : MonoBehaviour
{
    [SerializeField]
    private BossController4 bossController; // Reference to the BossController4 component

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone!");

            // Check if the bossController reference is set
            if (bossController != null)
            {
                // Activate the boss controller directly
                bossController.gameObject.SetActive(true);
                bossController.ActivateBoss();
            }
            else
            {
                Debug.LogError("BossController4 reference is not set in TriggerZone script!");
            }

            // Deactivate the trigger zone itself
            gameObject.SetActive(false);
        }
    }
}
