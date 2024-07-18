using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Basic,
    ArmorPiercing,
    Direct
}
public abstract class AbstractAmmo : MonoBehaviour
{
    public float damage { get; }
    public float speed { get; set; }
    public DamageType damageType { get; }
    public AbstractAmmo(DamageType type) : this(1.0f, type)
    {
    }
    public AbstractAmmo(float dmg = 1.0f, DamageType type = DamageType.Basic, float sp = 1.0f)
    {
        damage = dmg;
        damageType = type;
        speed = sp;
    }
}

public class Ammo : MonoBehaviour
{
    public float speed = 10.0f;
    public float damage = 1.0f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * speed);
    }
}
