using System;
using UnityEngine;

[Serializable]
public class InventoryItems
{
    public Item item;
    private int stackSize;

    public InventoryItems(Item thing) {
        item = thing;
    }
}
