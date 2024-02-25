using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectWalletButton : MonoBehaviour
{
    public void connectWallet()
    {
        WalletChain.Instance.GetWalletP();
    }
}
