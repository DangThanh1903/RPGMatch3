using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewLootbox", menuName = "Scriptables/Lootbox")]
public class LootBox : ScriptableObject
{
    public string lootBoxName;
    public Sprite lootBoxSprite;
    public InventoryItems[] items;
    public int[] raritys;
}


