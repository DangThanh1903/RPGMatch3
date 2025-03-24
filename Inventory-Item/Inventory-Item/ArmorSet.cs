using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorSet", menuName = "Items/ArmorSet")]
public class ArmorSet : ScriptableObject
{
    public int setID;
    public string setName;
    public List<StatModifier> statModifiers; 
    public int requiredPieces;
}
