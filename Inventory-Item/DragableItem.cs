using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image image;
    [HideInInspector] public Transform parentAfterDrag;
    public InventoryItems itemInSlot;

    void Awake() {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
        image.raycastTarget = true;
    }

    public SlotType GetCurrentSlotType()
    {
        InventorySlot slot = parentAfterDrag.GetComponent<InventorySlot>();
        return slot != null ? slot.slotType : SlotType.Inventory; // Default to Inventory
    }

    public ItemType GetCurrentItemInSlotType() {
        return itemInSlot.item.Type;
    }
}
