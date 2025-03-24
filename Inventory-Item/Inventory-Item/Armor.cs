using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmorType
{
    Helmet,
    Chestplate,
    Leggings
}

[CreateAssetMenu(fileName = "NewArmor", menuName = "Inventory/Armor")]
public class Armor : Item
{
    public ArmorType armorType;
    public int[] setID;

    public override ItemType Type => ItemType.Armor;
}

