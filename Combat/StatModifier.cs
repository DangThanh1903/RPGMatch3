using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewStatModifier", menuName = "Game/Stat Modifier")]
public class StatModifier : ScriptableObject {
    public List<StatType> types = new List<StatType>();
    public List<float> stats = new List<float>();
    public String decription;
}

public enum StatType {
    Health,
    Attack,
    Defense,
    CritChance,
    LifeSteal
}
