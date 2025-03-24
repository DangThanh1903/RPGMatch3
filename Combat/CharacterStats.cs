using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character/Stats")]
public class CharacterStats : ScriptableObject {
    public string characterName;
    public Sprite characterSprite;
    
    public int baseMaxHealth;
    public int baseAttackPower;
    public int baseDefense;
    public float baseCriticalChance;
    public float baseCriticalDamage;
    public float baseLifeSteal;
    public int baseCoin;

}
