using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour {
    private int accumulatedDamage = 0;
    [SerializeField] Text damageText;
    void Awake() {
        GameController.Instance.matchManager = this;
    }
    private void OnEnable() {
        EventBus.OnWeaponDestroyed += AddDamage;
    }

    private void OnDisable() {
        EventBus.OnWeaponDestroyed -= AddDamage;
    }

    private void AddDamage(int damage) {
        accumulatedDamage += damage;
        Debug.Log($"Total Damage Accumulated: {accumulatedDamage}");
        UpdateDamageText();
    }
    private void UpdateDamageText() {
        damageText.text = $"{accumulatedDamage}";
        int sizeOfFont = Mathf.RoundToInt(((float)accumulatedDamage/100) * 200);
        damageText.fontSize = sizeOfFont;
    }

    public int GetAccumulatedDamage() => accumulatedDamage;

    public void ResetDamage() {
        accumulatedDamage = 0;
        damageText.text = "";
    } 
}
