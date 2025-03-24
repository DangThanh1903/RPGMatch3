
public class AccessorySlot : InventorySlot
{
    public override bool CanPlaceItem(Item item)
    {
        return item.Type == ItemType.Accessory;
    }
    
}

