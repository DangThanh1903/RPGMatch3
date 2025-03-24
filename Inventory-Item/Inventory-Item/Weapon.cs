using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    public int damage;
    public float attackSpeed;

    public override ItemType Type => ItemType.Weapon;
}

