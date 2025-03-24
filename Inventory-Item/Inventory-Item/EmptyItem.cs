using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NullItem", menuName = "Inventory/NullItem")]
public class EmptyItem : Item
{
    public override ItemType Type => ItemType.Null;
}
