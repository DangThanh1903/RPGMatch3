using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public GameObject armorSlotPrefab;
    public GameObject weaponSlotPrefab;
    public GameObject accessorySlotPrefab;
    public Transform inventoryPanel;
    public Transform weaponPanel;
    public Transform armorPanel;
    public Transform accessoryPanel;
    public GameObject inventoryUI;

    private List<GameObject> slots = new List<GameObject>();
    void Awake() {
        InventoryManager.Instance.inventoryUI = this; 
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
    }
    public void PopulateUI()
    {
        // Clear existing slots
        foreach (GameObject slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();

        PopulateSlots(InventoryManager.Instance.inventorySlots, inventoryPanel, SlotType.Inventory);
        PopulateSlots(InventoryManager.Instance.weaponSlots, weaponPanel, SlotType.Weapon);
        PopulateSlots(InventoryManager.Instance.armorSlots, armorPanel, SlotType.Armor);
        // PopulateSlots(InventoryManager.Instance.accessorySlots, accessoryPanel, SlotType.Accessory);
    }

    private void PopulateSlots(List<InventoryItems> itemList, Transform parentPanel, SlotType slotType)
    {
        GameObject slotPrefab = GetPrefabBySlotType(slotType);
        foreach (InventoryItems item in itemList)
        {
            // 1. Instantiate the correct slot prefab
            GameObject slotGO = Instantiate(slotPrefab, parentPanel);

            // 2. Get the InventorySlot component and assign the item
            InventorySlot slot = slotGO.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.SetItem(item);
            }

            // 3. Store the slot for future reference
            slots.Add(slotGO);
        }
        
    }
    // Helper method to select the right prefab
    private GameObject GetPrefabBySlotType(SlotType slotType)
    {
        switch (slotType)
        {
            case SlotType.Weapon: return weaponSlotPrefab;
            case SlotType.Armor: return armorSlotPrefab;
            case SlotType.Accessory: return accessorySlotPrefab;
            default: return inventorySlotPrefab;
        }
    }

    public void OpenInventory()
    {
        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
        }
        else
        {
            inventoryUI.SetActive(true);
        }
    }
    public void OnItemDroppedToTrash(InventoryItems item)
    {
        InventoryManager.Instance.RemoveItem(item, SlotType.Inventory);
    }

}
