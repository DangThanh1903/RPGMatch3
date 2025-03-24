using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory System/Inventory")]
public class InventoryData : ScriptableObject
{
    public List<InventoryItems> inventoryItems;
    public List<InventoryItems> armorItems;
    public List<InventoryItems> weaponItems;
    public List<InventoryItems> accessoryItems;
}

