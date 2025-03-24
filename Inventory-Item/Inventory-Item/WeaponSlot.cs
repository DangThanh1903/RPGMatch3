using UnityEngine.EventSystems;
using UnityEngine;

public class WeaponSlot : InventorySlot
{
    public override bool CanPlaceItem(Item item)
    {
        return item.Type == ItemType.Weapon;
    }
}