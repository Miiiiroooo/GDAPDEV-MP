using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // FIELDS
    [SerializeField] private int maxHP = 3;
    [SerializeField] private int currentHP = 3;
    [SerializeField] private int currentGold = 0;

    [SerializeField] private List<BasicWeaponTemplate> weaponList = new List<BasicWeaponTemplate>();
    [SerializeField] private BasicWeaponTemplate.WeaponType currentWeaponColor = BasicWeaponTemplate.WeaponType.Unknown;
    [SerializeField] private int currentWeaponIndex = -1;

    [SerializeField] private AudioSource playerShotsAudio;
    [SerializeField] private AudioSource reloadAudio;
    [SerializeField] private AudioSource noAmmoAudio;

    private bool isHiding = false;

    // PROPERTIES
    public int MaxHP
    {
        get { return maxHP; }
        private set { maxHP = value; }
    }

    public int CurrentHP
    { 
        get { return currentHP; } 
        private set { currentHP = value; }
    }

    public int CurrentGold
    {
        get { return this.currentGold; }
        private set { this.currentGold = value; }
    }

    public BasicWeaponTemplate.WeaponType CurrentWeaponColor
    {
        get { return this.currentWeaponColor; }
        private set { this.currentWeaponColor = value; }
    }

    public AudioSource PlayerShotsAudio
    {
        get { return this.playerShotsAudio; }
        private set { this.playerShotsAudio = value; }
    }

    public AudioSource ReloadAudio
    {
        get { return this.reloadAudio; }
        private set { this.reloadAudio = value; }
    }

    public AudioSource NoAmmoAudio
    {
        get { return this.noAmmoAudio; }
        private set { this.noAmmoAudio = value; }
    }

    // UNITY METHODS
    void Awake()
    {
        BasicWeaponTemplate redWeapon = ScriptableObject.CreateInstance<BasicWeaponTemplate>();
        redWeapon.Type = BasicWeaponTemplate.WeaponType.Red;
        weaponList.Add(redWeapon);

        BasicWeaponTemplate greenWeapon = ScriptableObject.CreateInstance<BasicWeaponTemplate>();
        greenWeapon.Type = BasicWeaponTemplate.WeaponType.Green;
        weaponList.Add(greenWeapon);

        BasicWeaponTemplate blueWeapon = ScriptableObject.CreateInstance<BasicWeaponTemplate>();
        blueWeapon.Type = BasicWeaponTemplate.WeaponType.Blue;
        weaponList.Add(blueWeapon);

        currentWeaponIndex = 0;
        currentWeaponColor = weaponList[0].Type;

        UpdateFromPreviousPlayerData();
    }

    // Methods for HP
    public void IncreaseMaxHP(int value)
    {
        this.maxHP += value;
    }

    public void DamagePlayer(int value)
    {
        if (this.isHiding || DebugHandler.Instance.IsPlayerInvulvenrable)
            return;

        this.currentHP -= value;

        if (SystemInfo.supportsVibration)
            Handheld.Vibrate();

        if (this.currentHP <= 0)
            GameManager.Instance.UpdateGameState(GameManager.GameStates.GameOver);
    }

    public void HealPlayer(int value)
    {
        this.currentHP += value;

        if (this.currentHP > this.maxHP)
            this.currentHP = this.maxHP;
    }

    // Methods for Gold
    public void AddPlayerGold(int value)
    {
        this.currentGold += value;

        if(this.currentGold > 99999)
        {
            this.currentGold = 99999;
        }
    }

    public bool DoesPlayerHaveEnoughGold(int value)
    {
        return this.currentGold >= value;
    }

    public void ReducePlayerGold(int value)
    {
        this.currentGold -= value;
    }

    // Methods for Weapons
    public BasicWeaponTemplate GetCurrentWeapon()
    {
        return weaponList[currentWeaponIndex];
    }

    public void IncreaseAmmoCount(int value)
    {
        for (int i = 0; i < weaponList.Count; i++)
        {
            weaponList[i].IncreaseAmmoCount(value);
        }
    }

    public void SwitchWeapons(int dir)
    {
        if (dir != 1 && dir != -1)
            return;


        this.currentWeaponIndex += dir;

        if (this.currentWeaponIndex > 2)
            this.currentWeaponIndex = 0;
        else if (this.currentWeaponIndex < 0)
            this.currentWeaponIndex = this.weaponList.Count - 1;

        this.currentWeaponColor = this.weaponList[currentWeaponIndex].Type;
    }

    // other player methods
    public void OnPlayerHide(bool isHiding)
    {
        this.isHiding = isHiding;
    }

    public PlayerData CreatePlayerData()
    {
        PlayerData playerData = new()
        { 
            MaxHP = this.maxHP,
            CurrentGold = this.CurrentGold,
            RedAmmoMaximumCount = this.weaponList[0].GetMaximumAmmo(),
            GreenAmmoMaximumCount = this.weaponList[1].GetMaximumAmmo(),
            BlueAmmoMaximumCount = this.weaponList[2].GetMaximumAmmo(),
        };

        return playerData;
    }

    private void UpdateFromPreviousPlayerData()
    {
        PlayerData prevData = GameManager.Instance.GetPlayerData();

        this.maxHP = prevData.MaxHP;
        this.currentHP = this.maxHP;
        this.currentGold = prevData.CurrentGold;

        int increase = prevData.RedAmmoMaximumCount - this.weaponList[0].GetMaximumAmmo();
        this.weaponList[0].IncreaseAmmoCount(increase);
        this.weaponList[0].Reload();

        increase = prevData.GreenAmmoMaximumCount - this.weaponList[1].GetMaximumAmmo();
        this.weaponList[1].IncreaseAmmoCount(increase);
        this.weaponList[0].Reload();

        increase = prevData.BlueAmmoMaximumCount - this.weaponList[2].GetMaximumAmmo();
        this.weaponList[2].IncreaseAmmoCount(increase);
        this.weaponList[0].Reload();
    }

    public void OnRevive()
    {
        this.currentHP = this.maxHP;

        foreach (BasicWeaponTemplate weapon in this.weaponList)
        {
            weapon.Reload();
        }
    }

}
