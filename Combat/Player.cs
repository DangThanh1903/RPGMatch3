using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private Image HpUI;
    [SerializeField] private CharacterStats _stats;
    private float maxWidth = 188;
    [SerializeField] private int _maxHp = 100;
    [SerializeField] public int _currentHp = 100;
    [SerializeField] private int _attack = 0;
    [SerializeField] private int _defense = 0;
    [SerializeField] private int _lifeSteal = 0; // %
    [SerializeField] private int _critChance = 0; // %
    [SerializeField] private int _critDamage = 0; // %
    [SerializeField] public int _coin = 1;
    private float attackMoveDistance = 0.5f; 
    public float attackMoveDuration = 0.2f;
    private float returnMoveDuration = 0.3f;
    private Vector3 originalPosition;
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        UpdatePlayerStats();
        UpdatePlayerSprite();
        UpdatePlayerCoin();
    }
    void Start() {
        originalPosition = new Vector3(-4, 0, 0);
    }

    public void TakeDamage(int damage) {
        _currentHp -= damage;
        SlashVFXManager.Instance.PlaySlashEffect(new Vector3(transform.position.x + 0.2f, transform.position.y + 0.5f, 0f), 1, false);
        Debug.Log("Player lost " + damage + "HP");
        UpdateHpUI();
    }
    public void AttackAnima() {
        Vector3 attackPosition = originalPosition + new Vector3(attackMoveDistance, 0, 0);

        transform.DOMove(attackPosition, attackMoveDuration).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            transform.DOMove(originalPosition, returnMoveDuration).SetEase(Ease.InQuad);
        });
    }
    private void UpdatePlayerStats() {
        if (_stats == null) return;
        _maxHp = _stats.baseMaxHealth;
        _currentHp = _stats.baseMaxHealth;
        _attack = _stats.baseAttackPower;
        _defense = _stats.baseDefense;
        _lifeSteal = (int)_stats.baseLifeSteal;
        _critChance = (int)_stats.baseCriticalChance;
        _critDamage = (int)_stats.baseCriticalDamage;
    }
    private void UpdatePlayerCoin() {
        _coin = _stats.baseCoin;
    }

    private void UpdatePlayerSprite() {
        if (_stats == null) return;
        GetComponent<SpriteRenderer>().sprite = _stats.characterSprite;
    }

    private void UpdateHpUI() {
        int newWidth = Mathf.RoundToInt((float)_currentHp / _maxHp * maxWidth);
        HpUI.rectTransform.sizeDelta = new Vector2(newWidth, HpUI.rectTransform.sizeDelta.y);
    }
    public void CalculateStat(List<StatModifier> stats) {
        UpdatePlayerStats();
        foreach (StatModifier stat in stats)  {
            for (int i = 0; i < stat.types.Count; i++) {
                switch (stat.types[i])
                {
                    case StatType.Health:
                        Debug.Log($"Applying {(int)stat.stats[i]} to Health.");
                        // Example: Increase player's health
                        _maxHp += (int)stat.stats[i];
                        _currentHp += (int)stat.stats[i];
                        break;
                    
                    case StatType.Attack:
                        Debug.Log($"Applying {(int)stat.stats[i]} to Attack.");
                        // Example: Increase player's attack
                        _attack += (int)stat.stats[i];
                        break;
                    
                    case StatType.Defense:
                        Debug.Log($"Applying {(int)stat.stats[i]} to Defense.");
                        // Example: Increase player's defense
                        _defense += (int)stat.stats[i];
                        break;
                    
                    case StatType.CritChance:
                        Debug.Log($"Applying {(int)stat.stats[i]}% to Crit Chance.");
                        // Example: Increase critical hit chance
                        _critChance += (int)stat.stats[i];
                        break;
                    
                    case StatType.LifeSteal:
                        Debug.Log($"Applying {(int)stat.stats[i]}% to Life Steal.");
                        // Example: Increase life steal
                        _lifeSteal += (int)stat.stats[i];
                        break;
                    
                    default:
                        Debug.LogWarning("Unknown StatType.");
                        break;
                }
            }
        }
    }

}
