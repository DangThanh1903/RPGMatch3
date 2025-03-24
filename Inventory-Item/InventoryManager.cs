using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;
using UnityEngine.SceneManagement;
using System.Linq;


public enum SlotType { Inventory, Armor, Weapon, Accessory, Trash }

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; 
    public InventoryData inventoryData;
    public InventoryUI inventoryUI;
    public Item emptySlotItem;

    public List<InventoryItems> inventorySlots = new List<InventoryItems>();
    public List<InventoryItems> armorSlots = new List<InventoryItems>();
    public List<InventoryItems> weaponSlots = new List<InventoryItems>();
    public List<InventoryItems> accessorySlots = new List<InventoryItems>();

    public List<StatModifier> statModifiers = new List<StatModifier>();
    public List<ArmorSet> allArmorSets;

    public int inventorySize = 20; 
    public const int armorSlotCount = 3; 
    public int weaponSlotCount = 3; 
    public int accessorySlotCount = 4; 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start() {
        InitializeEmptySlots();
        LoadInventoryFromDatabase();
        CheckStatBonus();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StartCoroutine(DelayLoad());
    }
    private IEnumerator DelayLoad() {
        yield return new WaitForSeconds(0.1f); 
        if (inventoryUI != null)
        {
            inventoryUI.PopulateUI();
        }
    }
    public void SetEquipedWeapon() {
        for (int i = 0; i < 4; i++) {
            Weapon weapon = (Weapon)inventoryData.weaponItems[i].item;
            BoardController.Instance.weaponType[i] = weapon;
        }
    }
    private void InitializeEmptySlots()
    {
        for (int i = 0; i < inventorySize; i++)
            inventorySlots.Add(new InventoryItems(emptySlotItem));

        for (int i = 0; i < armorSlotCount; i++)
            armorSlots.Add(new InventoryItems(emptySlotItem));

        for (int i = 0; i < weaponSlotCount; i++)
            weaponSlots.Add(new InventoryItems(emptySlotItem));

        // for (int i = 0; i < accessorySlotCount; i++)
        //     accessorySlots.Add(new InventoryItems(emptySlotItem));
    }

    public void LoadInventoryFromDatabase()
    {
        if (inventoryData == null) return;

        foreach (InventoryItems itemInSlot in inventoryData.inventoryItems)
        {
            ReplaceFirstEmptySlot(inventorySlots, itemInSlot);
        }
        foreach (InventoryItems itemInSlot in inventoryData.weaponItems)
        {
            ReplaceFirstEmptySlot(weaponSlots, itemInSlot);
        }
        foreach (InventoryItems itemInSlot in inventoryData.armorItems)
        {
            ReplaceFirstEmptySlot(armorSlots, itemInSlot);
        }
        // foreach (InventoryItems itemInSlot in inventoryData.accessoryItems)
        // {
        //     ReplaceFirstEmptySlot(accessorySlots, itemInSlot);
        // }
    }
    private void ReplaceFirstEmptySlot(List<InventoryItems> slotList, InventoryItems newItem)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].item.Type == ItemType.Null)
            {
                slotList[i] = newItem;
                return;
            }
        }
    }

    public void UpdateSlotData(InventoryItems item, SlotType fromSlot, SlotType toSlot, int newIndex)
    {
        List<InventoryItems> sourceList = GetInventoryList(fromSlot);
        List<InventoryItems> targetList = GetInventoryList(toSlot);

        List<InventoryItems> sourceData = GetInventoryDataList(fromSlot);
        List<InventoryItems> targetData = GetInventoryDataList(toSlot);

        if (sourceList == null || targetList == null) return;
        if (sourceData == null || targetData == null) return;
        // Remove item from source slot
        sourceList.Remove(item);
        sourceData.Remove(item);

        // Insert item in target slot
        targetList.Insert(newIndex, item);
        targetData.Insert(newIndex, item);

        if (inventoryUI != null)
        {
            inventoryUI.PopulateUI();
        }
        CheckStatBonus();
    }

    private List<InventoryItems> GetInventoryList(SlotType slotType)
    {
        switch (slotType)
        {
            case SlotType.Inventory: return inventorySlots;
            case SlotType.Armor: return armorSlots;
            case SlotType.Weapon: return weaponSlots;
            default: return null;
        }
    }
    public bool AddItem(InventoryItems newItem, SlotType slotType)
    {
        List<InventoryItems> targetList = GetInventoryList(slotType);
        List<InventoryItems> inventoryData = GetInventoryDataList(slotType);

        if (targetList == null) return false;

        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i].item.Type == ItemType.Null)
            {
                targetList[i] = newItem;
                inventoryData[i] = newItem;
                inventoryUI.PopulateUI();
                return true;
            }
        }
        return false;
    }
    public bool RemoveItem(InventoryItems item, SlotType slotType)
    {
        List<InventoryItems> targetList = GetInventoryList(slotType);
        if (targetList == null) return false;

        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i] == item)
            {
                targetList[i] = new InventoryItems(emptySlotItem);
                List<InventoryItems> inventoryData = GetInventoryDataList(slotType);
                inventoryData[i] = new InventoryItems(emptySlotItem);
                inventoryUI.PopulateUI();
                return true;
            }
        }
        return false;
    }

    private List<InventoryItems> GetInventoryDataList(SlotType slotType)
    {
        switch (slotType)
        {
            case SlotType.Inventory: return inventoryData.inventoryItems;
            case SlotType.Armor: return inventoryData.armorItems;
            case SlotType.Weapon: return inventoryData.weaponItems;
            case SlotType.Accessory: return inventoryData.accessoryItems;
            default: return null;
        }
    }

    private void CheckArmorSetBonus()
    {
        Dictionary<int, int> setIDs = new Dictionary<int, int>();
        foreach (InventoryItems item in armorSlots)
        {
            if (item.item.Type == ItemType.Null) continue;
            Armor armor = (Armor)item.item;
            foreach (int setID in armor.setID)
            {
                if (setIDs.ContainsKey(setID))
                {
                    setIDs[setID]++;
                }
                else
                {
                    setIDs.Add(setID, 1);
                }
            }
            foreach (KeyValuePair<int, int> set in setIDs)
            {
                if (set.Value >= 3)
                {
                    ArmorSet armorSet = allArmorSets.Find(x => x.setID == set.Key);
                    foreach (StatModifier stat in armorSet.statModifiers)
                    {
                        statModifiers.Add(stat);
                    }
                }
            }
        }
    }
    private void CheckItemBonus() {
        foreach (InventoryItems item in armorSlots) {
            foreach (StatModifier stat in item.item.statModifiers) {
                statModifiers.Add(stat);
            }
        }
    }
    private void CheckStatBonus() {
        statModifiers.Clear();
        CheckArmorSetBonus();
        CheckItemBonus();
        Player.Instance.CalculateStat(statModifiers);
    }

    public void BuyItem(InventoryItems item) {
        AddItem(item, SlotType.Inventory);
        int coin = Player.Instance._coin;
        if (coin >= item.item.price) {
            Player.Instance._coin -= item.item.price;
        }
    }
}

