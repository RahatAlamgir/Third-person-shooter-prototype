using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventoryData", menuName = "Scriptable Objects/PlayerInventoryData")]

public class PlayerInventoryData : ScriptableObject
{

    [Header("HP")]
    public int health = 100;

    [Header("Money")]
    public int money = 0;


    [Header("Reserve Ammo")]
    public int totalBullets = 120; // What's in your pockets
    public int totalGrenades = 2;

    [Header("Limits")]
    public int maxBullets = 120;
    public int maxGrenades = 2;

    // Use this when pick up an ammo box
    public void AddAmmo(int amount)
    {
        totalBullets = Mathf.Min(totalBullets + amount, maxBullets);
    }

    // Use this the Rifle reloads
    public int ExtractAmmo(int amountNeeded)
    {
        int amountToGive = Mathf.Min(amountNeeded, totalBullets);
        totalBullets -= amountToGive;
        return amountToGive;
    }

    public void ReFillAll()
    {
        totalBullets = maxBullets;
        totalGrenades = maxGrenades;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }
    public void AddMoney(int money)
    {
        this.money += money;
    }
    public bool RemoveMoney(int money)
    {
        if (this.money < money) return false;
        else if (this.money >= money)
        {
            this.money -= money;
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        // This resets your ammo every time the game starts 
        // or when the ScriptableObject is loaded into memory.
        ReFillAll();
        health = 100;
    }
    public bool IsBullelEmpty() => totalBullets <= 0;

}
