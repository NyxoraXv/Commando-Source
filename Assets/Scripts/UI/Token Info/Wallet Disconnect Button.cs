using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletDisconnectButton : MonoBehaviour
{
    private void OnEnable()
    {
        if (SaveManager.Instance.playerData.playerInformation.isWalletConnected)
        {
            this.gameObject.SetActive(true);
        }else { this.gameObject.SetActive(false); }
    }
}
