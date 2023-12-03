using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    /*
    public static WeaponManager Instance;

    public Weapon selectedWeapon;
    public List<Weapon> ownedWeapons = new List<Weapon>();

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        selectedWeapon = SaveManager.Instance.playerData.weaponInfo.selectedWeapon;
    }

    public void AddOwnedWeapon(Weapon weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
        }
    }

    public void RemoveOwnedWeapon(Weapon weapon)
    {
        if (ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Remove(weapon);
        }
    }

    public void ChangeSelectedWeapon(Weapon weapon)
    {
        if (ownedWeapons.Contains(weapon))
        {
            selectedWeapon = weapon;
        }
    }
    */
}
