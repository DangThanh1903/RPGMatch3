using UnityEngine;
using System.Collections.Generic;

public enum ItemType { Null, Weapon, Armor, Accessory }


public abstract class Item : ScriptableObject {
    public string itemName;
    public Sprite sprite;
    public int price;
    public string description;
    public List<StatModifier> statModifiers;
    public abstract ItemType Type { get; }
}
