using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClick : MonoBehaviour
{
    public enum Weapon
    {
        Glock,
        Revolver
    }
    

    public Weapon SelectedWeapon = Weapon.Glock;

    public void Awake()
    {
        
    }


    public void OnClick(Weapon selectWeapon) {

        SelectedWeapon = selectWeapon;

    }
}
