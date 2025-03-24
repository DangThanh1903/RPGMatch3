using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ArmorSlot : InventorySlot
{
    public ArmorType armorType;

    public override bool CanPlaceItem(Item item)
    {
        Armor armor = item as Armor;
        return armor != null && armor.Type == ItemType.Armor && armor.armorType == armorType;
    }
    public override void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DragableItem dragableItem = droppedItem.GetComponent<DragableItem>();
        if (dragableItem == null || dragableItem.itemInSlot == null) return;

        base.OnDrop(eventData);
    }
}