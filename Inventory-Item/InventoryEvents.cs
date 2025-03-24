public static class InventoryEvents
{
    public static event System.Action<Item, InventorySlot, InventorySlot> OnItemMoved;

    public static void MoveItem(Item item, InventorySlot fromSlot, InventorySlot toSlot)
    {
        OnItemMoved?.Invoke(item, fromSlot, toSlot);
    }
}
