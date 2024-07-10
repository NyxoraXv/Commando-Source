using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    // Tag of the player GameObject
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            MainPlayer player = other.GetComponent<MainPlayer>();
            if (player != null)
            {
                player.SetWin(true);
            }
        }
    }
}
