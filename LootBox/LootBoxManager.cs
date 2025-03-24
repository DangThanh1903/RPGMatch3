using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxManager : MonoBehaviour
{
    [SerializeField] private List<LootBox> lootBoxs = new List<LootBox>();
    public int currentIndex = 0;

    void Start() {
        SetLootBox(currentIndex);
    }

    public void SetLootBox(int index) {
        if (index < 0 || index >= lootBoxs.Count) return; // Prevent out-of-bounds errors
        
        LootBox lootBox = lootBoxs[index];
        GameObject lootBoxImage = GameObject.Find("LootBoxImage");
        if (lootBoxImage != null) {
            Image image = lootBoxImage.GetComponent<Image>();
            if (image != null) {
                image.sprite = lootBox.lootBoxSprite;
            }
        }
    }

    public void OnLeftButtonClick() {
        currentIndex = (currentIndex - 1 + lootBoxs.Count) % lootBoxs.Count;
        SetLootBox(currentIndex);
    }

    public void OnRightButtonClick() {
        currentIndex = (currentIndex + 1) % lootBoxs.Count;
        SetLootBox(currentIndex);
    }

    public void OnOpenButtonClick() {
        if (lootBoxs.Count == 0) return;

        LootBox lootBox = lootBoxs[currentIndex];
        InventoryItems selectedItem = GetRandomItemBasedOnRarity(lootBox);
        if (selectedItem != null) {
            InventoryManager.Instance.AddItem(selectedItem, SlotType.Inventory);
        }
    }

    private InventoryItems GetRandomItemBasedOnRarity(LootBox lootBox) {
        if (lootBox.items.Length == 0 || lootBox.raritys.Length == 0) return null;

        int totalWeight = 0;
        foreach (int rarity in lootBox.raritys) {
            totalWeight += rarity;
        }

        int randomInt = Random.Range(1, totalWeight + 1);
        int sum = 0;

        for (int i = 0; i < lootBox.items.Length; i++) {
            sum += lootBox.raritys[i];
            if (randomInt <= sum) {
                return lootBox.items[i];
            }
        }
        return null;
    }
}
