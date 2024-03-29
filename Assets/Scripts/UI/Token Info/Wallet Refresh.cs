using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletRefresh : MonoBehaviour
{
    private void OnEnable()
    {
        if (SaveManager.Instance.isWalletConnected)
        {
            this.gameObject.SetActive(true);
        }
        else { this.gameObject.SetActive(false); }
    }
}
