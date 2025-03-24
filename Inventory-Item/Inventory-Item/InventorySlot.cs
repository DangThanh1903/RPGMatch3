using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IInventorySlot
{
    public virtual bool CanPlaceItem(Item item) => true;

    public Image itemIcon;
    public SlotType slotType; // Determine if it's an inventory, weapon, armor, or accessory slot

    protected virtual void Awake()
    {
        itemIcon = GetComponent<Image>();
    }

    public void SetItem(InventoryItems item)
    {
        Transform itemChild = transform.GetChild(0); // Get the first child UI
        Image itemImage = itemChild.GetComponent<Image>();
        DragableItem dragInventoryItem = itemChild.GetComponent<DragableItem>();
        dragInventoryItem.itemInSlot = item;
        if (item == null || item.item.Type == ItemType.Null)
        {
            itemImage.enabled = false;
            itemImage.sprite = null;
        }
        else
        {
            itemImage.sprite = item.item.sprite;
            itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1f);
            itemImage.preserveAspect = true;
            itemImage.enabled = true;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DragableItem dragableItem = droppedItem.GetComponent<DragableItem>();
        if (dragableItem == null || dragableItem.itemInSlot == null) return;

        InventorySlot targetSlot = GetComponent<InventorySlot>();
        if (targetSlot == null) return;

        InventorySlot originalSlot = dragableItem.parentAfterDrag.GetComponent<InventorySlot>();
        if (originalSlot == null) return;

        SlotType fromSlotType = originalSlot.slotType;
        SlotType toSlotType = targetSlot.slotType;
        Transform existingItem = transform.GetChild(0);
        DragableItem existingDragableItem = existingItem.GetComponent<DragableItem>();

        if ((fromSlotType != SlotType.Inventory) &&
        ((existingDragableItem.GetCurrentItemInSlotType() == ItemType.Weapon && fromSlotType != SlotType.Weapon) ||
        (existingDragableItem.GetCurrentItemInSlotType() == ItemType.Armor && fromSlotType != SlotType.Armor) ||
        (existingDragableItem.GetCurrentItemInSlotType() == ItemType.Accessory && fromSlotType != SlotType.Accessory)))
        {
            return;
        }
        // Allow Inventory slots to accept any item, but enforce rules for other slots
        if (toSlotType != SlotType.Inventory &&
            ((dragableItem.GetCurrentItemInSlotType() == ItemType.Weapon && toSlotType != SlotType.Weapon) ||
            (dragableItem.GetCurrentItemInSlotType() == ItemType.Armor && toSlotType != SlotType.Armor) ||
            (dragableItem.GetCurrentItemInSlotType() == ItemType.Accessory && toSlotType != SlotType.Accessory)))
        {
            return;
        }

        // Check if target slot already has an item
        if (transform.childCount > 0)
        {
            if (existingDragableItem != null)
            {
                // Swap positions between slots
                existingItem.SetParent(dragableItem.parentAfterDrag);
                existingItem.localPosition = Vector3.zero;

                dragableItem.transform.SetParent(transform);
                dragableItem.transform.localPosition = Vector3.zero;

                // Update inventory data for both items
                InventoryManager.Instance.UpdateSlotData(existingDragableItem.itemInSlot, toSlotType, fromSlotType, originalSlot.transform.GetSiblingIndex());
                InventoryManager.Instance.UpdateSlotData(dragableItem.itemInSlot, fromSlotType, toSlotType, targetSlot.transform.GetSiblingIndex());
            }
        }
        else
        {
            // Move item if slot is empty
            dragableItem.transform.SetParent(transform);
            dragableItem.transform.localPosition = Vector3.zero;

            // Update inventory
            InventoryManager.Instance.UpdateSlotData(dragableItem.itemInSlot, fromSlotType, toSlotType, targetSlot.transform.GetSiblingIndex());
        }
    }

}
