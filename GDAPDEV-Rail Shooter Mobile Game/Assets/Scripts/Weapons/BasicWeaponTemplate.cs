using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class BasicWeaponTemplate : ScriptableObject
{
    // ENUMS
    public enum WeaponType
    { 
        Unknown,
        Red, 
        Green,
        Blue,
    }

    // FIELDS
    [SerializeField] private WeaponType type = WeaponType.Unknown;
    [SerializeField] private int baseAmmoCount = 5;
    [SerializeField] private int increaseAmmoCount = 0;
    [SerializeField] private int currentAmmoCount = 5;

    // PROPERTIES
    public WeaponType Type
    { 
        get { return this.type; }
        set { this.type = value; }
    }

    // USER-DEFINED METHODS
    public int GetMaximumAmmo()
    {
        return this.baseAmmoCount + this.increaseAmmoCount;
    }

    public int GetCurrentAmmoCount()
    {
        return this.currentAmmoCount;
    }

    public void IncreaseAmmoCount(int value)
    {
        if (value < 0)
            return;

        this.increaseAmmoCount += value;
    }

    public void ReduceCurrentAmmo(int value)
    {
        if (value != 1 || DebugHandler.Instance.HasUnlimitedBullets)
            return;

        this.currentAmmoCount -= value;
    }

    public void Reload()
    {
        this.currentAmmoCount = this.baseAmmoCount + this.increaseAmmoCount;
    }
}
