using UnityEngine;

public enum EffectType { Burn, Poison, Paralysis, ArmorBreak }

[CreateAssetMenu(fileName = "NewEffect", menuName = "Effects/StatusEffect")]
public class EffectSO : ScriptableObject {
    public EffectType effectType;
    public string effectName;
    public string description;
    public int duration; // Turns active
    public int damagePerTurn;
    public float defenseReduction; // Armor Break %
    public float paralysisChance;  // Chance to skip turn (0-1)
}
