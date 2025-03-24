using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyUI : MonoBehaviour {
    private Enemy enemy;
    public string enemyName => enemy.enemyName;
    public int maxHP => enemy.maxHealth;
    public int currentHP;
    public int damage => enemy.damage;
    private float maxWidth = 188;
    [SerializeField] Image HpUI;
    public float attackMoveDistance = 0.5f;
    public float attackMoveDuration = 0.2f;
    public float returnMoveDuration = 0.3f;

    private Vector3 originalPosition;
    void Awake() {
        enemy = GameController.Instance.currentEnemy;
        GameController.Instance._enemyUI = this;
        currentHP = maxHP;
        UpdateHpUI();
    }
    void Start() {
        originalPosition = transform.position;
    }

    public void TakeDamage(int damage) {
        if (damage <= 0) {
            return;
        }
        SlashVFXManager.Instance.PlaySlashEffect(transform.position, 0, true);
        currentHP -= damage;
        Debug.Log($"Enemy HP: {currentHP}");
        UpdateHpUI();
    }
    public void AttackAnima() {
        Vector3 attackPosition = originalPosition + new Vector3(-attackMoveDistance, 0, 0);

        transform.DOMove(attackPosition, attackMoveDuration).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            transform.DOMove(originalPosition, returnMoveDuration).SetEase(Ease.InQuad);
        });
    }

    public void UpdateHpUI() {
        int newWidth = Mathf.RoundToInt((float)currentHP / maxHP * maxWidth);
        HpUI.rectTransform.sizeDelta = new Vector2(newWidth, HpUI.rectTransform.sizeDelta.y);
    }

}
