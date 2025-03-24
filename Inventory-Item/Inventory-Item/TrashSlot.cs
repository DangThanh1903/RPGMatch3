using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : InventorySlot, IDropHandler
{
    public override bool CanPlaceItem(Item item) => true;
    public override void OnDrop(PointerEventData eventData)
    {
        // Get the dragged item
        DragableItem dragableItem = eventData.pointerDrag.GetComponent<DragableItem>();
        SlotType fromSlotType = dragableItem.GetCurrentSlotType();

        if (dragableItem != null)
        {
            // Remove the item from InventoryManager
            InventoryManager.Instance.RemoveItem(dragableItem.itemInSlot, fromSlotType);
        }
    }
}
