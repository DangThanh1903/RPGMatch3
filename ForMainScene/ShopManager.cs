using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<Item> _shopItems;
    [SerializeField] private List<GameObject> _shopItemUIs;
    [SerializeField] private TMP_Text _cointText;
    void Awake() {
        UpdateCoinText();
        UpdatePanel();
    }
    private void UpdatePanel()
    {
        foreach (GameObject panel in _shopItemUIs) {
            Image iconImage = panel.transform.Find("Item").GetComponent<Image>();
            Button buyButton = panel.transform.Find("Button").GetComponent<Button>();
            TMP_Text descriptionText = panel.transform.Find("Price/Text").GetComponent<TMP_Text>();
            Item temp = GetRandomItem(_shopItems);
            iconImage.sprite = temp.sprite;
            InventoryItems item = new InventoryItems(temp);
            buyButton.onClick.RemoveListener(() => InventoryManager.Instance.BuyItem(null)); 
            buyButton.onClick.RemoveListener(UpdateCoinText);
            buyButton.onClick.AddListener(() => InventoryManager.Instance.BuyItem(item));
            buyButton.onClick.AddListener(UpdateCoinText);
            descriptionText.text = "" + temp.price;
        }
    }
    private void UpdateCoinText() {
        _cointText.text = "Coin: " + Player.Instance._coin;
    }
    private Item GetRandomItem(List<Item> list) {
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}
