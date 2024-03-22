using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentCurrencyDisplay : MonoBehaviour
{
    public TextMeshProUGUI frg, lunc;

    public void refresh()
    {
        frg.text = GameManager.getFRG().ToString();
        lunc.text = GameManager.getLUNC().ToString();
    }


}
