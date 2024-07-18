using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractWeapon
{
    float shotsPerSecond { get; set; }

    AbstractAmmo ammo { get; }

    public AbstractWeapon()
    {
        shotsPerSecond = 1.0f;
        ammo = new BasicAmmo();
    }
}

interface IDamagable
{
    float Health { get; set; }
    void Heal(float health);
    void DamageHealth(float damage);
}

interface IShielded
{
    bool ShieldIsOn { get; }
    float ShieldAmmount { get; set; }
    void RestoreShield(float ammount);
    void DamageShield(float damage);
}

interface IArmored
{
    float ArmorAmmount { get; }
    void RepairArmor(float ammount);
    void DamageArmor(float damage);
}


public abstract class BasicEnemy : MonoBehaviour, IDamagable
{
    public float Health { get; set; }

    public void Heal(float health)
    {
        Health += health;
        Debug.Log("Restored health: " + Health);
    }

    public void DamageHealth(float damage)
    {
        Health -= damage;
    }
}


public abstract class ShieldedEnemy : BasicEnemy, IShielded
{
    bool shiledIsOn;
    [SerializeField]
    public bool ShieldIsOn { get { return shiledIsOn; } }
    public float ShieldAmmount { get; set; }
    public void RestoreShield(float ammount)
    {
        Debug.Log("Restored shield: " + ammount);
    }
    public void DamageShield(float damage)
    {
        float lastValue = ShieldAmmount;
        ShieldAmmount -= damage;
        Debug.Log("Shieled damaged! From: " + lastValue + " to: " + ShieldAmmount);
        if (ShieldAmmount < 0)
        {
            shiledIsOn = false;
            Debug.Log("Shieled destroyed!");
        }
    }
}

public abstract class ArmoredEnemy : BasicEnemy, IArmored
{
    public float ArmorAmmount { get; set; }
    public void RepairArmor(float ammount)
    {
        Debug.Log("Restored armor: " + ammount);
    }
    public void DamageArmor(float damage)
    {
        float lastValue = ArmorAmmount;
        ArmorAmmount -= damage;
        Debug.Log("Armor damaged! From: " + lastValue + " to: " + ArmorAmmount);
        if (ArmorAmmount < 0)
        {
            Debug.Log("Armor destroyed!");
        }
    }
}

