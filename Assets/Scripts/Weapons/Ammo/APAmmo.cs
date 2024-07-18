using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APAmmo : AbstractAmmo
{
    public APAmmo() : base(DamageType.ArmorPiercing) {;}
    public APAmmo(float damage) : base(damage, DamageType.ArmorPiercing) {; }

}
